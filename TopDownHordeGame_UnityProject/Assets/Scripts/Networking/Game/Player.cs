using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Steamworks;

public class Player : NetworkBehaviour
{
    // --- Player connection and Input Setup --- 
    [SyncVar]
    private PlayerConnection myConnection;
    public PlayerConnection GetConnection() { return myConnection; }

    [ClientRpc]
    public void SetConnection(GameObject connectionObj) {
        Debug.LogWarning("Connection set:" + connectionObj.GetComponent<PlayerConnection>().netId);
        myConnection = connectionObj.GetComponent<PlayerConnection>();
        SetUpPlayerOnClient();
    }

    private void Start() {
        //If our connection is already set (Was spawned in on server) we still need to set up this player for this client
        if (myConnection != null) {
            SetUpPlayerOnClient();
            Debug.Log("Myconnection already set, LOCAL? " + myConnection.isLocalPlayer);
        }
    }

    [Client]
    private void SetUpPlayerOnClient() {
        GetComponent<PlayerInput>().enabled = myConnection.isLocalPlayer;
        GetComponent<PlayerInput>().camera = Camera.main;
        GetComponent<PlayerStats>().playerName = PlayerConnection.GetName(myConnection);
    }
}
