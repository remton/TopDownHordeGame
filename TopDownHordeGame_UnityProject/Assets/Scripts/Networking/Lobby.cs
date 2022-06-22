using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Mirror;

public class Lobby : NetworkBehaviour
{
    [SerializeField]
    private LobbyMenu menu;
    [SerializeField][Scene]
    private string gameSceneName;
    [SerializeField][Scene]
    private string mainMenuScene;

    [SyncVar]
    public int maxPlayers;
    [SyncVar]
    private int numPlayers;

    //Synced list holds details for each player's Ready state
    private readonly SyncList<PlayerLobbyDetails> playerDetails = new SyncList<PlayerLobbyDetails>();


    //Local devices
    private List<int> localDeviceIds = new List<int>();
    private List<InputDevice> localDevices = new List<InputDevice>();


    // ----- UNITY MESSAGES -----

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

        if (isClient) {
            //Set Synclist callback and update our UI
            playerDetails.Callback += OnPlayerDetailsChanged;
            UpdateUI();
        }
    }

    private void Update() {
        if(isClient)
            CheckAddDevice();
    }

    private void OnDestroy() {
        if (isServer) {
            //unsubscribe server events
            MyNetworkManager.instance.ServerEvent_PlayerConnectionAdded -= AddConnection;
            MyNetworkManager.instance.ServerEvent_PlayerConnectionRemoved -= RemoveConnection;
        }
    }


    // ------ CLIENT SIDE -----
    #region Client Side
    public override void OnStartClient() {
        base.OnStartClient();
    }

    [Client]
    public void LeaveLobby() {
        PlayerConnection.myConnection.Disconnect();
    }

    //Client method to update the UI (called whenever player details changes
    [Client]
    private void UpdateUI() {
        List<PlayerLobbyDetails> details = new List<PlayerLobbyDetails>();
        for (int i = 0; i < playerDetails.Count; i++) {
            details.Add(playerDetails[i]);
        }
        menu.UpdateUI(details);
    }
    //Client method to ready up
    [Client]
    public void ReadyUp() {
        ReadyUpCommand(PlayerConnection.myConnection.netId);
    }
    [Client]
    private bool TryJoinLocalPlayer(InputDevice device) {
        if (PlayerConnection.myConnection == null) {
            Debug.LogError("MyConnection not set!");
            return false;
        }
        if (numPlayers >= maxPlayers) {
            Debug.Log("max players reached");
            return false;
        }
        //Add a local player
        JoinLocalPlayerCommand(PlayerConnection.myConnection.netId);
        PlayerConnection.myConnection.devices.Add(device);
        PlayerConnection.myConnection.SetNumLocalPlayers(PlayerConnection.myConnection.GetNumLocalPlayers() + 1);
        return true;
    }
    //In update, adds a device if a button is pressed
    private void CheckAddDevice() {
        if (Gamepad.current != null && !localDeviceIds.Contains(Gamepad.current.deviceId)) {
            if (Gamepad.current.startButton.IsPressed()) {
                if (TryJoinLocalPlayer(Gamepad.current)) {
                    localDeviceIds.Add(Gamepad.current.deviceId);
                    localDevices.Add(Gamepad.current);
                }
            }
        }
        if (Keyboard.current != null && !localDeviceIds.Contains(Keyboard.current.deviceId)) {
            if (Keyboard.current.spaceKey.IsPressed()) {
                if (TryJoinLocalPlayer(Keyboard.current)) {
                    localDeviceIds.Add(Keyboard.current.deviceId);
                    localDevices.Add(Keyboard.current);
                }
            }
        }
    }
    //Callback for playerDetails change
    [Client]
    private void OnPlayerDetailsChanged(SyncList<PlayerLobbyDetails>.Operation op, int index, PlayerLobbyDetails oldDetails, PlayerLobbyDetails newDetails) {
        if (isClient)
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
    #endregion


    // ----- SERVER SIDE -----
    #region Server Side
    //Command called in the server to readyUp a player with given netID
    [Command(requiresAuthority = false)]
    private void ReadyUpCommand(uint netID) {
        int detailsIndex = GetDetailsIndex(netID);
        PlayerLobbyDetails oldDetails = playerDetails[detailsIndex];
        PlayerLobbyDetails newDetails;
        newDetails.name = oldDetails.name;
        newDetails.netID = oldDetails.netID;
        newDetails.hasDevice = oldDetails.hasDevice;
        newDetails.numLocalPlayers = oldDetails.numLocalPlayers;
        newDetails.isReady = true;
        playerDetails[detailsIndex] = newDetails;
    }

    //Server method to start the game
    [Server]
    private void StartGame() {
        GameSettings.instance.numPlayers = numPlayers;
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
        PlayerLobbyDetails details;
        details.netID = connection.netId;
        details.name = PlayerConnection.GetName(connection);
        details.hasDevice = connection.devices.Count > 0;
        details.isReady = false;
        details.numLocalPlayers = connection.GetNumLocalPlayers();
        playerDetails.Add(details);
    }

    [Command(requiresAuthority = false)]
    private void JoinLocalPlayerCommand(uint netID) {
        numPlayers++;
        int detailsIndex = GetDetailsIndex(netID);
        PlayerLobbyDetails oldDetails = playerDetails[detailsIndex];
        PlayerLobbyDetails newDetails;
        newDetails.netID = oldDetails.netID;
        newDetails.name = oldDetails.name;
        newDetails.isReady = oldDetails.isReady;
        newDetails.numLocalPlayers = oldDetails.numLocalPlayers + 1;
        newDetails.hasDevice = true;
        playerDetails[detailsIndex] = newDetails;
    }

    #endregion




    // gets the index of the playerDetails with a given netID
    private int GetDetailsIndex(uint netID) {
        for (int i = 0; i < playerDetails.Count; i++) {
            if (playerDetails[i].netID == netID)
                return i;
        }
        Debug.LogError("No details with netID: " + netID);
        return -1;
    }


    

    //Struct contains data that needs passed between clients for the lobby
    public struct PlayerLobbyDetails {
        public int numLocalPlayers;
        public uint netID;
        public string name;
        public bool isReady;
        public bool hasDevice;
    }
}

//
//
//                //////////
//              ////      ////
//             ///
//              ///
//                ////////
//                      ////
//                       ///
//             //        ///
//              //////////
//
//