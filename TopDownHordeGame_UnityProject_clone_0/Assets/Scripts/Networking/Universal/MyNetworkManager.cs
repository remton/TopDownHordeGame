using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance;

    private Timer timer;

    [Header("Transport set by script!!")]
    public bool useSteam;
    [SerializeField] protected Transport steamTransport;
    [SerializeField] protected SteamLobby steamLobby;
    [SerializeField] protected SteamManager steamManager;
    [SerializeField] protected Transport kcpTransport;
    private GameObject steamLobbyObj;

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
        if (useSteam) {
            SetSteamTransport();
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
        Debug.Log("Connected User ID:" + SteamFriends.GetFriendPersonaName(data.userSteamID));
    }
    private void OnSteamLobbyCreateGame(SteamLobby.CreateLobbyData data) {
        Debug.Log("Hosting Lobby ID:" + data.lobbySteamID);
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
    }


    //--- Other Private methods ---
    public override void Awake() {
        timer = GetComponent<Timer>();
        steamLobbyObj = steamLobby.gameObject;
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
        kcpTransport.enabled = false;
        useSteam = true;

        if (!gameObject.HasComponent<SteamManager>())
            steamManager = gameObject.AddComponent<SteamManager>();
        if (!gameObject.HasComponent<FizzySteamworks>())
            steamTransport = gameObject.AddComponent<FizzySteamworks>();
        if (!steamLobbyObj.HasComponent<SteamLobby>())
            steamLobby = steamLobbyObj.AddComponent<SteamLobby>();

        steamTransport.enabled = true;
        steamManager.enabled = true;
        steamLobby.enabled = true;
        transport = steamTransport;

        if (!SteamManager.Initialized)
            SetKcpTransport();
    }
    private void SetKcpTransport() {
        useSteam = false;

        if (gameObject.HasComponent<SteamManager>())
            Destroy(steamManager);            
        if (gameObject.HasComponent<FizzySteamworks>())
            Destroy(steamTransport);
        if(steamLobbyObj.HasComponent<SteamLobby>())
            Destroy(steamLobby);
        
        kcpTransport.enabled = true;
        transport = kcpTransport;
    }
}
