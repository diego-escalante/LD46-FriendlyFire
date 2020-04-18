using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public Vector2 facingDirection = Vector2.up;
    public GameObject FireSwooshPrefab;
    public float attackCooldown = 0.25f;
    public float searchRadius = 1f;
    
    private float currentCooldownTime = 0;
    private TopDownMovement movementScript;
    private Animator animator;
    private PlayerInputs playerInputs;

    public void Start() {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<TopDownMovement>();
        playerInputs = GetComponent<PlayerInputs>();
        if (FireSwooshPrefab == null) {
            Debug.LogError("FireSwooshPrefab has not been picked in the PlayerManager!");
        }
    }

    public void Update() {
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
