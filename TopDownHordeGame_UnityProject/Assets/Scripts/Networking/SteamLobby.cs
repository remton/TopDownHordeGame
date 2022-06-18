using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    // --- Callbacks used by steam when various things happen ---

    //When steam finishes creating a lobby
    protected Callback<LobbyCreated_t> lobbyCreated;

    //When a steam user tries to join via steam
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;

    //When the user joins a lobby
    protected Callback<LobbyEnter_t> lobbyEntered;

    //When steam finishes getting the list of lobbies
    protected Callback<LobbyMatchList_t> lobbyList;


    // --- Events called by this script ---
    public struct JoinLobbyData {
        public bool successful;
        public CSteamID userSteamID;
    }
    public delegate void OnJoinGame(JoinLobbyData data);
    public event OnJoinGame EventOnJoinGame;

    public struct CreateLobbyData {
        public CSteamID lobbySteamID;
    }
    public delegate void OnCreateLobby(CreateLobbyData data);
    public event OnCreateLobby EventOnCreateLobby;


    // --- Keys for storing lobby data ---
    public const string HOSTADDRESS_KEY = "HostAddress";
    public const string LOBBYCODE_KEY = "Code";
    

    // --- private data for logic of requesting lobbies ---
    private delegate void AfterLobbyRequest();
    private event AfterLobbyRequest EventAfterLobbyRequest;
    private bool isWaitingOnLobbyRequest;
    private string lobbyCode;
    private List<CSteamID> lobbies = new List<CSteamID>();

    private void Awake() {
        //Debug.Log("Awake called");
        //Debug.Log("enabled? " + this.enabled);
        //Debug.Log("gameobject enabled? " + this.gameObject.activeSelf);
    }


    private void Start() {
        // if not signed into steam
        if (!SteamManager.Initialized) {
            Debug.Log("Must sign into steam for online play");
            return;
        }

        //Set steam callback functions
        Debug.Log("callbacks set");
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);

        DontDestroyOnLoad(gameObject);
    }
    public void HostLobby() {
        if (isWaitingOnLobbyRequest) {
            EventAfterLobbyRequest += HostLobby;
            return;
        }

        if(lobbies.Count == 0) {
            Debug.Log("Creating lobby. . .");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MyNetworkManager.instance.maxConnections);
            //Create lobby triggers a callback (OnLobbbyCreated) when it is done
        }
        else {
            Debug.Log("code already in use");
        }
    }
    public void JoinLobby() {
        if (isWaitingOnLobbyRequest) {
            EventAfterLobbyRequest += JoinLobby;
            return;
        }

        if(lobbies.Count > 0) {
            Debug.Log("joining lobby:" + lobbies[0].ToString());
            SteamMatchmaking.JoinLobby(lobbies[0]);
            //Join lobby triggers a callback (OnLobbyEntered) when it is done
        }
        else {
            CallJoinLobbyEvent(false);
            Debug.Log("No lobbies with code:" + lobbyCode);
        }
    }

    public void GetLobbies(string code) {
        isWaitingOnLobbyRequest = true;
        lobbyCode = code;
        SteamMatchmaking.AddRequestLobbyListStringFilter(
            LOBBYCODE_KEY,
            code,
            ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    //Event Call methods
    private void CallJoinLobbyEvent(bool joined) {
        if (EventOnJoinGame != null) {
            JoinLobbyData data;
            data.successful = joined;
            data.userSteamID = SteamUser.GetSteamID();
            EventOnJoinGame.Invoke(data);
        }
    }
    private void CallCreateLobbyEvent(CSteamID lobbyID) {
        if(EventOnCreateLobby != null) {
            CreateLobbyData data;
            data.lobbySteamID = lobbyID;
            EventOnCreateLobby.Invoke(data);
        }
    }

    //callback called when steam finishes creating the lobby
    private void OnLobbyCreated(LobbyCreated_t callback) {
        //if the lobby wasnt created
        if (callback.m_eResult == EResult.k_EResultAccessDenied) {
            Debug.LogError("Could not create lobby: Access Denied!");
        }
        if(callback.m_eResult != EResult.k_EResultOK) {
            Debug.LogError("Could not create lobby: " + callback.m_eResult.ToString());
            return;
        }

        MyNetworkManager.instance.StartHost();

        //Adds data to the lobby. Data is accessed using a string key in this case, HOSTADDRESS_KEY.
        //Here we are storing the user's steam id as the address.
        //With steamworks you connect using steam Ids rather than IP Adresses
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HOSTADDRESS_KEY, 
            SteamUser.GetSteamID().ToString());
        //Here we are setting the code for our lobby
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            LOBBYCODE_KEY,
            lobbyCode);
        Debug.Log("Lobby host address: " + SteamUser.GetSteamID().ToString());
        CallCreateLobbyEvent(new CSteamID(callback.m_ulSteamIDLobby));
        CallJoinLobbyEvent(true);
    }

    //Callback when a user tries to join via steam
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    //Callback When a user joins a lobby
    private void OnLobbyEntered(LobbyEnter_t callback) {
        //If we are the host
        if (NetworkServer.active) {
            return;
        }

        Debug.Log("Entered lobby: " + callback.m_ulSteamIDLobby); 

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HOSTADDRESS_KEY);

        //Check if we are trying to join a lobby we are hosting in
        CSteamID lobby = new CSteamID(callback.m_ulSteamIDLobby);
        if(SteamMatchmaking.GetLobbyData(lobby, HOSTADDRESS_KEY) == SteamUser.GetSteamID().ToString()) {
            Debug.LogWarning("Steamaccount: " + SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()) + " already in lobby");
            CallJoinLobbyEvent(false);
            return;
        }
        MyNetworkManager.instance.networkAddress = hostAddress;
        MyNetworkManager.instance.StartClient();
        CallJoinLobbyEvent(true);
    }

    //Callback when steam finishes lobby list request
    private void OnGetLobbiesList(LobbyMatchList_t result) {
        isWaitingOnLobbyRequest = false;
        lobbies.Clear();
        //Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for (int i = 0; i < result.m_nLobbiesMatching; i++) {
            lobbies.Add(SteamMatchmaking.GetLobbyByIndex(i));
        }
        if (EventAfterLobbyRequest != null) { 
            EventAfterLobbyRequest.Invoke();
            EventAfterLobbyRequest = null;
        }
    }
}
