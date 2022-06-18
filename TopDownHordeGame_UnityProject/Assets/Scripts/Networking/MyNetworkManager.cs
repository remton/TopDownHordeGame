using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;

[RequireComponent(typeof(SteamLobby)), 
 RequireComponent(typeof(FizzySteamworks)), 
 RequireComponent(typeof(kcp2k.KcpTransport))]
public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance {
        get {
            return singleton as MyNetworkManager;
        }
    }

    [Header("Transport set by script!!")]
    [SerializeField]
    private bool steamDisabled = false;
    [HideInInspector]
    public bool useSteam;

    protected SteamLobby steamLobby;
    protected Transport steamTransport;
    protected Transport kcpTransport;

    private List<PlayerConnection> playerConnections = new List<PlayerConnection>();
    public List<PlayerConnection> GetPlayerConnections() { return playerConnections; }

    //--- Events for various things ---
    public delegate void Server_PlayerConnectionAdded(PlayerConnection pConnection);
    public event Server_PlayerConnectionAdded ServerEvent_PlayerConnectionAdded;

    public delegate void Server_PlayerConnectionRemoved(PlayerConnection pConnection);
    public event Server_PlayerConnectionRemoved ServerEvent_PlayerConnectionRemoved;

    public delegate void Server_AllClientsReady();
    public event Server_AllClientsReady ServerEvent_AllClientsReady;

    //--- Public Methods ---
    public void HostOffline() {
        useSteam = false;
        networkAddress = "localhost";
        StartHost();
    }
    public void HostGame(string code) {
        Debug.Log("Hosting game");

        if (useSteam) {
            steamLobby.GetLobbies(code);
            steamLobby.HostLobby();
        }
        else {
            SetKcpTransport();
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
    [Server]
    public void ChangeScene(string sceneName) {
        if (isNetworkActive)
            ServerChangeScene(sceneName);
    }
    
    public int GetPrefabIndex(GameObject prefab) {
        for (int i = 0; i < spawnPrefabs.Count; i++) {
            if (prefab == spawnPrefabs[i])
                return i;
        }
        Debug.LogError("Prefab: " + prefab.name + " was not found in MyNetworkManager spawnable prefabs");
        return 0;
    }
    [Server]
    public GameObject GetPrefab(int index) {
        return spawnPrefabs[index];
    }

    //--- Handle SteamLobby Events ---
    private void OnSteamLobbyJoinGame(SteamLobby.JoinLobbyData data) {
        if (data.successful) {
            Debug.Log("Connecting User SteamID:" + SteamFriends.GetFriendPersonaName(data.userSteamID));
        }
    }
    private void OnSteamLobbyCreateGame(SteamLobby.CreateLobbyData data) {
        Debug.Log("Hosting Lobby SteamID:" + data.lobbySteamID);
    }


    public override void OnServerConnect(NetworkConnectionToClient conn) {
        base.OnServerConnect(conn);
        Debug.Log("Connected: " + conn.ToString());
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        base.OnServerDisconnect(conn);
        Debug.Log("Disconnected: " + conn.ToString());
    }
    public override void OnClientConnect() {
        base.OnClientConnect();
        Debug.Log("Connected!");
    }
    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        Debug.Log("Disconnected!");
    }

    public override void OnServerReady(NetworkConnectionToClient conn) {
        base.OnServerReady(conn);

        //Check if all clients are ready
        if(ServerEvent_AllClientsReady != null) {
            if (AllClientsReady()) {
                ServerEvent_AllClientsReady.Invoke();
            }
        }
    }

    public bool AllClientsReady() {
        bool b = true;
        for (int i = 0; i < playerConnections.Count; i++) {
            if (!playerConnections[i].connectionToClient.isReady) {
                b = false;
            }
        }
        return b;
    }
    public bool AllPlayerCharactersSpawned() {
        for (int i = 0; i < playerConnections.Count; i++) {
            if (!playerConnections[i].AllLocalPlayersSpawned())
                return false;
        }
        return true;
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
        player.GetComponent<PlayerConnection>().Init();
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

        //If we manually disable steam in editor
        useSteam = !steamDisabled;
    }

    //--- Other Private methods ---
    public override void Awake() {
        base.Awake();

        kcpTransport = GetComponent<kcp2k.KcpTransport>();
        if (!steamDisabled && useSteam && SetSteamTransport()) {
            steamLobby.EventOnJoinGame += OnSteamLobbyJoinGame;
            steamLobby.EventOnCreateLobby += OnSteamLobbyCreateGame;
        }
        else {
            DisableSteam();
            SetKcpTransport();
        }
    }

    public override void Start() {
        base.Start();
        useSteam = !steamDisabled;
    }

    private bool SetSteamTransport() {
        if (steamDisabled) {
            return false;
        }

        kcpTransport.enabled = false;
        useSteam = true;

        steamLobby = gameObject.GetComponent<SteamLobby>();
        steamTransport = gameObject.GetComponent<FizzySteamworks>();
       
        steamTransport.enabled = true;
        steamLobby.enabled = true;
        transport = steamTransport;

        //Failed connection to steam.
        if (!SteamManager.Initialized) {
            DisableSteam();
            return false;
        }
        return true;
    }
    private void SetKcpTransport() {
        useSteam = false;
        kcpTransport.enabled = true;
        transport = kcpTransport;
    }
    private void DisableSteam() {
        steamDisabled = true;
        useSteam = false;

        steamTransport.enabled = false;
        steamLobby.enabled = false;
    }

}
