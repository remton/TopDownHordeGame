
// This is the magic controller 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MagicController : NetworkBehaviour
{
    [SerializeField] private List<RandomChoice> dropList;

    public static MagicController instance;
    private void Awake()
    {
        HandleInstance();
    }

    [Server]
    public void MagicDrop(Vector3 zombieLocation)
    {   
        GameObject dropObj = Instantiate(RandomChoice.ChooseRandom(dropList), zombieLocation, Quaternion.identity);
        if (dropObj.CompareTag("NullChoice"))
            return;
        NetworkServer.Spawn(dropObj);
    }

    private void HandleInstance() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}

