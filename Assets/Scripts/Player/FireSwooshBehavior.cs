using UnityEngine;

public class FireSwooshBehavior : MonoBehaviour {

    void Start() {
        Destroy(this.gameObject, 0.25f);
    }
}
