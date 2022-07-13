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
    }

    public override void OnStartServer() {
        base.OnStartServer();
        playerID = new System.Guid();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        SetUpPlayerOnClient();
    }

    [Client]
    private void SetUpPlayerOnClient() {
        bool isLocalCharacter = (connection == PlayerConnection.myConnection);
        if(isLocalCharacter)
            connection.PlayerSpawnConfirm();

        GetComponent<PlayerInput>().enabled = isLocalCharacter;
        GetComponent<PlayerMovement>().enabled = isLocalCharacter;
        GetComponent<PlayerWeaponControl>().enabled = isLocalCharacter;
        GetComponent<PlayerActivate>().enabled = isLocalCharacter;
        GetComponent<PlayerInput>().camera = Camera.main;
        GetComponent<PlayerStats>().SetName(PlayerConnection.GetName(connection, gameObject));

        //Server controlled
        GetComponent<PlayerHealth>().enabled = isServer;
    }

    public delegate void ControlsChanged(string controlScheme);
    public event ControlsChanged EventControlsChanged;
    public void OnControlsChange() {
        string scheme = GetComponent<PlayerInput>().currentControlScheme;
        if (EventControlsChanged != null) { EventControlsChanged.Invoke(scheme); }
    }

    //We use this to disable input to prevent the edge case where input is disabled in the callback called by the input itself
    IEnumerator SetInputActive(bool active) {
        yield return new WaitForEndOfFrame();
        if (active)
            GetComponent<PlayerInput>().ActivateInput();
        else
            GetComponent<PlayerInput>().DeactivateInput();
    }
    //Enables the player to move, shoot etc.
    public void EnablePlayer() {
        StartCoroutine(SetInputActive(true));
    }
    //Stops the player from moving, shooting etc.
    public void DisablePlayer() {
        StartCoroutine(SetInputActive(false));
    }
}
