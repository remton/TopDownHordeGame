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

    [SerializeField]
    private NetworkTransformChild weaponSpriteNetTransform;
    public void SetWeaponForNetworkSync(GameObject weaponObj) {
        weaponSpriteNetTransform.target = weaponObj.GetComponent<Weapon>().transform;
    }

    public void SetConnection(GameObject connectionObj) {
        Debug.LogWarning("Connection set:" + connectionObj.GetComponent<PlayerConnection>().netId);
        myConnection = connectionObj.GetComponent<PlayerConnection>();
    }

    private void Start() {
        //If our connection is already set (Was spawned in on server) we still need to set up this player for this client
        SetUpPlayerOnClient();
    }

    [Client]
    private void SetUpPlayerOnClient() {
        //Set up things handled on the local client side
        GetComponent<PlayerInput>().enabled = myConnection.isLocalPlayer;
        GetComponent<PlayerMovement>().enabled = myConnection.isLocalPlayer;
        GetComponent<PlayerWeaponControl>().enabled = myConnection.isLocalPlayer;
        GetComponent<PlayerActivate>().enabled = myConnection.isLocalPlayer;
        GetComponent<PlayerHealth>().enabled = isLocalPlayer;
        GetComponent<PlayerInput>().camera = Camera.main;
        GetComponent<PlayerStats>().playerName = PlayerConnection.GetName(myConnection);

        //Set up things handled on the server side
    }
}
