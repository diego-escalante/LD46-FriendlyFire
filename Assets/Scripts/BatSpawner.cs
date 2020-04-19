using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSpawner : MonoBehaviour {

    public GameObject batPrefab;
    public float intervalBetweenSpawns = 15f;
    public float decreasingStep = 0.5f;

    private float spawnMoment;
    private float countdown;


    void Start(){
        spawnMoment = intervalBetweenSpawns * 2;
        countdown = intervalBetweenSpawns;
    }

    void Update() {
        countdown -= Time.deltaTime;

        if (spawnMoment <= countdown) {
            Instantiate(batPrefab, calculateSpawnLocation(), Quaternion.identity);
            spawnMoment = Mathf.Infinity;
            Debug.Log("Spawned!");
        }

        if (countdown <= 0) {
            intervalBetweenSpawns -= decreasingStep;
            spawnMoment = Random.Range(0f, intervalBetweenSpawns);
            countdown = intervalBetweenSpawns;
        }
    }

    private Vector2 calculateSpawnLocation() {
        return Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0f,1f),Mathf.Sign(Random.Range(-1, 1)) * 1.2f,0));
    }
}
