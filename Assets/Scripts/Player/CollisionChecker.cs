using UnityEngine;

public class CollisionChecker : MonoBehaviour {

    /* This class is used to check for collisions in moving objects.
     * Note that this does not define how the object moves, but it merely 
     * takes a translation vector and returns a vector that has been collision
     * checked. It is unchanged if there is no collision, and it is modified
     * accordingly if there were collisions.
     */

    // Defines what the object can collide with.
    public LayerMask collisionMask;

    private BoxCollider2D boxCollider;
    private const float MARGIN = 0.01f;
    private const int RAYCAST_COUNT = 3;

    public void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null) {
            Debug.LogError(string.Format("There is no BoxCollider2D attached to game object {0} for the CollisionChecker to work.", gameObject.name));
            this.enabled = false;
        }
        if (collisionMask.value == 0) {
            Debug.LogWarning("There is no layers selected for the Collision Mask in the CollisionChecker.");
            this.enabled = false;
        }
    }

    /*
     * Takes in a moveVector that indicates how much the object wants to move,
     * and returns a vector indicating where the object will actually move to
     * given any collisions.
     */ 
    public Vector2 checkForCollisions(Vector2 moveVector) {

        bool collisionExists = false;

        // Horizontal collision check.
        if (moveVector.x != 0) {
            float direction = Mathf.Sign(moveVector.x);
            float distance = Mathf.Abs(moveVector.x) + MARGIN;
            Vector2 startOrigin = (Vector2)transform.position + boxCollider.offset + new Vector2((boxCollider.size.x/2 - MARGIN) * direction, -boxCollider.size.y/2 + MARGIN);
            Vector2 endOrigin = startOrigin + new Vector2(0, boxCollider.size.y - MARGIN*2);
            
            for (int i = 0; i < RAYCAST_COUNT; i++) {
                Vector2 raycastOrigin = Vector2.Lerp(startOrigin, endOrigin, (float)i / (RAYCAST_COUNT-1));
                RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, direction * Vector2.right, distance, collisionMask);

                if (hit.collider != null) {
                    moveVector.x = (hit.distance - MARGIN) * direction;
                    collisionExists = true;
                    break;
                }
            }
        }

        // Vertical collision check.
        if (moveVector.y != 0) {
            float direction = Mathf.Sign(moveVector.y);
            float distance = Mathf.Abs(moveVector.y) + MARGIN;
            Vector2 startOrigin = (Vector2)transform.position + boxCollider.offset + new Vector2(-boxCollider.size.x / 2 + MARGIN, (boxCollider.size.y / 2 - MARGIN) * direction);
            Vector2 endOrigin = startOrigin + new Vector2(boxCollider.size.x - MARGIN*2, 0);

            for (int i = 0; i < RAYCAST_COUNT; i++) {
                Vector2 raycastOrigin = Vector2.Lerp(startOrigin, endOrigin, (float)i / (RAYCAST_COUNT-1));
                RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, direction * Vector2.up, distance, collisionMask);

                if (hit.collider != null) {
                    moveVector.y = (hit.distance - MARGIN) * direction;
                    collisionExists = true;
                    break;
                }
            }
        }

        // If there's yet to be a collision and we are moving diagonally, it is possible we have hit the corner case. Pretend that the object already moved in the x-axis, and 
        // cast a y-axis ray from the closest corner of the object at this pretend position. If we hit something, moveVector.y changes to meet this. This causes the object
        // to prioritize x movement over y movement in the case of a corner, without any stuttering or clipping.
        if (moveVector.x != 0 && moveVector.y != 0 && !collisionExists) {
            float direction = Mathf.Sign(moveVector.y);
            float distance = Mathf.Abs(moveVector.y);
            Vector2 cornerPoint = (Vector2)transform.position + boxCollider.offset + new Vector2(Mathf.Sign(moveVector.x) * boxCollider.size.x/2 + moveVector.x, direction * boxCollider.size.y / 2);
            RaycastHit2D hit = Physics2D.Raycast(cornerPoint, direction * Vector2.up, distance, collisionMask);

            if (hit.collider != null) {
                moveVector.y = hit.distance * direction;
            }
        }

        return moveVector;
    }
}
