﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;

    private Animator _anim;

    private BoxCollider2D _boxCol;

    [SerializeField]
    private AudioClip _explosionSoundClip;

    [SerializeField]
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;


    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        _boxCol = GetComponent<BoxCollider2D>();
        if (_boxCol == null)
        {
            Debug.Log("The BoxCollider2D is NULL.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.Log("The AudioSource(Enemy) is NULL.");
        }
        else
        {
            _audioSource.clip = _explosionSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time * _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
        

    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down* _speed * Time.deltaTime);

        if (transform.position.y< -5.1f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }
    
    


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            AudioSource.PlayClipAtPoint(_explosionSoundClip, transform.position);
            Destroy(this.gameObject, 2.8f);
            
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player !=null)
            {
                _player.AddScore(10); 
            }

            _boxCol.enabled = false;
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            AudioSource.PlayClipAtPoint(_explosionSoundClip, transform.position);
            //_audioSource.Play(); (alternative)

            Destroy(this.gameObject, 2.8f);
        }
    }
}
