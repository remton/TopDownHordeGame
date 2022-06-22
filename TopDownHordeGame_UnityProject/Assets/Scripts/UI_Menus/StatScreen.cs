using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatScreen : NetworkBehaviour
{
    public static StatScreen instance;
    public StatScreen instanceDEBUG;
    public GameObject screen;
    public List<UIPlayerSidebar> statHolders;

    public readonly SyncList<PlayerStats> stats = new SyncList<PlayerStats>();

    //Only called by the server when scene is loaded. Clients do not run awake if the object was already in the scene when it loads
    private void Awake() {
        SetInstance();
    }
    private void SetInstance() {
        if (instance == null) {
            instance = this;
            instanceDEBUG = instance;
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();
        if (isServer) {
            if (MyNetworkManager.instance.AllClientsReady()) {
                OnAllClientsLoaded();
            }
            else {
                MyNetworkManager.instance.ServerEvent_AllClientsReady += OnAllClientsLoaded;
            }
        }
        else {
            CloseMenu();
        }
    }
    
    [Server]
    private void OnAllClientsLoaded() {
        MyNetworkManager.instance.ServerEvent_AllClientsReady -= OnAllClientsLoaded;
        StartCoroutine(SetPlayers());
    }
    IEnumerator SetPlayers() {
        if (!isServer)
            yield break;

        //wait unitl all charcters are spawned
        yield return new WaitUntil(MyNetworkManager.instance.AllPlayerCharactersSpawned);
        //Setplayers
        stats.Clear();
        List<PlayerConnection> conns = MyNetworkManager.instance.GetPlayerConnections();
        for (int connIndex = 0; connIndex < conns.Count; connIndex++) {
            List<GameObject> charas = conns[connIndex].GetPlayerCharacters();
            for (int charaIndex = 0; charaIndex < charas.Count; charaIndex++) {
                stats.Add(charas[charaIndex].GetComponent<PlayerStats>());
            }
        }
        instance.UpdateStats();
        CloseMenu();
    }

    private void Update() {
        if (screen.activeSelf) {
            UpdateStats();
        }
    }
    public void UpdateStats() {
        foreach (var statHolder in statHolders) {
            statHolder.Activate(false);
        }

        int maxIndex = Mathf.Min(stats.Count, statHolders.Count);
        for (int i = 0; i < maxIndex; i++) {
            statHolders[i].Activate(true);
            statHolders[i].UpdateBankTxt(stats[i].GetBank());
            statHolders[i].SetPlayerName(stats[i].GetName());
        }
    }

    public static void OpenMenu() {
        if (instance == null) {
            Debug.LogWarning("Cannot open stat screen. Instance is null");
            return;
        }
        instance.UpdateStats();
        instance.screen.SetActive(true);
    }

    public static void CloseMenu() {
        if (instance == null)
            return;
        instance.screen.SetActive(false);
    }
}
