
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
    private List<MagicTimer> timers = new List<MagicTimer>();

    [Client]
    public void CreateTimer(Magic magic) {
        if (!magic.useTimer)
            return;
        GameObject timerObj = Instantiate(magicTimerUIPrefab, magicTimerUIHolder.transform);
        MagicTimer timer = timerObj.GetComponent<MagicTimer>();
        timer.StartTimer(magic.time, magic);
        timer.EventOnTimerEnd += RemoveTimer;
        timers.Add(timer);
        LayoutRebuilder.ForceRebuildLayoutImmediate(magicTimerUIHolder.GetComponent<RectTransform>());
    }
    [Client]
    private void RemoveTimer(MagicTimer timer) {
        timer.EventOnTimerEnd -= RemoveTimer;
        timers.Remove(timer);
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

