using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Lobby : NetworkBehaviour
{
    [SerializeField]
    private LobbyMenu menu;
    [SerializeField]
    private string gameSceneName;


    //Synced list holds details for each player's Ready state
    private SyncList<PlayerDetails> playerDetails = new SyncList<PlayerDetails>();
    //Callback for playerDetails change
    private void OnPlayerDetailsChanged(SyncList<PlayerDetails>.Operation op, int index, PlayerDetails oldDetails, PlayerDetails newDetails) {
        if(isClient)
            UpdateUI();

        if (isServer) {
            //Check if everyone is ready and start the game
            bool everyoneReady = true;
            for (int i = 0; i < playerDetails.Count; i++) {
                if (!playerDetails[i].isReady)
                    everyoneReady = false;
            }
            if (everyoneReady) {
                StartGame();
            }
        }
    }


    //Client method to update the UI (called whenever player details changes
    [Client]
    private void UpdateUI() {
        List<PlayerDetails> details = new List<PlayerDetails>();
        for (int i = 0; i < playerDetails.Count; i++) {
            details.Add(playerDetails[i]);
        }
        PlayerDetails d = details[0];
        d.isReady = true;

        menu.UpdateUI(details);
    }
    //Client method to ready up
    [Client]
    public void ReadyUp() {
        ReadyUpCommand(PlayerConnection.myConnection.netId);
    }
    //Command called in the server to readyUp a player with given netID
    [Command(requiresAuthority = false)]
    private void ReadyUpCommand(uint netID) {
        int detailsIndex = GetDetailsIndex(netID);
        PlayerDetails oldDetails = playerDetails[detailsIndex];
        PlayerDetails newDetails = new PlayerDetails(oldDetails.netID, oldDetails.name, true);
        playerDetails[detailsIndex] = newDetails;
    }


    //Server method to start the game
    [Server]
    private void StartGame() {
        GameSettings.instance.numPlayers = 1;
        GameSettings.instance.devices.Add(Keyboard.current);
        MyNetworkManager.instance.ChangeScene(gameSceneName);
    }
    // AddConnection and RemoveConnection subscribed to server events 
    [Server]
    private void RemoveConnection(PlayerConnection connection) {
        for (int i = 0; i < playerDetails.Count; i++) {
            if(connection.netId == playerDetails[i].netID) {
                playerDetails.RemoveAt(i);
                return;
            }
        }
        Debug.LogWarning("No playerdetails with connectionID: " + connection.netId);
    }
    [Server]
    private void AddConnection(PlayerConnection connection) {
        PlayerDetails details;
        details.netID = connection.netId;
        details.name = PlayerConnection.GetName(connection);
        details.isReady = false;
        playerDetails.Add(details);
    }



    // gets the index of the playerDetails with a given netID
    private int GetDetailsIndex(uint netID) {
        for (int i = 0; i < playerDetails.Count; i++) {
            if (playerDetails[i].netID == netID)
                return i;
        }
        Debug.LogError("No details with netID: " + netID);
        return -1;
    }

    private void Start() {
        Cursor.visible = true;

        if (isServer) {
            //Subscribe to server events 
            MyNetworkManager.instance.ServerEvent_PlayerConnectionAdded += AddConnection;
            MyNetworkManager.instance.ServerEvent_PlayerConnectionRemoved += RemoveConnection;
            //Add all connected players (Should only really be the local player)
            List<PlayerConnection> playerConnections = MyNetworkManager.instance.GetPlayerConnections();
            for (int i = 0; i < playerConnections.Count; i++) {
                AddConnection(playerConnections[i]);
            }
        }

        if(isClient) {
            //Set Synclist callback and update our UI
            playerDetails.Callback += OnPlayerDetailsChanged;
            UpdateUI();
        }
    }
    private void OnDestroy() {
        if (isServer) {
            //unsubscribe server events
            MyNetworkManager.instance.ServerEvent_PlayerConnectionAdded -= AddConnection;
            MyNetworkManager.instance.ServerEvent_PlayerConnectionRemoved -= RemoveConnection;
        }
    }

    //Struct contains data that needs passed between clients for the lobby
    public struct PlayerDetails {
        public PlayerDetails(uint mNetID, string mName, bool mIsReady) {
            netID = mNetID;
            name = mName;
            isReady = mIsReady;
        }

        public uint netID;
        public string name;
        public bool isReady;
    }

}
