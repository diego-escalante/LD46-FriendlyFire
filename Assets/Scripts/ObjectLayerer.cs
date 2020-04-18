using UnityEngine;

// This basically set the Z position of an object to Y to created a layering effect.

public class ObjectLayerer : MonoBehaviour {

    public bool updating = false;

    void Start() {
        updatePosition();
    }

    void Update() {
        if (updating) {
            updatePosition();
        }
    }

    private void updatePosition() {
        Vector3 position = transform.position;
        position.z = position.y;
        transform.position = position;
    }
}
