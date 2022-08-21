using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Steamworks;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerConnection : NetworkBehaviour
{
    //The playerconnection for a given client (set in first Init call)
    public static PlayerConnection myConnection;

    public string sceneToLoadOnDisconnect;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private InputActionAsset playerInputActions;

    private int numLocalPlayers = 0;
    public int GetNumLocalPlayers() { return numLocalPlayers; }

    private List<string> controlSchemes = new List<string>();
    private readonly SyncList<GameObject> playerCharacters = new SyncList<GameObject>();


    //Steam ID of this connection
    public CSteamID steamID;
    [Server]
    public void SetSteamID(ulong newID) {
        steamID = new CSteamID(newID);
        SetSteamIDRPC(newID);
    }
    [ClientRpc]
    private void SetSteamIDRPC(ulong newID) {
        steamID = new CSteamID(newID);
    }


    public void AddLocalPlayer(InputDevice device) {
        string scheme = "null";
        InputControlScheme[] schemes = playerInputActions.controlSchemes.ToArray();
        for (int i = 0; i < schemes.Length; i++) {
            if (schemes[i].SupportsDevice(device))
                scheme = schemes[i].name;
        }
        if(scheme == "null") {
            Debug.Log("No control scheme found for device: " + device.name);
            scheme = schemes[0].name;
        }
        controlSchemes.Add(scheme);
        Debug.Log("Added control scheme: " + scheme);
        numLocalPlayers++;
        SetNumLocalPlayersCMD(numLocalPlayers);
    }

    [Command]
    private void SetNumLocalPlayersCMD(int num) { numLocalPlayers = num; }

    public List<GameObject> GetPlayerCharacters() {
        List<GameObject> players = new List<GameObject>();
        for (int i = 0; i < playerCharacters.Count; i++) {
            players.Add(playerCharacters[i]);
        }
        return players; 
    }
    public string GetControlSchemeForPlayer(int playerIndex) {
        if (playerIndex > controlSchemes.Count)
            return "";
        return controlSchemes[playerIndex];
    }


    public static string GetName(PlayerConnection connection) {
        if (MyNetworkManager.instance.useSteam) {
            return SteamFriends.GetFriendPersonaName(connection.steamID);
        }
        else {
            return System.Environment.UserName + " [" + connection.netId + "]";
        }
        
        //return "Con [" + connection.netId + "]";
    }
    public static string GetName(PlayerConnection connection, GameObject player) {
        string name; 
        if (MyNetworkManager.instance.useSteam) {
            name = SteamFriends.GetFriendPersonaName(connection.steamID);
        }
        else {
            name = "Player";
        }

        List<GameObject> localPlayers = connection.GetPlayerCharacters();
        for (int i = 1; i < localPlayers.Count; i++) {
            if (player == localPlayers[i])
                name += " [" + (i+1).ToString() + "]";
        }
        return name;
    }
    public static Texture2D GetIcon(PlayerConnection connection) {
        Texture2D texture = null;
        if(connection != null) {
            if (MyNetworkManager.instance.useSteam) {
                int imageID = SteamFriends.GetLargeFriendAvatar(connection.steamID);
                texture = Utilities.GetSteamImageAsTexture(imageID);
            }
        }
        if (texture == null) {
            texture = Utilities.GetEmptyTexture(32,32, Color.clear);
        }
        return texture;
    }


    [ClientRpc(includeOwner = true)]
    public void Init() {
        numLocalPlayers = 0;
        controlSchemes.Clear();
        DontDestroyOnLoad(gameObject);
        if(myConnection == null) {
            //Debug.Log("My netID: " + netId);
            myConnection = this;
        }
    }

    //Leaves the steam lobby for all clients
    [ClientRpc(includeOwner = true)]
    public void CloseSteamLobby(CSteamID lobbyID) {
        Debug.Log(SteamFriends.GetPersonaName() + " Left Lobby");
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    [TargetRpc]
    public void SpawnPlayers(NetworkConnection network, Vector3 location) {
        SetNumSpawnedPlayers(0);
        for (int i = 0; i < numLocalPlayers; i++) {
            SpawnPlayerCommand(location, this, i);
        }
    }

    [SyncVar]
    private int numSpawnedPlayers = 0;
    [Command]
    private void SetNumSpawnedPlayers(int num) { numSpawnedPlayers = num; }

    [Command]
    private void SpawnPlayerCommand(Vector3 location, PlayerConnection conn, int playerIndex) {
        GameObject character = Instantiate(conn.playerPrefab, location, Quaternion.identity);
        character.name = "Player " +  PlayerConnection.GetName(conn);
        if(playerIndex > 0)
            character.name += " " + (playerIndex+1).ToString();
        character.GetComponent<Player>().SetConnection(conn);
        character.GetComponent<Player>().playerIndex = playerIndex;
        NetworkServer.Spawn(character, conn.connectionToClient);
        character.GetComponent<NetworkIdentity>().AssignClientAuthority(conn.connectionToClient);

        playerCharacters.Add(character);
        PlayerManager.instance.AddPlayerCharacter(character, conn);
    }
    [Command(requiresAuthority = false)]
    public void PlayerSpawnConfirm() {
        numSpawnedPlayers++;
        Debug.Log("Confirmed spawn: " + numSpawnedPlayers + " spawned for con [" + netId + "]");
    }

    public bool AllLocalPlayersSpawned() {
        return numSpawnedPlayers == numLocalPlayers;
    }

    public void Disconnect() {
        if (MyNetworkManager.instance.isNetworkActive) {
            MyNetworkManager.instance.offlineScene = sceneToLoadOnDisconnect;
            MyNetworkManager.instance.DisconnectFromLobby();
            if (isServer && isClient)
                MyNetworkManager.instance.StopHost();
            else if (isClient)
                MyNetworkManager.instance.StopClient();
        }
        else {
            SceneManager.LoadScene(sceneToLoadOnDisconnect);
        }
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy() {
        if(MyNetworkManager.instance!=null)
            MyNetworkManager.instance.OnPlayerConnectionDestroyed(this);
    }

}
