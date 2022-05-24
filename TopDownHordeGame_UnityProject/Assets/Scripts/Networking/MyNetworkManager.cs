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
    private CSteamID lobbyID;

    public delegate void OnTryJoinGame(bool wasSuccessful);
    public event OnTryJoinGame EventOnTryJoinGame;

    public delegate void OnHostGame();
    public event OnHostGame EventOnHostGame;

    public List<PlayerConnection> players = new List<PlayerConnection>();

    public List<CSteamID> GetConnectedUsers() {
        int numMembers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        List<CSteamID> users = new List<CSteamID>();
        for (int i = 0; i < numMembers; i++) {
            users.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i));
        }
        return users;
    }
    public void HostGame(string code) {
        if (useSteam) {
            steamLobby.GetLobbies(code);
            steamLobby.HostLobby();
        }
        else {
            StartHost();
            if (EventOnHostGame != null) { EventOnHostGame.Invoke(); }
            if (EventOnTryJoinGame != null) { EventOnTryJoinGame.Invoke(true); }
        }
    }
    public void JoinGame(string code) {
        if (useSteam) {
            steamLobby.GetLobbies(code);
            steamLobby.JoinLobby();
        }
        else {
            StartClient();
            if (EventOnTryJoinGame != null) { EventOnTryJoinGame.Invoke(true); }
        }
    }
    public void CloseLobby() {
        if (!useSteam)
            return;
        steamLobby.CloseLobby(lobbyID);
        lobbyID = CSteamID.Nil;
    }

    public void SetSteamTransport() {
        useSteam = true;
        kcpTransport.enabled = false;
        steamTransport.enabled = true;
        steamLobby.enabled = true;
        steamManager.enabled = true;
        transport = steamTransport;
    }
    public void SetKcpTransport() {
        useSteam = false;
        steamTransport.enabled = false;
        steamLobby.enabled = false;
        steamManager.enabled = false;
        kcpTransport.enabled = true;
        transport = kcpTransport;
    }

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

    private void OnSteamLobbyJoinGame(SteamLobby.JoinLobbyData data) {
        Debug.Log("Connected User ID:" + SteamFriends.GetFriendPersonaName(data.userSteamID));
        if(EventOnTryJoinGame != null) { EventOnTryJoinGame.Invoke(data.successful); }
    }
    private void OnSteamLobbyCreateGame(SteamLobby.CreateLobbyData data) {
        Debug.Log("Hosting Lobby ID:" + data.lobbySteamID);
        lobbyID = data.lobbySteamID;
        if (EventOnHostGame != null) { EventOnHostGame.Invoke(); }
    }

    private void HandleInstance() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        //BASE
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        //NEW
        PlayerConnection playerConn = player.GetComponent<PlayerConnection>();
        PlayerConnection.ConnectionData connectionData;
        connectionData.connection = conn;
        if (useSteam) {
            connectionData.useSteam = true;
            Debug.Log("Connection address: " + conn.address);
            if (conn.address != "localhost")
                connectionData.steamID = new CSteamID(ulong.Parse(conn.address));
            else
                connectionData.steamID = SteamUser.GetSteamID();
        }
        else {
            connectionData.useSteam = false;
            connectionData.steamID = CSteamID.Nil;
        }
        playerConn.Init(connectionData);
        players.Add(playerConn);
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

}
