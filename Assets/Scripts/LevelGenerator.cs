using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public GameObject kindlingPrefab;
    public GameObject chestPrefab;
    public int totalKindling = 100;
    public int totalChests = 100;

    void Start() {
        // Chests.
        for (int i = 0; i < totalChests; i++) {
            Vector2 randomPosition = new Vector2 (Random.Range(0,90) - Random.Range(0, 90), Random.Range(0,90) - Random.Range(0, 90));
            if (!isPositionFreeOfNeighbors(randomPosition)) {
                i--;
                continue;
            }
            Instantiate(chestPrefab, randomPosition, Quaternion.identity);
        }

        // Kindling.
        for (int i = 0; i < totalKindling; i++) {
            Vector2 randomPosition = new Vector2 (Random.Range(0,90) - Random.Range(0, 90), Random.Range(0,90) - Random.Range(0, 90));
            if (!isPositionFreeOfNeighbors(randomPosition)) {
                i--;
                continue;
            }
            Instantiate(kindlingPrefab, randomPosition, Quaternion.identity);
        }
    }

    private bool isPositionFreeOfNeighbors(Vector2 pos) {
        return Physics2D.OverlapCircle(pos, 3f) == null;
    }
}
