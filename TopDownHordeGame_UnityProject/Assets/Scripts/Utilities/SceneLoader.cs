using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

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

    //Server Load
    public void SubServerLoad(Action action, int priority) {
        serverLoadActions.Add(new PriorityAction(action, priority));
    }
    public void UnSubServerLoad(Action action) {
        for (int i = 0; i < serverLoadActions.Count; i++) {
            if (serverLoadActions[i].action == action)
                serverLoadActions.RemoveAt(i);
        }
    }

    //ClientsLoad
    public void SubClientsLoad(Action action, int priority) {
        serverLoadActions.Add(new PriorityAction(action, priority));
    }
    public void UnSubClientsLoad(Action action) {
        for (int i = 0; i < clientsLoadActions.Count; i++) {
            if (clientsLoadActions[i].action == action)
                clientsLoadActions.RemoveAt(i);
        }
    }

    //PostLoad
    public void SubPostLoad(Action action, int priority) {
        postLoadActions.Add(new PriorityAction(action, priority));
    }
    public void UnSubPostLoad(Action action) {
        for (int i = 0; i < postLoadActions.Count; i++) {
            if (postLoadActions[i].action == action)
                postLoadActions.RemoveAt(i);
        }
    }

    // ------------ PRIVATE ----------------

    private List<PriorityAction> serverLoadActions = new List<PriorityAction>();
    private List<PriorityAction> clientsLoadActions = new List<PriorityAction>();
    private List<PriorityAction> postLoadActions = new List<PriorityAction>();

    public override void OnStartServer() {
        base.OnStartServer();
        OnServerLoad();
        MyNetworkManager.instance.ServerEvent_AllClientsReady += OnClientLoad;
        if (MyNetworkManager.instance.AllClientsReady())
            OnClientLoad();
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
    }

    private void OnClientLoad() {
        OnClientLoadRPC();
        OnPostLoad();
    }
    [ClientRpc]
    private void OnClientLoadRPC() {
        Debug.Log("SCENE LOADER: CLIENTS LOAD");
        MyNetworkManager.instance.ServerEvent_AllClientsReady -= OnClientLoad;
        clientsLoadActions.Sort(PriorityAction.CompareByPriority);
        foreach (PriorityAction priorityAction in clientsLoadActions) {
            try {
                priorityAction.action();
            }
            catch (Exception e) {
                Debug.LogError("Scene Loader OnClietnLoad action call error: " + e.ToString());
            }
        }
    }
    private void OnPostLoad() {
        OnPostLoadRPC();
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
