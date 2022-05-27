using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Steamworks;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    //The playerconnection for a given client (set in first Init call)
    public static PlayerConnection myConnection;

    [SerializeField]
    private GameObject playerPrefab;

    [SyncVar]
    private GameObject playerCharacter;
    public GameObject GetPlayerCharacter() { return playerCharacter; }

    public static string GetName(PlayerConnection connection) {
        ConnectionData data = connection.connectionData;
        if (data.useSteam)
            return SteamFriends.GetFriendPersonaName(new CSteamID(ulong.Parse(data.steamID)));
        else
            return "Con [" + connection.netId + "]";
    }

    [ClientRpc(includeOwner = true)]
    public void Init(ConnectionData data) {
        connectionData = data;
        Debug.Log(GetName(this) + "'s PlayerConnection Initialized");
        DontDestroyOnLoad(gameObject);
        if(myConnection == null) {
            Debug.Log("My netID: " + netId);
            myConnection = this;
        }
    }

    //Leaves the steam lobby for all clients
    [ClientRpc(includeOwner = true)]
    public void CloseSteamLobby(CSteamID lobbyID) {
        Debug.Log(SteamFriends.GetPersonaName() + " Left Lobby");
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    [Client]
    public void SpawnPlayer(Vector3 location) {
        SpawnPlayerCommand(location, gameObject);
    }

    [Command]
    private void SpawnPlayerCommand(Vector3 location, GameObject connObj) {
        PlayerConnection conn = connObj.GetComponent<PlayerConnection>();
        GameObject character = Instantiate(conn.playerPrefab, location, Quaternion.identity);
        character.GetComponent<Player>().SetConnection(connObj);
        NetworkServer.Spawn(character, connObj);
        playerCharacter = character;
        PlayerManager.instance.AddLocalPlayerCharacter(conn.GetComponent<PlayerConnection>().connectionToClient, character);
        PlayerManager.instance.AddPlayerCharacter(character);
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy() {
        MyNetworkManager.instance.OnPlayerConnectionDestroyed(this);
    }

    public struct ConnectionData {
        public uint netID;
        public bool useSteam;
        public string steamID;
        public string address;
    }
    public ConnectionData connectionData;

}
