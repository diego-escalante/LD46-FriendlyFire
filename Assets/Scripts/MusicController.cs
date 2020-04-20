using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

  public AudioSource all;
  public AudioSource allButDrone;
  public AudioSource justDrumsUp;
  public AudioSource justTune;

  private void Awake() { 
    DontDestroyOnLoad(transform.gameObject); 
    if(GameObject.FindGameObjectsWithTag("MusicController").Length > 1) Destroy(gameObject);
  }

  private void Start() {
    musicPlayerDeath();
  }

  private IEnumerator fadeIn(AudioSource audio, float toVolume=0.8f, float duration=0.5f) {
      float timeLeft = duration;
      float startingVolume = audio.volume;

      while (timeLeft > 0) {
          timeLeft -= Time.deltaTime;
          audio.volume = Mathf.Lerp(startingVolume, toVolume, 1f - (timeLeft/duration));
          yield return null;
      }
      audio.volume = toVolume;
  }

  public void musicStartGame() {
    StartCoroutine(fadeIn(allButDrone));

    StartCoroutine(fadeIn(justDrumsUp, 0));
    StartCoroutine(fadeIn(all, 0));
    StartCoroutine(fadeIn(justTune, 0));
  }

  public void musicPlayerDeath() {
    StartCoroutine(fadeIn(justDrumsUp, 0.8f, 0.1f));

    StartCoroutine(fadeIn(allButDrone, 0, 0.1f));
    StartCoroutine(fadeIn(all, 0, 0.1f));
    StartCoroutine(fadeIn(justTune, 0, 0.1f));
  }

  public void musicUnlit() {
    StartCoroutine(fadeIn(justTune));

    StartCoroutine(fadeIn(justDrumsUp, 0));
    StartCoroutine(fadeIn(allButDrone, 0));
    // StartCoroutine(fadeIn(all, 0));
  }

  public void musicSetDrone(float fraction) {
      float vol = 0.8f * Mathf.Clamp(fraction, 0, 1);
      StartCoroutine(fadeIn(all, vol, 1f));
  }
}