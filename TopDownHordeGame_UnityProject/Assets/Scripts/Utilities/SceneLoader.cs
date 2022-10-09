//SceneLoader
//Author: Remington Ward
//This script organizes the order of events when loading into a scene.
//This script needs to exist in the scene being loaded

//There are multiple distinct points of loading:
//Server Load:  Called on the first frame in the scene on the server
//Clients Load: Called when all clients have loaded into the scene
//Players Load: Called after all player characters have spawned in
//Post Load:    Called after all other loads

//To subscribe to one of these events simply pass a function with void return value and no parameters to the relaent method
//You can specify the priority of the event which is used to determine the order that the actions are called (default to 0)
//By default the action givin will be forced to run immediatly if the relevant event has already been called
//This can be changed by passing forceRun parameter as false


using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : NetworkBehaviour
{
    //Instance Handleing
    public static SceneLoader instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    // ------------ PUBLIC ----------------

    //Server Loads first
    public void AddServerLoad(Action action, int priority=0, bool forceRun=true) {
        if(!serverLoaded)
            serverLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }

    //When all clients have loaded into the scene
    public void AddClientsLoad(Action action, int priority=0, bool forceRun=true) {
        if(!clientsLoaded)
            clientsLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }
    //When all the player characters have spawned in
    public void AddPlayerLoad(Action action, int priority=0, bool forceRun=true) {
        if (!playersLoaded)
            playerLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }
    //After all other loads have been called
    public void AddPostLoad(Action action, int priority=0, bool forceRun=true) {
        if(!postLoaded)
            postLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }



    // ------------ PRIVATE ----------------
    bool serverLoaded = false;
    bool clientsLoaded = false;
    bool playersLoaded = false;
    bool postLoaded = false;
    private List<PriorityAction> serverLoadActions = new List<PriorityAction>();
    private List<PriorityAction> clientsLoadActions = new List<PriorityAction>();
    private List<PriorityAction> playerLoadActions = new List<PriorityAction>();
    private List<PriorityAction> postLoadActions = new List<PriorityAction>();

    public override void OnStartServer() {
        base.OnStartServer();
        OnServerLoad();
        MyNetworkManager.instance.ServerEvent_AllClientsReady += OnClientLoad;
        if (MyNetworkManager.instance.AllClientsReady())
            OnClientLoad();
        serverLoaded = true;
    }

    [Server]
    private void OnServerLoad() {
        Debug.Log("SCENE LOADER: SERVER LOADED");
        serverLoadActions.Sort(PriorityAction.CompareByPriority);
        foreach (PriorityAction priorityAction in serverLoadActions) {
            try {
                priorityAction.action();
            }
            catch (Exception e) {
                Debug.LogError("Scene Loader OnServerLoad action call error: " + e.ToString());
            }
        }
        serverLoadActions.Clear();
        serverLoaded = true;
    }

    [Server]
    private void OnClientLoad() {
        MyNetworkManager.instance.ServerEvent_AllClientsReady -= OnClientLoad;
        OnClientLoadRPC();
        clientsLoaded = true;
        StartCoroutine(WaitForPlayerSpawns());
    }
    [ClientRpc]
    private void OnClientLoadRPC() {
        Debug.Log("SCENE LOADER: CLIENTS LOADED");
        clientsLoadActions.Sort(PriorityAction.CompareByPriority);
        foreach (PriorityAction priorityAction in clientsLoadActions) {
            try {
                priorityAction.action();
            }
            catch (Exception e) {
                Debug.LogError("Scene Loader OnClientsLoad action call error: " + e.ToString());
            }
        }
        clientsLoadActions.Clear();
        clientsLoaded = true;
    }

    [Server]
    private IEnumerator WaitForPlayerSpawns() {
        yield return new WaitUntil(MyNetworkManager.instance.AllPlayerCharactersSpawned);
        OnPlayersLoaded();
    }
    [Server]
    private void OnPlayersLoaded() {
        OnPlayersLoadedRPC();
        playersLoaded = true;
        OnPostLoad();
    }
    [ClientRpc]
    private void OnPlayersLoadedRPC() {
        Debug.Log("SCENE LOADER: PLAYERS LOADED");
        playerLoadActions.Sort(PriorityAction.CompareByPriority);
        foreach (PriorityAction priorityAction in playerLoadActions) {
            try {
                priorityAction.action();
            }
            catch (Exception e) {
                Debug.LogError("Scene Loader OnPlayersLoaded action call error: " + e.ToString());
            }
        }
        playerLoadActions.Clear();
        playersLoaded = true;
    }

    [Server]
    private void OnPostLoad() {
        OnPostLoadRPC();
        postLoaded = true;
    }
    [ClientRpc]
    private void OnPostLoadRPC() {
        Debug.Log("SCENE LOADER: POST LOAD");
        postLoadActions.Sort(PriorityAction.CompareByPriority);
        foreach (PriorityAction priorityAction in postLoadActions) {
            try {
                priorityAction.action();
            }
            catch (Exception e) {
                Debug.LogError("Scene Loader OnPostLoad action call error: " + e.ToString());
            }
        }
        postLoadActions.Clear();
        postLoaded = true;
    }


    struct PriorityAction {
        public Action action;
        public int priority;
        public static int CompareByPriority(PriorityAction a, PriorityAction b) {
            return a.priority - b.priority;
        }
        public PriorityAction(Action newAction, int newPriority) {
            action = newAction;
            priority = newPriority;
        }
    }
}
