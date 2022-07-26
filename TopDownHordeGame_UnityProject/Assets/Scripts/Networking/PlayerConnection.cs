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

    public static string GetName(PlayerConnection connection) {
        return "Con [" + connection.netId + "]";

        //ConnectionData data = connection.connectionData;
        //if (MyNetworkManager.instance.useSteam)
        //    return SteamFriends.GetFriendPersonaName(new CSteamID(ulong.Parse(data.steamID)));
        //else
        //    return "Con [" + connection.netId + "]";
    }
    public static string GetName(PlayerConnection connection, GameObject player) {
        string name;
        name = "Player";

        //if (MyNetworkManager.instance.useSteam)
        //    name = SteamFriends.GetFriendPersonaName(new CSteamID(ulong.Parse(data.steamID)));
        //else
        //    name = "Player";

        List<GameObject> localPlayers = connection.GetPlayerCharacters();
        for (int i = 1; i < localPlayers.Count; i++) {
            if (player == localPlayers[i])
                name += " [" + (i+1).ToString() + "]";
        }
        return name;
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
            SpawnPlayerCommand(location, this, controlSchemes[i]);
        }
    }

    [SyncVar]
    private int numSpawnedPlayers = 0;
    [Command]
    private void SetNumSpawnedPlayers(int num) { numSpawnedPlayers = num; }

    [Command]
    private void SpawnPlayerCommand(Vector3 location, PlayerConnection conn, string controlScheme) {
        GameObject character = Instantiate(conn.playerPrefab, location, Quaternion.identity);
        character.GetComponent<Player>().SetConnection(conn);
        character.GetComponent<PlayerInput>().neverAutoSwitchControlSchemes = true;
        character.GetComponent<PlayerInput>().defaultControlScheme = controlScheme;
        character.GetComponent<PlayerInput>().SwitchCurrentControlScheme(controlScheme);
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
