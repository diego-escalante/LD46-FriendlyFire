using UnityEngine;

public class SoundController : MonoBehaviour {

    public AudioClip batEnterSound;
    public AudioClip batLeaveSound;
    public AudioClip pickupSound;
    public AudioClip dropSound;
    public AudioClip errorSound;
    public AudioClip hitSound;
    public AudioClip scoreSound;
    public AudioClip swingSound;
    public AudioClip lightSound;

    public AudioSource audioSource;

    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void playBatEnterSound() {
        audioSource.PlayOneShot(batEnterSound);
    }

    public void playBatLeaveSound() {
        audioSource.PlayOneShot(batLeaveSound);
    }

    public void playDropSound() {
        audioSource.PlayOneShot(dropSound);
    }

    public void playPickupSound() {
        audioSource.PlayOneShot(pickupSound);
    }

    public void playErrorSound() {
        if (audioSource.isPlaying) {
            return;
        }
        audioSource.PlayOneShot(errorSound);
    }

    public void playHitSound() {
        audioSource.PlayOneShot(hitSound);
    }

    public void playScoreSound() {
        audioSource.PlayOneShot(scoreSound);
    }

    public void playSwingSound() {
        audioSource.PlayOneShot(swingSound);
    }

    public void playlightSound() {
        audioSource.PlayOneShot(lightSound);
    }
}