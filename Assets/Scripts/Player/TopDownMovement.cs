using UnityEngine;

// For future reuse:
// TopDownMovement shouldn't take input directly, there should be a input manager controlling everything. (Ok, inputs are a little better now but there's still room to improve.)
// It would be nice for TopDownMovement to have acceleration, but its not necessary for this game.

public class TopDownMovement : MonoBehaviour {
    
    public float moveSpeed = 5f;

    // public bool accelerationEnabled = true;
    // public float acceleration = 50f;

    private PlayerInputs playerInputs;
    private Vector2 velocity = Vector2.zero;
    private CollisionChecker collisionChecker;

    public void Start() {
        playerInputs = GetComponent<PlayerInputs>();
        // Make sure this object has a collision checker.
        collisionChecker = GetComponent<CollisionChecker>();
        if (collisionChecker == null) {
            Debug.LogError(string.Format("There is no CollisionChecker on game object {0} for TopDownMovement to work.", gameObject.name));
            this.enabled = false;
        }
    }

    public void Update() {
        
        // Calculate the new velocity based on inputs.
        move();

        // Update velocity based on collisions.
        Vector2 translationVector = velocity * Time.deltaTime;
        Vector2 checkedTranslationVector = collisionChecker.checkForCollisions(translationVector);

        // If the checked translation is different, we hit something, and velocity in that direction should stop.
        if (translationVector.y != checkedTranslationVector.y) {
            velocity.y = 0;
        }
        if (translationVector.x != checkedTranslationVector.x) {
            velocity.x = 0;
        }

        // Actually move the object.
        transform.Translate(checkedTranslationVector, Space.World);
    }

    public Vector2 getVelocity() {
        return velocity;
    }

    private void move() {
        Vector2 targetVelocity = new Vector2(playerInputs.horizontal, playerInputs.vertical).normalized * moveSpeed;
        velocity = targetVelocity;

        // There's really no need to spend time with acceleration here for this project.

        // if (!accelerationEnabled) {
        //     velocity = targetVelocity;
        // } else {
        //     if (targetVelocity == Vector2.zero) {
        //         targetVelocity = -velocity;
        //     }
        //     velocity = Vector2.ClampMagnitude((targetVelocity * acceleration * Time.deltaTime) + velocity, moveSpeed);
        // }
    }
}
