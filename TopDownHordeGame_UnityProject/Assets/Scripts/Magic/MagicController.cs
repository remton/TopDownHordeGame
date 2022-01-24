
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
    public bool selling = false;

    public List<Window> startRoomWindows;
    public List<GameObject> players;

    public int maxActiveZombies;

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
        int chance = Random.Range(0, 200); // 1 / 200 chance for a magic drop
        if (chance == 199)
        {
            chance = Random.Range(0, 1000); // The magics can be balanced to make some more likely than others 
            if (chance >= 800)
            {
                // SpawnAmmo
                Debug.Log("Ammo Drop");
                ammoObj = Instantiate(ammoPrefab, transform);
                ammoObj.transform.position  = zombieLocation;
            }
            else if (chance >= 600)
            {
                // SpawnCarpenter
                Debug.Log("Carpenter Drop");
                carpenterObj = Instantiate(carpenterPrefab, transform);
                carpenterObj.transform.position = zombieLocation;
            }
            else if (chance >= 400)
            {
                // SpawnKill
                Debug.Log("Kill Drop");
                killObj = Instantiate(killPrefab, transform);
                killObj.transform.position = zombieLocation;
            }
            else if (chance >= 200)
            {
                // SpawnNuke
                Debug.Log("Nuke Drop");
                nukeObj = Instantiate(nukePrefab, transform);
                nukeObj.transform.position = zombieLocation;
            }
            else if (chance >= 0 && !selling)
            {
                // SpawnSale
                Debug.Log("Sale Drop");
                saleObj = Instantiate(salePrefab, transform);
                saleObj.transform.position = zombieLocation;
            }
        }
    }
}

