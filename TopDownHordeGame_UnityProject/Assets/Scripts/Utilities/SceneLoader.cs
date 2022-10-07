//SceneLoader
//Author: Remington Ward
//This script organizes the order of events when loading into a scene.
//This script needs to exist in the scene being loaded

//There are 3 distinct points of loading: Server loads, all clients load, and a post load

//To subscribe to one of these events simply pass a function with void return value and no parameters to the relaent method
//You can specify the priority of the event which is used to determin the order that the actions are called (default is 0)
//By default the action givin will be forced to run immediatly if the relevant event has already been called


using Mirror;
using System;
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
    public void AddServerLoad(Action action, int priority = 0, bool forceRun=true) {
        if(!serverLoaded)
            serverLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }

    //When all clients have loaded into the scene
    public void AddClientsLoad(Action action, int priority = 0, bool forceRun=true) {
        if(!clientsLoaded)
            clientsLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }

    //After all clients load has been called
    public void AddPostLoad(Action action, int priority = 0, bool forceRun=true) {
        if(!postLoaded)
            postLoadActions.Add(new PriorityAction(action, priority));
        else if (forceRun) {
            action();
        }
    }

    // ------------ PRIVATE ----------------
    bool serverLoaded = false;
    bool clientsLoaded = false;
    bool postLoaded = false;
    private List<PriorityAction> serverLoadActions = new List<PriorityAction>();
    private List<PriorityAction> clientsLoadActions = new List<PriorityAction>();
    private List<PriorityAction> postLoadActions = new List<PriorityAction>();

    public override void OnStartServer() {
        base.OnStartServer();
        OnServerLoad();
        MyNetworkManager.instance.ServerEvent_AllClientsReady += OnClientLoad;
        if (MyNetworkManager.instance.AllClientsReady())
            OnClientLoad();
        serverLoaded = true;
    }


    private void OnServerLoad() {
        Debug.Log("SCENE LOADER: SERVER LOAD");
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

    private void OnClientLoad() {
        MyNetworkManager.instance.ServerEvent_AllClientsReady -= OnClientLoad;
        OnClientLoadRPC();
        OnPostLoad();
        clientsLoaded = true;
    }
    [ClientRpc]
    private void OnClientLoadRPC() {
        Debug.Log("SCENE LOADER: CLIENTS LOAD");
        clientsLoadActions.Sort(PriorityAction.CompareByPriority);
        foreach (PriorityAction priorityAction in clientsLoadActions) {
            try {
                priorityAction.action();
            }
            catch (Exception e) {
                Debug.LogError("Scene Loader OnClietnLoad action call error: " + e.ToString());
            }
        }
        clientsLoadActions.Clear();
        clientsLoaded = true;
    }
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
