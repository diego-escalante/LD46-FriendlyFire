using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    public Vector2 facingDirection = Vector2.up;
    public float attackCooldown = 0.25f;
    public float searchRadius = 1f;
    public float maxFuelSeconds = 60f;
    public Slider fuelSlider;
    public Text kindlingText;
    public Text goldText;
    public GameObject kindlingPrefab;
    
    private int gold = 0;
    private int kindling = 5;
    private float fuelLeftSeconds;
    private float currentCooldownTime = 0;
    private TopDownMovement movementScript;
    private Animator animator;
    private PlayerInputs playerInputs;

    public void Start() {
        kindlingText.text = kindling.ToString();
        goldText.text = gold.ToString();
        fuelLeftSeconds = maxFuelSeconds;
        animator = GetComponent<Animator>();
        movementScript = GetComponent<TopDownMovement>();
        playerInputs = GetComponent<PlayerInputs>();
    }

    public void Update() {
        updateFuel();
        attack();
        action();
        controlAnimation();
    }

    private void attack() {
        // If not on cooldown and the player attacks, make a swoosh, stop moving.
        if (currentCooldownTime > 0) {
            currentCooldownTime -= Time.deltaTime;
        } else if (playerInputs.attacking) {
            // movementScript.enabled = false;
            currentCooldownTime = attackCooldown;
            animator.SetTrigger("Attack");

            // Choose attack point.
            Vector2 attackPoint = getPositionInFrontOfPlayer();
            Vector2 boxSize;
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Up")) {
                boxSize = new Vector2(1.25f, 0.5f);
            } else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Right")) {
                boxSize = new Vector2(0.5f, 1.25f);
            } else if  (animator.GetCurrentAnimatorStateInfo(0).IsTag("Down")) {
                boxSize = new Vector2(1.25f, 0.5f);
            } else {
                boxSize = new Vector2(0.5f, 1.25f);
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint, boxSize, 0f);
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
            KindlingBehavior kindlingBehavior = findNearbyUnlitKindling();
            if (kindlingBehavior != null) {
                kindling++;
                kindlingText.text = kindling.ToString();
                Destroy(kindlingBehavior.gameObject);
                return;
            }
            
            // Otherwise, if have kindling, drop it.
            if (kindling > 0) {
                dropKindling();
            }
        }
    }

    private void dropKindling() {
        Vector2 dropPoint = getPositionInFrontOfPlayer() + new Vector2(0,-0.5f * 0.75f); // Kindling is offsetted. gross.
        Collider2D[] colls = Physics2D.OverlapBoxAll(dropPoint, Vector2.one, 0);
        // TODO: Would be nice to provide feedback when unable to place, like a sound.
        if (colls == null || colls.Length == 0) {
            Instantiate(kindlingPrefab, dropPoint, Quaternion.identity);
            kindling--;
            kindlingText.text = kindling.ToString();
        }
    }

    private Vector2 getPositionInFrontOfPlayer() {
        Vector2 pos = transform.position;
        float distanceAhead = 0.75f;
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Up")) {
            pos += Vector2.up * distanceAhead + new Vector2(0, 0.2f); //This is a hack, let me go, please.
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Right")) {
            pos += Vector2.right * distanceAhead;
        } else if  (animator.GetCurrentAnimatorStateInfo(0).IsTag("Down")) {
            pos += Vector2.down * distanceAhead;
        } else {
            pos += Vector2.left * distanceAhead;
        }
        return pos;
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

    private KindlingBehavior findNearbyUnlitKindling() {
        // Find all nearby kindling.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        if (colliders == null || colliders.Length == 0) {
            return null;
        }

        // If a kindling is unlit, return that one.
        KindlingBehavior kindlingBehavior;
        foreach (Collider2D coll in colliders) {
            
            if (coll.gameObject.tag != "Kindling") {
                // This collider is not a Kindling, skip.
                continue;
            }

            kindlingBehavior = coll.GetComponent<KindlingBehavior>();
            if (!kindlingBehavior.lit) {
                return kindlingBehavior;
            }
        }
        return null;
    }

    private void controlAnimation() {
        Vector2 velocity = movementScript.getVelocity();
        animator.SetInteger("Horizontal", velocity.x == 0 ? 0 : (int)Mathf.Sign(velocity.x));
        animator.SetInteger("Vertical", velocity.y == 0 ? 0 : (int)Mathf.Sign(velocity.y));
    }
}
