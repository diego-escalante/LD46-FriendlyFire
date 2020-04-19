using UnityEngine;

public class PlayerInputs : MonoBehaviour {

    public float horizontal { get; private set; }
    public float vertical { get; private set; }
    public bool attacking { get; private set; }
    public bool action { get; private set; }
    public bool refuel { get; private set; }
    public bool restart { get; private set; }

    void OnDisable() {
        horizontal = 0;
        vertical = 0;
        attacking = false;
        action = false;
        refuel = false;
        restart = false;
    }

    void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        attacking = Input.GetButtonDown("Fire");
        action = Input.GetButtonDown("Action");
        refuel = Input.GetButtonDown("Refuel");
        restart = Input.GetButtonDown("Restart");
    }
}
