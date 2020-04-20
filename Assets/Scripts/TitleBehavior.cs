using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleBehavior : MonoBehaviour {

    private MusicController musicController;

    void Start() {
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<MusicController>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            musicController.musicStartGame();
            SceneManager.LoadScene(1);
        }       
    }
}
