
    // This is the magic controller 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private GameObject carpenterPrefab;
    [SerializeField] private GameObject killPrefab;
    [SerializeField] private GameObject nukePrefab;
    [SerializeField] private GameObject salePrefab;

    private GameObject ammoObj;

    [SerializeField] private int round;

    public List<Window> startRoomWindows;
    public List<GameObject> players;

    private float spawnDelay;
    private float timeUntilNextSpawn;

    int zombiesToSpawn;
    public int maxActiveZombies;
    float speed;
    int health;
    int damage;

    public static MagicController instance;

    private void Awake()
    {
        //Initial round values
        Debug.Log("Magic controller is awake.");

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

     }

    private void Update()
    {


        //Manages round changing
        /*
                if (isWaitingForNextRound)
                {
                    if (timeUntilRoundStart <= Time.deltaTime)
                    {
                        isWaitingForNextRound = false;
                        NextRound();
                    }
                    timeUntilRoundStart -= Time.deltaTime;
                }
                else if (zombiesSpawnedThisRound >= zombiesToSpawn && numberActiveZombies <= 0)
                {
                    isWaitingForNextRound = true;
                    timeUntilRoundStart = pauseBetweenRounds;
                }
        */

        //Zombie Spawning
        /*        if (activeWindows.Count != 0)
                {
                    if (!isWaitingForNextRound)
                    {
                        if (timeUntilNextSpawn <= 0)
                        {
                            if (zombiesSpawnedThisRound < zombiesToSpawn && numberActiveZombies < maxActiveZombies)
                            {
                                int i = Mathf.RoundToInt(Random.Range(0, activeWindows.Count));
                                activeWindows[i].AddZombiesToQueue(1); // the window handles spawning the zombie
                                zombiesSpawnedThisRound++;
                                numberActiveZombies++;
                            }
                            timeUntilNextSpawn = spawnDelay;
                        }
                        else
                        {
                            timeUntilNextSpawn -= Time.deltaTime;
                        }
                    }
                }
                else
                {
                    Debug.Log("NO ACTIVE WINDOWS!!!! -_-");
                }
            }
        */
    }

/*    public GameObject CreateMagic()
    {
//    GameObject zombieObj = Instantiate(zombiePrefab);
//      ZombieAI zombie = zombieObj.GetComponent<ZombieAI>();
//        zombie.SetValues(GetHealth(), GetSpeed(), GetDamage());
//        return zombieObj;
    } */

    public void MagicDrop(Vector3 zombieLocation)
    {
        int chance = Random.Range(0, 1000);
        if (chance > 0)
        {
            // SpawnAmmo
            Debug.Log("Ammo Drop");
            ammoObj = Instantiate(ammoPrefab, transform);
            ammoObj.transform.position = zombieLocation;

        }
        else if (chance > 880)
        {
            // SpawnCarpenter

        }
        else if (chance > 750)
        {
            // SpawnKill
            GameObject obj = Instantiate(killPrefab);
            obj.transform.position = zombieLocation;
        }
        else if (chance > 730)
        {
            // SpawnNuke // FUNCTIONS NOT IMPLEMENTED
            GameObject obj = Instantiate(nukePrefab);
            obj.transform.position = zombieLocation;
        }
        else if (chance > 600)
        {
            // SpawnSale 
            GameObject obj = Instantiate(salePrefab);
            obj.transform.position = zombieLocation;
        }

    }
}

