
// This is the magic controller 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MagicController : NetworkBehaviour
{
    public static MagicController instance;

    [SerializeField] private GameObject magicTimerUIHolder;
    [SerializeField] private GameObject magicTimerUIPrefab;
    [SerializeField] private List<RandomChoice> dropList;

    [Client]
    public void CreateTimer(Magic magic, System.Guid timerID) {
        GameObject timerObj = Instantiate(magicTimerUIPrefab, magicTimerUIHolder.transform);
        MagicTimer timer = timerObj.GetComponent<MagicTimer>();
        timer.StartTimer(magic, timerID);
        LayoutRebuilder.ForceRebuildLayoutImmediate(magicTimerUIHolder.GetComponent<RectTransform>());
    }

    [Client]
    public void RemoveTimer(MagicTimer timer) {
        Destroy(timer.gameObject);
    }

    [Server]
    public void MagicDrop(Vector3 zombieLocation){   
        GameObject dropObj = Instantiate(RandomChoice.ChooseRandom(dropList), zombieLocation, Quaternion.identity);
        if (dropObj.CompareTag("NullChoice"))
            return;
        NetworkServer.Spawn(dropObj);
    }

    private void Awake(){
        HandleInstance();
    }
    private void HandleInstance() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}

