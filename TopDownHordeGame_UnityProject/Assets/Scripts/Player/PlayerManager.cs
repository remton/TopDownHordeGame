using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;


public class PlayerManager : NetworkBehaviour
{
    //Public vars
    public static PlayerManager instance;
    [Scene] public string GameOverScene;
    public GameObject spawnPoint;
    public GameObject deadPlayerLocation;

    //Public events
    public delegate void OnPlayersChanged(List<GameObject> players);
    public event OnPlayersChanged EventActiveLocalPlayersChange;
    public event OnPlayersChanged EventActivePlayersChange;

    //Private vars
    private List<GameObject> localPlayers = new List<GameObject>(); //Holds only the local players to this client
    private List<GameObject> allPlayers = new List<GameObject>(); //Holds all players in game

    public bool allowEndGame = true;

    //--- Public Methods ---

    //Returns the playercharacter with the given ID
    public static Player FindPlayerWithID(System.Guid id) {
        for (int i = 0; i < instance.allPlayers.Count; i++) {
            GameObject player = instance.allPlayers[i];
            if (player.GetComponent<Player>().GetPlayerID() == id)
                return player.GetComponent<Player>();
        }
        Debug.LogWarning("Could not find player with id:" + id);
        return null;
    }

    //Returns living local players
    public List<GameObject> GetActiveLocalPlayers() {
        List<GameObject> localPlayers = new List<GameObject>();
        foreach (GameObject player in this.localPlayers) {
            if (!player.GetComponent<PlayerHealth>().GetIsDead())
                localPlayers.Add(player);
        }
        return localPlayers;
    }

    //Returns all living players
    public List<GameObject> GetActivePlayers() {
        List<GameObject> activePlayers = new List<GameObject>();
        for (int i = 0; i < allPlayers.Count; i++) {
            GameObject player = allPlayers[i];
            if (!player.GetComponent<PlayerHealth>().GetIsDead())
                activePlayers.Add(allPlayers[i]);
        }
        return activePlayers;
    }
    //Returns all players in game
    public List<GameObject> GetAllPlayers() {
        return allPlayers;
    }

    public int NumPlayers() { 
        return allPlayers.Count; 
    }

    /// <summary> [ClientRPC] Respawns all dead players </summary>
    [ClientRpc]
    public void RespawnDeadPlayersRPC() {
        //Clients respawn all dead players since respawn does not auto update the server
        for (int i = 0; i < allPlayers.Count; i++) {
            GameObject player = allPlayers[i];
            if (player == null)
                continue;

            if (player.GetComponent<PlayerHealth>().GetIsDead()) {
                player.GetComponent<Player>().EnablePlayer();
                player.transform.position = spawnPoint.transform.position;
                player.GetComponent<PlayerHealth>().Respawn();
                //Debug.Log("Player respawned");
            }
        }
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    /// <summary> [ClientRPC] Revives all downed players </summary>
    [ClientRpc]
    public void ReviveDownedPlayersRPC() {
        //Each client revives their own local players Revive auto updates the server
        for (int i = 0; i < localPlayers.Count; i++) {
            GameObject player = localPlayers[i];
            if (player == null)
                continue;

            if (player.GetComponent<PlayerHealth>().GetIsBleedingOut()) {
                player.GetComponent<PlayerHealth>().ReviveCMD();
                player.GetComponent<Player>().EnablePlayer();
                //Debug.Log("Player revived.");
            }
        }
    }

    [ClientRpc]
    public void HealAllPlayersRPC(){
                for (int i = 0; i < localPlayers.Count; i++) {
            GameObject player = localPlayers[i];
            if (player == null)
                continue;

            player.GetComponent<PlayerHealth>().Heal(7000);
            //Debug.Log("Player healed.");
        }
    }

    /// <summary> [ClientRPC] Called on all clients when a player dies </summary>
    [ClientRpc]
    public void OnPlayerDieRPC(GameObject player) {
        player.GetComponent<Player>().DisablePlayer();
        player.transform.position = deadPlayerLocation.transform.position;
        CheckGameOverCMD();
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
    }

    /// <summary> [Server] Adds the given player character </summary>
    [Server]
    public void AddPlayerCharacter(GameObject player, PlayerConnection connection) {
        AddLocalPlayerCharacterTRPC(connection.connectionToClient, player);
        allPlayers.Add(player);
        SetAllPlayersRPC(allPlayers.ToArray());
    }

    /// <summary> [Server] Removes given player character </summary>
    [Server]
    public void RemovePlayerCharacter(GameObject player, PlayerConnection connection) {
        RemoveLocalPlayerCharacterTRPC(connection.connectionToClient, player);
        allPlayers.Remove(player);
        SetAllPlayersRPC(allPlayers.ToArray());
    }

    [ClientRpc]
    public void SetGaveOverData() {
        GameOverData.instance.SetData(allPlayers, RoundController.instance.round);
    }


    [Client]
    public void SavePlayerData() {
        SaveData.Save();
    }

    // --- Private Methods ---

    // Called immediatly when creating this script object
    private void Awake() {
        //Handle instance
        if (instance == null) { instance = this; }
        else { Debug.Log("Two playerManagers active. Destroying one..."); }
        //Set up events from MyNetworkManager
        MyNetworkManager.instance.ServerEvent_PlayerConnectionRemoved += OnPlayerLeft;
    }


    // called on first frame in scene only on server
    public override void OnStartServer() {
        SceneLoader.instance.AddClientsLoad(CreatePlayers);
    }

    // called on first frame in scene only on clients
    public override void OnStartClient() {
        base.OnStartClient();
        localPlayers.Clear();
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    /// <summary> [Server] call when a player leaves </summary>
    [Server]
    private void OnPlayerLeft(PlayerConnection connection) {
        List<GameObject> charas = connection.GetPlayerCharacters();
        for (int i = 0; i < allPlayers.Count; i++) {
            if (allPlayers[i] == null)
                allPlayers.RemoveAt(i);
        }
        for (int i = 0; i < charas.Count; i++) {
            allPlayers.Remove(charas[i]);
        }
        SetAllPlayersRPC(allPlayers.ToArray());
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    /// <summary> [Server] Creates player characters for all player connections </summary>
    [Server]
    private void CreatePlayers() {
        //This is an online game
        List<PlayerConnection> connections = MyNetworkManager.instance.GetPlayerConnections();
        if (MyNetworkManager.instance.isNetworkActive) {
            for (int i = 0; i < connections.Count; i++) {
                connections[i].SpawnPlayers(connections[i].connectionToClient, spawnPoint.transform.position);
            }
        }
        else {
            Debug.LogError("NetworkServer is inactive");
        }
    }

    /// <summary> [ClientRpc] Updates allplayers list for every client </summary>
    [ClientRpc]
    private void SetAllPlayersRPC(GameObject[] allPlayers) {
        this.allPlayers = new List<GameObject>(allPlayers); 
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    /// <summary> [TargetRpc] Adds local player character to localPlayers list </summary>
    [TargetRpc]
    private void AddLocalPlayerCharacterTRPC(NetworkConnection network, GameObject player) {
        localPlayers.Add(player);
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
    }

    /// <summary> [TargetRpc] Removes local player character to localPlayers list </summary>
    [TargetRpc]
    private void RemoveLocalPlayerCharacterTRPC(NetworkConnection network, GameObject player) {
        localPlayers.Remove(player);
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
    }

    /// <summary> [Command] Checks if game should end, and ends it accordingly </summary>

    [Command(requiresAuthority = false)]
    private void CheckGameOverCMD() {
        List<PlayerConnection> connections = MyNetworkManager.instance.GetPlayerConnections();
        for (int i = 0; i < connections.Count; i++) {
            List<GameObject> localCharacters = connections[i].GetPlayerCharacters();
            for (int j = 0; j < localCharacters.Count; j++) {
                GameObject playerChar = localCharacters[j];
                if (!playerChar.GetComponent<PlayerHealth>().GetIsDead())
                    return;
            }
        }
        //Everyone died so end game
        EndGame();
    }

    [Server]
    public void EndGame(bool forceEnd = false) {
        if (!allowEndGame && !forceEnd)
            return;
        SetGaveOverData();
        SavePlayerData();
        MyNetworkManager.instance.ChangeScene(GameOverScene);
    }
}
