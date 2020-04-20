using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public Vector2 facingDirection = Vector2.up;
    public float attackCooldown = 0.25f;
    public float searchRadius = 1f;
    public float maxFuelSeconds = 60f;
    public Slider fuelSlider;
    public Text kindlingText;
    public Text goldText;
    public GameObject kindlingPrefab;
    public Light2D torchLightFg;
    public Light2D torchLightBg;
    public Material material;
    public Light2D globalLight;
    public Text endText;
    
    private bool isUnlit = false;
    private float startingFgIntensity, startingBgIntensity, startingGIntensity;
    private float minimumMultiplier = 0.1f;
    private int gold = 0;
    private int kindling = 5;
    private float fuelLeftSeconds;
    private float currentCooldownTime = 0;
    private TopDownMovement movementScript;
    private Animator animator;
    private PlayerInputs playerInputs;
    private Color startingColor;
    private bool isDead = false;
    private SoundController soundController;
    private MusicController musicController;

    public void Start() {
        kindlingText.text = kindling.ToString();
        goldText.text = gold.ToString();
        fuelLeftSeconds = maxFuelSeconds;
        animator = GetComponent<Animator>();
        movementScript = GetComponent<TopDownMovement>();
        playerInputs = GetComponent<PlayerInputs>();
        startingBgIntensity = torchLightBg.intensity;
        startingFgIntensity = torchLightFg.intensity;
        startingGIntensity = globalLight.intensity;
        startingColor = new Color(4f, 2f, 0f, 1f);
        material.SetColor("_Color", startingColor);
        soundController = GameObject.FindGameObjectWithTag("Ladder").GetComponent<SoundController>();
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<MusicController>();
    }

    public void Update() {
        updateFuel();
        attack();
        action();
        refuel();
        controlAnimation();

        if (Input.GetButtonDown("Restart")) {
            SceneManager.LoadScene(0);
        }
    }

    private void win() {
        Destroy(playerInputs);
        GetComponent<SpriteRenderer>().color = Color.clear;
        fuelLeftSeconds = 0.01f;
        animator.SetTrigger("Unlit");
        musicController.musicPlayerDeath();
        Invoke("afterWin", 2f);
        GameObject[] bats = GameObject.FindGameObjectsWithTag("Bat");
        foreach (GameObject bat in bats) {
            Destroy(bat);
        }
        Destroy(GameObject.FindGameObjectWithTag("Ladder").GetComponent<BatSpawner>());
    }

    public void die() {
        if (!isDead) {
            soundController.playHitSound();
            musicController.musicPlayerDeath();
            isDead = true;
            Destroy(playerInputs);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            fuelLeftSeconds = 0.01f;
            transform.GetChild(2).localRotation = Quaternion.Euler(0,0,-90);
            animator.SetTrigger("Unlit");
            GameObject.FindGameObjectWithTag("Ladder").GetComponent<BatSpawner>().swarm();
            Invoke("afterDeath", 5f);
        }
    }

    private void afterWin() {
        endText.text = "You escaped with " + gold.ToString() + "\ngold bars! Press 'R'";
    }

    private void afterDeath() {
        GetComponent<SpriteRenderer>().color = Color.clear;
        endText.text = "You have perished.\nPress 'R'";
    }

    private void refuel() {
        if (playerInputs.refuel) {
            if (kindling > 0) {
                soundController.playlightSound();
                kindling--;
                kindlingText.text = kindling.ToString();
                fuelLeftSeconds = maxFuelSeconds;
                fuelSlider.value = Mathf.Clamp(fuelLeftSeconds/maxFuelSeconds, 0, 1);
            } else {
                soundController.playErrorSound();
            }
            
        }
    }

    private void attack() {
        // If not on cooldown and the player attacks, make a swoosh, stop moving.
        if (currentCooldownTime > 0) {
            currentCooldownTime -= Time.deltaTime;
        } else if (playerInputs.attacking) {
            // movementScript.enabled = false;
            currentCooldownTime = attackCooldown;
            animator.SetTrigger("Attack");
            soundController.playSwingSound();

            // Choose attack point.
            Vector2 attackPoint = getPositionInFrontOfPlayer();
            Vector2 boxSize;// = new Vector2(1.25f, 1.25f);
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Up")) {
                attackPoint.y += 0.15f;
                boxSize = new Vector2(4f, 2f);
            } else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Right")) {
                attackPoint.x += 0.25f;
                boxSize = new Vector2(2f, 4f);
            } else if  (animator.GetCurrentAnimatorStateInfo(0).IsTag("Down")) {
                attackPoint.y -= 0.35f;
                boxSize = new Vector2(4f, 2f);
            } else /* Left */ {
                attackPoint.x -= 0.25f;
                boxSize = new Vector2(2f, 4f);
            }

            Debug.DrawLine(attackPoint + new Vector2(boxSize.x/2, boxSize.y/2), attackPoint + new Vector2(-boxSize.x/2, boxSize.y/2), Color.green, 10f);
            Debug.DrawLine(attackPoint + new Vector2(-boxSize.x/2, boxSize.y/2), attackPoint + new Vector2(-boxSize.x/2, -boxSize.y/2), Color.green, 10f);
            Debug.DrawLine(attackPoint + new Vector2(-boxSize.x/2, -boxSize.y/2), attackPoint + new Vector2(boxSize.x/2, -boxSize.y/2), Color.green, 10f);
            Debug.DrawLine(attackPoint + new Vector2(boxSize.x/2, -boxSize.y/2), attackPoint + new Vector2(boxSize.x/2, boxSize.y/2), Color.green, 10f);

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint, boxSize, 0f);
            //Boy some object oriented programming would have been nice here.
            foreach (Collider2D hit in hits) {
                switch (hit.gameObject.tag) {
                    case "Kindling":
                        if (!isUnlit) {
                            soundController.playlightSound();
                            hit.GetComponent<KindlingBehavior>().Light();
                        } else {
                            if (hit.GetComponent<KindlingBehavior>().lit && fuelLeftSeconds > 0) {
                                soundController.playlightSound();
                                animator.SetTrigger("Lit");
                                musicController.musicStartGame();
                                globalLight.intensity = startingGIntensity;
                                isUnlit = false; 
                            }
                        }
                        break;
                    
                    case "Bat":
                        hit.GetComponent<BatBehavior>().getBurntYo();
                        break;
                }
            }
        }
    }

    private void action() {
        if (playerInputs.action) {
            // If near ladder and have enough gold, win.
            if (Vector2.Distance(GameObject.FindGameObjectWithTag("Ladder").transform.position, transform.position) <= searchRadius) {
                if (gold >= 100) {
                    win();
                } else {
                    soundController.playErrorSound();
                }
                return;
            }

            // Otherwise if near closed chest, open it.
            ChestBehavior chestBehavior = findNearbyClosedChest();
            if (chestBehavior != null) {
                chestBehavior.Open();
                return;
            }

            // Otherwise, if near kindling, pick it up.
            KindlingBehavior kindlingBehavior = findNearbyUnlitKindling();
            if (kindlingBehavior != null) {
                // if (kindling == 0) {
                //     torchLightFg.intensity = startingFgIntensity;
                //     torchLightBg.intensity = startingBgIntensity;
                //     globalLight.intensity = startingGIntensity;
                //     material.color = startingColor;
                // }
                soundController.playPickupSound();
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
        if (colls == null || colls.Length == 0) {
            Instantiate(kindlingPrefab, dropPoint, Quaternion.identity);
            kindling--;
            kindlingText.text = kindling.ToString();
            soundController.playDropSound();
        } else {
            soundController.playErrorSound();
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
        if (fuelLeftSeconds > 0 && !isUnlit) {
            fuelLeftSeconds -= Time.deltaTime;
            fuelSlider.value = Mathf.Clamp(fuelLeftSeconds/maxFuelSeconds, 0, 1);

            float multiplier = Mathf.Max(minimumMultiplier, fuelLeftSeconds/maxFuelSeconds);
            torchLightFg.intensity = startingFgIntensity * multiplier;
            torchLightBg.intensity = startingBgIntensity * multiplier;
            globalLight.intensity = startingGIntensity * multiplier;
            material.SetColor("_Color", new Color(startingColor.r * multiplier, startingColor.g * multiplier, startingColor.b * multiplier));

            if (fuelLeftSeconds <= 0) {
                if (kindling == 0 && !isDead) {
                    animator.SetTrigger("Unlit");
                    globalLight.intensity = 0;
                    isUnlit = true;
                    musicController.musicUnlit();
                } else {
                    // kindling--;
                    // kindlingText.text = kindling.ToString();
                    // fuelLeftSeconds = maxFuelSeconds;
                    // fuelSlider.value = Mathf.Clamp(fuelLeftSeconds/maxFuelSeconds, 0, 1);
                }
            }
        }

        // if (isUnlit && kindling > 0 && fuelLeftSeconds <= 0) {
        //     kindling--;
        //     kindlingText.text = kindling.ToString();
        //     fuelLeftSeconds = maxFuelSeconds;
        //     fuelSlider.value = Mathf.Clamp(fuelLeftSeconds/maxFuelSeconds, 0, 1);
        // }        
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
            if (chestBehavior != null && chestBehavior.closed) {
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

    public void addGold() {
        gold += 10;
        goldText.text = gold.ToString();
        musicController.musicSetDrone(gold/100f);
    }
}
