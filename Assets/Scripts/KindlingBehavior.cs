using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class KindlingBehavior : MonoBehaviour {

    public bool lit {get; private set;} = false;
    
    public float burningDurationInSeconds = 60f;
    public Light2D fgLight;
    public Light2D bgLight;
    public Collider2D batCollider;

    private float startingFgLightIntensity;
    private float startingBgLightIntensity;
    private float timeLeft;
    private Animator animator;
    private Vector3 startingScale;

    private float minimumMultiplier = 0.1f;
    // private Collider2D[] colls = new Collider2D[10];

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

            checkForBats();
        }
    }

    public void Light() {
        if (!lit) {
            animator.SetTrigger("Light");
            lit = true;
        }
    }

    public void checkForBats() {
        Collider2D[] colls = Physics2D.OverlapBoxAll(batCollider.bounds.center, batCollider.bounds.size, 0f);

        foreach (Collider2D coll in colls) {
            if (coll.gameObject.tag == "Bat") {
                coll.GetComponent<BatBehavior>().getBurntYo(); //What a dumb function name, what was 4am-Me thinking?
            }
        }
    }
}
