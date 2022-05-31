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
    private new bool isLocalPlayer = false;

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
    }
    private void OnDestroy() {
        if(isServer)
            PlayerManager.instance.RemovePlayerCharacter(gameObject, connection);
    }

    public override void OnStartClient() {
        base.OnStartClient();
        SetUpPlayerOnClient();
    }

    [Client]
    private void SetUpPlayerOnClient() {
        isLocalPlayer = (connection == PlayerConnection.myConnection);

        GetComponent<PlayerInput>().enabled = isLocalPlayer;
        GetComponent<PlayerMovement>().enabled = isLocalPlayer;
        GetComponent<PlayerWeaponControl>().enabled = isLocalPlayer;
        GetComponent<PlayerActivate>().enabled = isLocalPlayer;
        GetComponent<PlayerInput>().camera = Camera.main;
        GetComponent<PlayerStats>().playerName = PlayerConnection.GetName(connection);

        //Server controlled
        GetComponent<PlayerHealth>().enabled = isServer;
    }

}
