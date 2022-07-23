using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;


public class PlayerManager : NetworkBehaviour
{
    [Scene]
    public string GameOverScene;

    public GameObject spawnPoint;
    public GameObject deadPlayerLocation;
    private List<GameObject> localPlayers = new List<GameObject>();
    private readonly SyncList<GameObject> allPlayers = new SyncList<GameObject>();

    private void OnAllPlayersChange(SyncList<GameObject>.Operation op, int index, GameObject oldObj, GameObject newObj) {
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }
    
    public static Player FindPlayerWithID(System.Guid id) {
        foreach (GameObject player in instance.allPlayers) {
            if (player.GetComponent<Player>().GetPlayerID() == id)
                return player.GetComponent<Player>();
        }
        Debug.LogWarning("Could not find player with id:" + id);
        return null;
    }

    public List<GameObject> GetActiveLocalPlayers() {
        List<GameObject> localPlayers = new List<GameObject>();
        foreach (GameObject player in this.localPlayers) {
            if (!player.GetComponent<PlayerHealth>().GetIsDead())
                localPlayers.Add(player);
        }
        return localPlayers;
    }

    public List<GameObject> GetActivePlayers() {
        List<GameObject> activePlayers = new List<GameObject>();
        foreach (GameObject player in allPlayers) {
            if (!player.GetComponent<PlayerHealth>().GetIsDead())
                activePlayers.Add(player);
        }
        return activePlayers;
    }

    public delegate void OnPlayersChanged(List<GameObject> players);
    public event OnPlayersChanged EventActiveLocalPlayersChange;
    public event OnPlayersChanged EventActivePlayersChange;

    public static PlayerManager instance;
    private void Awake() {
        if (instance == null) { instance = this; }
        else { Debug.Log("Two playerManagers active. Destroying one..."); }

        //Set up events from MyNetworkManager
        MyNetworkManager.instance.ServerEvent_PlayerConnectionRemoved += OnPlayerLeft;
        allPlayers.Callback += OnAllPlayersChange;
    }

    private void Start() {
        CreatePlayers();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        localPlayers.Clear();
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

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
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    [Server]
    public void CreatePlayers() {
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


    [TargetRpc]
    public void AddLocalPlayerCharacter(NetworkConnection network, GameObject player) {
        localPlayers.Add(player);
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
    }
    [TargetRpc]
    public void RemoveLocalPlayerCharacter(NetworkConnection network, GameObject player) {
        localPlayers.Remove(player);
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
    }

    [Server]
    public void AddPlayerCharacter(GameObject player, PlayerConnection connection) {
        AddLocalPlayerCharacter(connection.connectionToClient, player);
        allPlayers.Add(player);
    }
    [Server]
    public void RemovePlayerCharacter(GameObject player, PlayerConnection connection) {
        RemoveLocalPlayerCharacter(connection.connectionToClient, player);
        allPlayers.Remove(player);
    }

    [ClientRpc]
    public void RespawnDeadPlayers() {
        //Clients respawn all dead players since respawn does not auto update the server
        foreach (GameObject player in allPlayers) {
            if (player == null)
                continue;

            if (player.GetComponent<PlayerHealth>().GetIsDead()) {
                player.SetActive(true);
                player.transform.position = spawnPoint.transform.position;
                player.GetComponent<PlayerHealth>().Respawn();
                //Debug.Log("Player respawned");
            }
        }
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    [ClientRpc]
    public void ReviveDownedPlayers() {
        //Each client revives their own local players Revive auto updates the server
        foreach (GameObject player in localPlayers) {
            if (player == null)
                continue;

            if (player.GetComponent<PlayerHealth>().GetIsBleedingOut()) {
                player.GetComponent<PlayerHealth>().Revive();
                //Debug.Log("Player revived.");
            }
        }
    }

    [ClientRpc]
    public void OnPlayerDie(GameObject player) {
        player.SetActive(false);
        player.transform.position = deadPlayerLocation.transform.position;
        CheckGameOver();
        if (EventActiveLocalPlayersChange != null) { EventActiveLocalPlayersChange.Invoke(GetActiveLocalPlayers()); }
    }


    [Command(requiresAuthority = false)]
    private void CheckGameOver() {
        List<PlayerConnection> connections = MyNetworkManager.instance.GetPlayerConnections();
        for (int i = 0; i < connections.Count; i++) {
            List<GameObject> localCharacters = connections[i].GetPlayerCharacters();
            for (int j = 0; j < localCharacters.Count; j++) {
                GameObject playerChar = localCharacters[j];
                if (!playerChar.GetComponent<PlayerHealth>().GetIsDead())
                    return;
            }
        }

        //Everyone died
        GameOverData.instance.SetData(localPlayers, RoundController.instance.round);
        SaveData.Save();
        MyNetworkManager.instance.ChangeScene(GameOverScene);
    }
}
