using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private float _speedBoostedSpeed = 10.0f;
    [SerializeField]
    private float _speedThruster = 8.0f;



    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _shieldStrength = 0;


    private SpawnManager _spawnManager;


    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isthrustersActivated = false;


    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisual;

    [SerializeField]
    private int _score;

    [SerializeField]
    private GameObject[] _engineDamage; //0 = Right 1 = Left

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;
   

    void Start()
    {

        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spanwn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UIManager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

    }

    void CalculateMovement()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedBoostActive == false)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(direction * _speedThruster * Time.deltaTime);
                //add thruster > 0 communicate with ui
            }
            else
            {
                transform.Translate(direction * _speed * Time.deltaTime);
            }
            
        }

        else
        {
            transform.Translate(direction * _speedBoostedSpeed * Time.deltaTime);
        }

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.4f)
        {
            transform.position = new Vector3(-11.4f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.4f)
        {
            transform.position = new Vector3(11.4f, transform.position.y, 0);
        }
    }

    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        else if (_isTripleShotActive == false)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();

    }

    public void Damage()
    {
        

        if (_isShieldActive == true)
        {
            _shieldStrength--;
            
            if (_shieldStrength <= 0)
            {
                _isShieldActive = false;
                _shieldVisual.SetActive(false);
                
            }
            _uiManager.UpdateShields(_shieldStrength);
            return;
            
        }
        

        _lives--;

        if (_lives == 2)
        {
            int randomEngine = Random.Range(0, 2); // 0 = Right 1 = Left
            _engineDamage[randomEngine].SetActive(true);
        }
        else if (_lives == 1)
        {
            _engineDamage[0].SetActive(true);
            _engineDamage[1].SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
            
        }



    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());

    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true; 
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        _shieldStrength++;


        if (_shieldStrength > 0)
        {
            _isShieldActive = true;
        }
        _shieldVisual.SetActive(true);

        if (_shieldStrength > 3)
        {
            _shieldStrength = 3;
        }

        _uiManager.UpdateShields(_shieldStrength);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);

    }

    


}



