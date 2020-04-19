using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSpawner : MonoBehaviour {

    public GameObject batPrefab;
    public float intervalBetweenSpawns = 15f;
    public float decreasingStep = 0.5f;

    private float spawnMoment;
    private float countdown;

    // private bool spawnSwarm = false;


    void Start(){
        spawnMoment = intervalBetweenSpawns * 2;
        countdown = intervalBetweenSpawns;
    }

    void Update() {
        countdown -= Time.deltaTime;

        if (spawnMoment <= countdown) {
            Instantiate(batPrefab, calculateSpawnLocation(), Quaternion.identity);
            spawnMoment = Mathf.Infinity;
        }

        if (countdown <= 0) {
            intervalBetweenSpawns -= decreasingStep;
            spawnMoment = Random.Range(0f, intervalBetweenSpawns);
            countdown = intervalBetweenSpawns;
        }

    }

     public void swarm() {
        // spawnSwarm = true;
        intervalBetweenSpawns = 0.1f;
        countdown = intervalBetweenSpawns;
        spawnMoment = 0;
        decreasingStep = 0;
        Destroy(this, 2f);
    }

    private Vector2 calculateSpawnLocation() {
        if (Random.Range(0,2) == 0) {
            return Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0f,1f),Random.Range(0, 2) == 0 ? 1.05f : -0.05f,0));
        }
        return Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0, 2) == 0 ? 1.05f : -0.05f, Random.Range(0f,1f),0));
    }
}
