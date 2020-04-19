using UnityEngine;

public class KindlingBehavior : MonoBehaviour {

    public float burningDurationInSeconds = 60f;

    private bool lit = false;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void Light() {
        if (!lit) {
            animator.SetTrigger("Light");
            Destroy(this.gameObject, burningDurationInSeconds);
        }
    }
}
