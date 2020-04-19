using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class KindlingBehavior : MonoBehaviour {

    public float burningDurationInSeconds = 60f;
    public Light2D fgLight;
    public Light2D bgLight;

    private float startingFgLightIntensity;
    private float startingBgLightIntensity;
    private float timeLeft;
    private bool lit = false;
    private Animator animator;
    private Vector3 startingScale;

    private float minimumMultiplier = 0.1f;

    void Start() {
        animator = GetComponent<Animator>();
        timeLeft = burningDurationInSeconds;
        startingScale = transform.localScale;
        startingBgLightIntensity = bgLight.intensity;
        startingFgLightIntensity = fgLight.intensity;
    }

    void Update() {
        if (lit) {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0) {
                Destroy(this.gameObject);
            }
            float multiplier = Mathf.Max(timeLeft/burningDurationInSeconds, minimumMultiplier);
            transform.localScale = startingScale * multiplier;
            fgLight.intensity = startingFgLightIntensity * multiplier;
            bgLight.intensity = startingBgLightIntensity * multiplier;

            if (multiplier <= minimumMultiplier * 2 && gameObject.layer != 0) {
                gameObject.layer = 0;
            }
        }
    }

    public void Light() {
        if (!lit) {
            animator.SetTrigger("Light");
            lit = true;
        }
    }
}
