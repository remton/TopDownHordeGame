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
    private PlayerConnection connection;
    public bool IsLocalCharacter() {
        return connection == PlayerConnection.myConnection;
    }


    [SyncVar]
    private System.Guid playerID;
    public System.Guid GetPlayerID() { return playerID; }

    public PlayerConnection GetConnection() { return connection; }

    public void SetConnection(PlayerConnection connection) {
        this.connection = connection;
    }

    private void Awake() {
        connection = PlayerConnection.myConnection;
        SetUpPlayerOnClient();
    }

    public override void OnStartServer() {
        base.OnStartServer();
        playerID = new System.Guid();
        StatScreen.AddSidebar(gameObject);
    }
    private void OnDestroy() {
        if(isServer)
            PlayerManager.instance.RemovePlayerCharacter(gameObject, connection);
    }

    public override void OnStartClient() {
        base.OnStartClient();
        SetUpPlayerOnClient();
        Debug.Log("CLIENT START START player netID:" + netId);
    }

    [Client]
    private void SetUpPlayerOnClient() {
        bool isLocalCharacter = (connection == PlayerConnection.myConnection);

        GetComponent<PlayerInput>().enabled = isLocalCharacter;
        GetComponent<PlayerMovement>().enabled = isLocalCharacter;
        GetComponent<PlayerWeaponControl>().enabled = isLocalCharacter;
        GetComponent<PlayerActivate>().enabled = isLocalCharacter;
        GetComponent<PlayerInput>().camera = Camera.main;
        GetComponent<PlayerStats>().playerName = PlayerConnection.GetName(connection, gameObject);

        //Server controlled
        GetComponent<PlayerHealth>().enabled = isServer;
    }

}
