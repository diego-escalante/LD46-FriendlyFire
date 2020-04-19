using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    public Vector2 facingDirection = Vector2.up;
    public GameObject FireSwooshPrefab;
    public float attackCooldown = 0.25f;
    public float searchRadius = 1f;
    public float maxFuelSeconds = 60f;
    public Slider fuelSlider;
    public Text kindlingText;
    public Text goldText;
    
    private int gold = 0;
    private int kindling = 0;
    private float fuelLeftSeconds;
    private float currentCooldownTime = 0;
    private TopDownMovement movementScript;
    private Animator animator;
    private PlayerInputs playerInputs;

    public void Start() {
        fuelLeftSeconds = maxFuelSeconds;
        animator = GetComponent<Animator>();
        movementScript = GetComponent<TopDownMovement>();
        playerInputs = GetComponent<PlayerInputs>();
        if (FireSwooshPrefab == null) {
            Debug.LogError("FireSwooshPrefab has not been picked in the PlayerManager!");
        }
    }

    public void Update() {
        updateFuel();
        updateFacingDirection();
        attack();
        action();
        controlAnimation();
    }

    private void attack() {
        // If not on cooldown and the player attacks, make a swoosh, stop moving.
        if (currentCooldownTime > 0) {
            currentCooldownTime -= Time.deltaTime;
        } else if (playerInputs.attacking) {
            Instantiate(FireSwooshPrefab, transform.position + (1.5f * transform.right), facingDirectionToQuaternion(facingDirection));
            // movementScript.enabled = false;
            currentCooldownTime = attackCooldown;
            animator.SetTrigger("Attack");

            // Choose attack point.
            Vector2 attackPoint = transform.position;
            Vector2 boxSize;
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Up")) {
                attackPoint += Vector2.up * 0.75f;
                boxSize = new Vector2(1.25f, 0.5f);
            } else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Right")) {
                attackPoint += Vector2.right * 0.75f;
                boxSize = new Vector2(0.5f, 1.25f);
            } else if  (animator.GetCurrentAnimatorStateInfo(0).IsTag("Down")) {
                attackPoint += Vector2.down * 0.75f;
                boxSize = new Vector2(1.25f, 0.5f);
            } else {
                attackPoint += Vector2.left * 0.75f;
                boxSize = new Vector2(0.5f, 1.25f);
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint, boxSize, 0f);
            Debug.Log(hits.Length);
            //Boy some object oriented programming would have been nice here.
            foreach (Collider2D hit in hits) {
                switch (hit.gameObject.tag) {
                    case "Kindling":
                        hit.GetComponent<KindlingBehavior>().Light();
                        break;
                }
            }
        }
    }

    private void action() {
        if (playerInputs.action) {
            // If near closed chest, open it.
            ChestBehavior chestBehavior = findNearbyClosedChest();
            if (chestBehavior != null) {
                chestBehavior.Open();
                return;
            }

            // Otherwise, if near kindling, pick it up.
            
            // Otherwise, if have kindling, drop it.
        }
    }

    private void updateFuel() {
        if (fuelLeftSeconds > 0) {
            fuelLeftSeconds -= Time.deltaTime;
            fuelSlider.value = Mathf.Clamp(fuelLeftSeconds/maxFuelSeconds, 0, 1);
            if (fuelLeftSeconds <= 0) {
                animator.SetTrigger("Unlit");
            }
        }        
    }

    private ChestBehavior findNearbyClosedChest() {
        
        // Find all nearby chests.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        if (colliders == null || colliders.Length == 0) {
            return null;
        }

        // If a chest is closed, return that one.
        ChestBehavior chestBehavior;
        foreach (Collider2D coll in colliders) {
            
            if (coll.gameObject.tag != "Chest") {
                // This collider is not a chest, skip.
                continue;
            }

            chestBehavior = coll.GetComponent<ChestBehavior>();
            if (chestBehavior.closed) {
                return chestBehavior;
            }
        }
        return null;
    }

    private void updateFacingDirection() {
        Vector2 velocity = movementScript.getVelocity();

        // Only update direction when moving.
        if (velocity != Vector2.zero) {
            if (velocity.y != 0) {
                // Prioritize up/down over left/right.
                facingDirection = Vector2.up * Mathf.Sign(velocity.y);
            } else {
                facingDirection = Vector2.right * Mathf.Sign(velocity.x);
            }
        }
    }

    // For instantiating fire swooshes based on the direction the player is facing.
    private Quaternion facingDirectionToQuaternion(Vector2 direction) {
        if (direction == Vector2.right) {
            return Quaternion.Euler(0, 0, 0);
        } else if (direction == Vector2.up) {
            return Quaternion.Euler(0, 0, 90);
        } else if (direction == Vector2.left) {
            return Quaternion.Euler(0, 0, 180);
        } else if (direction == Vector2.down) {
            return Quaternion.Euler(0, 0, 270);
        } else {
            Debug.LogError("Facing Direction is NOT up down left or right, and cannot accurately convert to quaternion.");
            return Quaternion.Euler(0, 0, 0);
        }
    }

    private void controlAnimation() {
        Vector2 velocity = movementScript.getVelocity();
        animator.SetInteger("Horizontal", velocity.x == 0 ? 0 : (int)Mathf.Sign(velocity.x));
        animator.SetInteger("Vertical", velocity.y == 0 ? 0 : (int)Mathf.Sign(velocity.y));
    }
}
