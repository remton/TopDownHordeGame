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

    public ulong lobbyID;

    // --- Events called by this script ---
    public struct JoinLobbyData {
        public string info;
        public bool successful;
        public CSteamID userSteamID;
    }
    public delegate void OnJoinGame(JoinLobbyData data);
    public event OnJoinGame EventOnJoinLobby;

    public struct CreateLobbyData {
        public string info;
        public bool successful;
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
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MyNetworkManager.instance.maxConnections);
            //Create lobby triggers a callback (OnLobbbyCreated) when it is done
        }
        else {
            CreateLobbyData createData;
            createData.lobbySteamID = CSteamID.Nil;
            createData.info = "Code already in use";
            createData.successful = false;
            CallCreateLobbyEvent(createData);
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
            JoinLobbyData data;
            data.successful = false;
            data.userSteamID = SteamUser.GetSteamID();
            data.info = "Lobby code not found";
            CallJoinLobbyEvent(data);
            Debug.Log("No lobbies with code:" + lobbyCode);
        }
    }

    public void GetLobbies(string code) {
        Debug.Log("GetLobbies called");

        isWaitingOnLobbyRequest = true;
        lobbyCode = code;
        SteamMatchmaking.AddRequestLobbyListStringFilter(
            LOBBYCODE_KEY,
            code,
            ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    public void LeaveLobby() {
        SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
    }

    //Event Call methods
    private void CallJoinLobbyEvent(JoinLobbyData data) {
        if (EventOnJoinLobby != null) {
            EventOnJoinLobby.Invoke(data);
        }
    }
    private void CallCreateLobbyEvent(CreateLobbyData data) {
        if(EventOnCreateLobby != null) {
            EventOnCreateLobby.Invoke(data);
        }
    }

    //callback called when steam finishes creating the lobby
    private void OnLobbyCreated(LobbyCreated_t callback) {
        CreateLobbyData createData;
        //if the lobby wasnt created
        if (callback.m_eResult == EResult.k_EResultAccessDenied) {
            //Debug.LogError("Could not create lobby: Access Denied!");
            createData.lobbySteamID = CSteamID.Nil;
            createData.info = "Could not create lobby: Access Denied!";
            createData.successful = false;
            CallCreateLobbyEvent(createData);
            return;
        }
        if(callback.m_eResult != EResult.k_EResultOK) {
            //Debug.LogError("Could not create lobby: " + callback.m_eResult.ToString());
            createData.lobbySteamID = CSteamID.Nil;
            createData.info = "Could not create lobby: " + callback.m_eResult.ToString();
            createData.successful = false;
            CallCreateLobbyEvent(createData);
            return;
        }

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
        lobbyID = callback.m_ulSteamIDLobby;
        Debug.Log("Created lobby with code: " + lobbyCode);
        MyNetworkManager.instance.StartHost();

        createData.lobbySteamID = new CSteamID(callback.m_ulSteamIDLobby);
        createData.info = "Created Lobby";
        createData.successful = true;
        CallCreateLobbyEvent(createData); 

        JoinLobbyData joinData;
        joinData.successful = true;
        joinData.userSteamID = SteamUser.GetSteamID();
        joinData.info = "Joined Lobby";
        CallJoinLobbyEvent(joinData);
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

        Debug.Log("We are client");

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HOSTADDRESS_KEY);

        //Check if we are trying to join a lobby we are hosting in
        JoinLobbyData data; 
        CSteamID lobby = new CSteamID(callback.m_ulSteamIDLobby);
        if(SteamMatchmaking.GetLobbyData(lobby, HOSTADDRESS_KEY) == SteamUser.GetSteamID().ToString()) {
            //Debug.LogWarning("Steamaccount: " + SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()) + " already in lobby");
            data.successful = false;
            data.userSteamID = SteamUser.GetSteamID();
            data.info = "Steam Account: " + SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()) + " already in lobby";
            CallJoinLobbyEvent(data);
            return;
        }
        MyNetworkManager.instance.networkAddress = hostAddress;
        //Debug.Log("Starting client!!!");
        MyNetworkManager.instance.StartClient(); 
        data.successful = true;
        data.userSteamID = SteamUser.GetSteamID();
        data.info = "Joined Lobby";
        CallJoinLobbyEvent(data);
    }

    //Callback when steam finishes lobby list request
    private void OnGetLobbiesList(LobbyMatchList_t result) {
        Debug.Log("OnGetLobbiesList called");

        isWaitingOnLobbyRequest = false;
        lobbies.Clear();
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for (int i = 0; i < result.m_nLobbiesMatching; i++) {
            lobbies.Add(SteamMatchmaking.GetLobbyByIndex(i));
        }
        if (EventAfterLobbyRequest != null) { 
            EventAfterLobbyRequest.Invoke();
            EventAfterLobbyRequest = null;
        }
    }

}
