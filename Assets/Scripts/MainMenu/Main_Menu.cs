using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void LoadGame()
    {
        //load the game scene
        SceneManager.LoadScene(1); //main game scene
    }
}
