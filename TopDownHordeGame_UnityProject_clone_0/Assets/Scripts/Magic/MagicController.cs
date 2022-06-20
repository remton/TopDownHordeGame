
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
    [SerializeField] private List<RandomChoice> dropList;
    private GameObject dropObj;
    public bool selling = false;
    public List<Window> startRoomWindows;
    public List<GameObject> players;

    public int maxActiveZombies;

    public static MagicController instance;

    private void Awake()
    {
        //Initial round values
        //Debug.Log("Magic controller is awake.");

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

     }
    public void MagicDrop(Vector3 zombieLocation)
    {   
        dropObj = Instantiate(RandomChoice.ChooseRandom(dropList), transform);
        dropObj.transform.position = zombieLocation;
    }
}

