using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public Vector2 facingDirection = Vector2.up;
    public GameObject FireSwooshPrefab;
    public float attackCooldown = 0.25f;
    
    private float currentCooldownTime = 0;
    private TopDownMovement movementScript;

    public void Start() {
        movementScript = GetComponent<TopDownMovement>();
        if (FireSwooshPrefab == null) {
            Debug.LogError("FireSwooshPrefab has not been picked in the PlayerManager!");
        }
    }

    public void Update() {
        updateFacingDirection();
        attack();
    }

    private void attack() {
        // If we are on cooldown, don't do anything but check if we get off of it.
        if (currentCooldownTime > 0) {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0) {
                movementScript.enabled = true;
            }
            return;
        }

        // If not on cooldown and the player attacks, make a swoosh, stop moving.
        if (Input.GetButtonDown("Fire")) {
            Instantiate(FireSwooshPrefab, transform.position + (1.5f * transform.right), facingDirectionToQuaternion(facingDirection));
            movementScript.enabled = false;
            currentCooldownTime = attackCooldown;
        }
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
            
            // Currently just rotate the sprite into the 4 cardinal directions by changing its Right direction.
            // If the actual sprites are not 100% top down (aka, need more than one drawing), this will need to be smarter.
            transform.right = facingDirection;
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
}
