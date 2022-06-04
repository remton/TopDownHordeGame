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
    public int numLocalPlayers = 0;
    public List<InputDevice> devices = new List<InputDevice>();


    [SerializeField]
    private GameObject playerPrefab;

    private SyncList<GameObject> playerCharacters = new SyncList<GameObject>();
    public List<GameObject> GetPlayerCharacters() {
        List<GameObject> players = new List<GameObject>();
        for (int i = 0; i < playerCharacters.Count; i++) {
            players.Add(playerCharacters[i]);
        }
        return players; 
    }

    public static string GetName(PlayerConnection connection) {
        ConnectionData data = connection.connectionData;
        if (data.useSteam)
            return SteamFriends.GetFriendPersonaName(new CSteamID(ulong.Parse(data.steamID)));
        else
            return "Con [" + connection.netId + "]";
    }
    public static string GetName(PlayerConnection connection, GameObject player) {
        ConnectionData data = connection.connectionData;
        string name;
        if (data.useSteam)
            name = SteamFriends.GetFriendPersonaName(new CSteamID(ulong.Parse(data.steamID)));
        else
            name = "Player";

        List<GameObject> localPlayers = connection.GetPlayerCharacters();
        for (int i = 1; i < localPlayers.Count; i++) {
            if (player == localPlayers[i])
                name += " [" + (i+1).ToString() + "]";
        }
        return name;
    }

    [ClientRpc(includeOwner = true)]
    public void Init(ConnectionData data) {
        numLocalPlayers = 0;
        devices.Clear();
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
    public void SpawnPlayers(Vector3 location) {
        for (int i = 0; i < numLocalPlayers; i++) {
            SpawnPlayerCommand(location, this, i);
        }

    }

    [Command]
    private void SpawnPlayerCommand(Vector3 location, PlayerConnection conn, int deviceIndex) {
        GameObject character = Instantiate(conn.playerPrefab, location, Quaternion.identity);
        character.GetComponent<Player>().SetConnection(conn);
        //InputActionAsset inputAsset = character.GetComponent<PlayerInput>().actions;
        //InputControlScheme scheme = (InputControlScheme)InputControlScheme.FindControlSchemeForDevice(devices[deviceIndex], inputAsset.controlSchemes);
        //character.GetComponent<PlayerInput>().defaultControlScheme = scheme.name; 
        NetworkServer.Spawn(character, conn.connectionToClient);
        character.GetComponent<NetworkIdentity>().AssignClientAuthority(conn.connectionToClient);

        playerCharacters.Add(character);
        PlayerManager.instance.AddPlayerCharacter(character, conn);

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
