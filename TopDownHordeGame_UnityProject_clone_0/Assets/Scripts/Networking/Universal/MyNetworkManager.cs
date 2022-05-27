using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance;

    [Header("Transport set by script!!")]
    public bool useSteam;
    [SerializeField] protected Transport steamTransport;
    [SerializeField] protected SteamLobby steamLobby;
    [SerializeField] protected SteamManager steamManager;
    [SerializeField] protected Transport kcpTransport;

    private List<PlayerConnection> playerConnections = new List<PlayerConnection>();
    public List<PlayerConnection> GetPlayerConnections() { return playerConnections; }

    //--- Events for various things ---
    public delegate void Server_PlayerConnectionAdded(PlayerConnection pConnection);
    public event Server_PlayerConnectionAdded ServerEvent_PlayerConnectionAdded;

    public delegate void Server_PlayerConnectionRemoved(PlayerConnection pConnection);
    public event Server_PlayerConnectionRemoved ServerEvent_PlayerConnectionRemoved;


    //--- Public Methods ---
    public void HostGame(string code) {
        if (useSteam) {
            steamLobby.GetLobbies(code);
            steamLobby.HostLobby();
        }
        else {
            StartHost();
        }
    }
    public void JoinGame(string code) {
        if (useSteam) {
            steamLobby.GetLobbies(code);
            steamLobby.JoinLobby();
        }
        else {
            StartClient();
        }
    }
    public void ChangeScene(string sceneName) {
        ServerChangeScene(sceneName);
    }

    //--- Handle SteamLobby Events ---
    private void OnSteamLobbyJoinGame(SteamLobby.JoinLobbyData data) {
        Debug.Log("Connected User ID:" + SteamFriends.GetFriendPersonaName(data.userSteamID));
    }
    private void OnSteamLobbyCreateGame(SteamLobby.CreateLobbyData data) {
        Debug.Log("Hosting Lobby ID:" + data.lobbySteamID);
    }


    //--- Handle PlayerConnection components ---
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        // Normal Networkmanager 
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        //Addition for MyNetworkManager
        PlayerConnection.ConnectionData connectionData;
        if (useSteam) {
            connectionData.useSteam = true;
            if(conn.address == "localhost") {
                connectionData.steamID = SteamUser.GetSteamID().ToString();
            }
            else {
                connectionData.steamID = conn.address;
            }
        }
        else {
            connectionData.useSteam = false;
            connectionData.steamID = "0";
        }
        connectionData.address = conn.address;
        connectionData.netID = player.GetComponent<PlayerConnection>().netId;
        player.GetComponent<PlayerConnection>().Init(connectionData);
        playerConnections.Add(player.GetComponent<PlayerConnection>());
        if(ServerEvent_PlayerConnectionAdded != null) { ServerEvent_PlayerConnectionAdded.Invoke(player.GetComponent<PlayerConnection>()); }
    }
    public void OnPlayerConnectionDestroyed(PlayerConnection connection) {
        if (playerConnections.Remove(connection)) {
            if (ServerEvent_PlayerConnectionRemoved != null) { ServerEvent_PlayerConnectionRemoved.Invoke(connection); }
        }
    }

    public override void OnValidate() {
        // always >= 0
        maxConnections = Mathf.Max(maxConnections, 0);

        if (playerPrefab != null && (playerPrefab.GetComponent<NetworkIdentity>() == null || playerPrefab.GetComponent<PlayerConnection>() == null)) {
            Debug.LogError("NetworkManager - Player Prefab must have a Playerconnection and a NetworkIdentity.");
            playerPrefab = null;
        }

        // This avoids the mysterious "Replacing existing prefab with assetId ... Old prefab 'Player', New prefab 'Player'" warning.
        if (playerPrefab != null && spawnPrefabs.Contains(playerPrefab)) {
            Debug.LogWarning("NetworkManager - Player Prefab should not be added to Registered Spawnable Prefabs list...removed it.");
            spawnPrefabs.Remove(playerPrefab);
        }
    }


    //--- Other Private methods ---
    public override void Awake() {
        HandleInstance();
        steamLobby.EventOnJoinGame += OnSteamLobbyJoinGame;
        steamLobby.EventOnCreateLobby += OnSteamLobbyCreateGame;
        if (useSteam) {
            SetSteamTransport();
        }
        else {
            SetKcpTransport();
        }
        base.Awake();
    }

    private void HandleInstance() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void SetSteamTransport() {
        useSteam = true;
        kcpTransport.enabled = false;
        steamTransport.enabled = true;
        steamLobby.enabled = true;
        steamManager.enabled = true;
        transport = steamTransport;
    }
    private void SetKcpTransport() {
        useSteam = false;
        steamTransport.enabled = false;
        steamLobby.enabled = false;
        steamManager.enabled = false;
        kcpTransport.enabled = true;
        transport = kcpTransport;
    }
}
