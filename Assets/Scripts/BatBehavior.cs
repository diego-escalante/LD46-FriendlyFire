using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehavior : MonoBehaviour {

    public float speed;
    private GameObject player;
    private PlayerManager playerManager;
    private Collider2D playerColl, coll;
    private bool isFleeing = false;
    private float startingSpeed;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerColl = player.GetComponent<Collider2D>();
        playerManager = player.GetComponent<PlayerManager>();
        coll = GetComponent<Collider2D>();
        startingSpeed = speed;
    }

    void Update() {
        speed += 0.2f * Time.deltaTime;
        float step = (isFleeing ? speed * 1.5f : speed) * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, isFleeing ? -step : step);

        if (!isFleeing && Vector2.Distance(transform.position, player.transform.position) <= 0.33f) {
            playerManager.die();
            speed = 0;
            Invoke("flee", 3f);
        }

        if ((Vector2)transform.position == (Vector2)player.transform.position) {
            transform.Translate(new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0));
        }
    }

    public void getBurntYo() {
        if (!isFleeing) {
            isFleeing = true;
            GetComponent<Animator>().SetTrigger("Scared");
        }
    }

    public void OnBecameInvisible() {
        if (isFleeing) {
            Destroy(this.gameObject);
        }
    }

    private void flee() {
        speed = startingSpeed;
        isFleeing = true;
    }
}
