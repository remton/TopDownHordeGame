
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
    private GameObject carpenterObj;
    private GameObject killObj;
    private GameObject nukeObj;
    private GameObject saleObj;

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
    public void MagicDrop(Vector3 zombieLocation)
    {
        int chance = Random.Range(0, 1000);
        if (chance > 999)
        {
            // SpawnAmmo
            Debug.Log("Ammo Drop");
            ammoObj = Instantiate(ammoPrefab, transform);
            ammoObj.transform.position = zombieLocation;
        }
        else if (chance >998)
        {
            // SpawnCarpenter
            Debug.Log("Carpenter Drop");
            carpenterObj = Instantiate(carpenterPrefab, transform);
            carpenterObj.transform.position = zombieLocation;
        }
        else if (chance > 997)
        {
            // SpawnKill
            Debug.Log("Kill Drop");
            carpenterObj = Instantiate(killPrefab, transform);
            carpenterObj.transform.position = zombieLocation;
        }
        else if (chance > 996)
        {
            Debug.Log("Nuke Drop");
            carpenterObj = Instantiate(nukePrefab, transform);
            carpenterObj.transform.position = zombieLocation;
        }
        else if (chance > 1000)
        {
            Debug.Log("Sale Drop");
            carpenterObj = Instantiate(salePrefab, transform);
            carpenterObj.transform.position = zombieLocation;
        }

    }
}

