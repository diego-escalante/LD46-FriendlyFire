using UnityEngine;

public class ChestBehavior : MonoBehaviour {
    
    public bool closed { get; private set; } = true;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void Open() {
        if (closed) {
            animator.SetTrigger("Open");
            closed = false;
        }
    }
}
