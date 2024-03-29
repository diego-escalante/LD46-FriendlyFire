﻿using UnityEngine;
using UnityEngine.UI;

public class ChestBehavior : MonoBehaviour {
    
    public GameObject unlockBar;
    public Slider kkslider;
    public float timeToOpen = 10f;
    public bool closed { get; private set; } = true;
    private bool opening = false;
    private Animator animator;
    private float currentTime;
    private PlayerInputs playerInputs;
    private PlayerManager playerManager;
    private SoundController soundController;

    void Start() {
        timeToOpen = Random.Range(timeToOpen/2f, timeToOpen);
        currentTime = timeToOpen;
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("No player found.");
            return;
        }
        playerInputs = player.GetComponent<PlayerInputs>();
        playerManager = player.GetComponent<PlayerManager>();
        soundController = GameObject.FindGameObjectWithTag("Ladder").GetComponent<SoundController>();
    }

    private void Update() {
        if (opening) {

            // Reset if the player moves or attacks.
            if (playerInputs.horizontal != 0 || playerInputs.vertical != 0 || playerInputs.attacking) {
                opening = false;
                currentTime = timeToOpen;
                kkslider.value = 0;
                unlockBar.SetActive(false);
                return;
            }

            currentTime -= Time.deltaTime;
            if (currentTime <= 0) {
                //Open!
                animator.SetTrigger("Open");
                soundController.playScoreSound();
                playerManager.addGold();
                unlockBar.SetActive(false);
                closed = false;
                Destroy(this);
            }
            kkslider.value = Mathf.Clamp(1 - (currentTime/timeToOpen), 0, 1);
        }
    }

    public void Open() {
        if (closed) {
            opening = true;
            unlockBar.SetActive(true);
        }
    }
}
