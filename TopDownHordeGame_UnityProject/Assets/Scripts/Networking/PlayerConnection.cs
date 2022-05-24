using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Steamworks;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    public struct ConnectionData {
        public bool useSteam;
        public CSteamID steamID;
        public NetworkConnectionToClient connection;
    }
    public ConnectionData connectionData;

    [SerializeField]
    private GameObject playerPrefab;

    private GameObject player;

    public void Init(ConnectionData data) {
        connectionData = data;
        if(data.useSteam)
            Debug.Log(SteamFriends.GetFriendPersonaName(data.steamID) + "'s PlayerConnection Initialized");
        else
            Debug.Log("Con[" + data.connection.ToString() + "] PlayerConnection initialized");
        DontDestroyOnLoad(gameObject);
    }

    //Leaves the steam lobby (not game but the lobby) for this client
    [ClientRpc(includeOwner = true)]
    public void LeaveLobby(CSteamID lobbyID) {
        Debug.Log(SteamFriends.GetPersonaName() + " Left Lobby");
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    [Command]
    public void SpawnPlayer(Vector3 location) {
        Destroy(player);
        if (playerPrefab == null) {
            Debug.Log("PlayerConnection is missing its player prefab!!");
            return;
        }

        player = Instantiate(playerPrefab, location, Quaternion.identity);
        NetworkServer.Spawn(player, connectionData.connection);
        //player.GetComponent<Player>().SetPlayerConnection(gameObject);
    }
    
    private void Start() {
    }
}
