using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyMenu : Menu
{
    const string EMPTY_NAME_TEXT = "Press start/space to join!";
    const string EMPTY_READY_TEXT = " ";
    const string NOT_READY_TEXT = "Not Ready";
    const string READY_TEXT = "Ready";

    const string INFOBOX_ALL_READY_TEXT = "Waiting on host...";
    const string INFOBOX_NOT_READY_TEXT = "Waiting for players to ready up...";

    [System.Serializable]
    private struct PlayerBox {
        public GameObject readyButton;
        public Text nameText;
        public RawImage icon;
    }
    [SerializeField]
    private List<PlayerBox> playerBoxes;

    [SerializeField]
    private GameObject StartGameButton;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text InfoBoxText;
    [SerializeField]
    private Lobby lobby;

    [SerializeField]
    private GameOptionsMenu gameOptionsMenu;

    private List<bool> readyGameButtonInteractable = new List<bool>();
    private bool startGameButtonInteractable;
    private bool hasSavedInteractable = false;

    public override void OnCancel() {
        //Do nothing
    }
    public override void SetInteractable(bool interactable) {
        List<GameObject> readyButtons = new List<GameObject>();
        foreach (var box in playerBoxes) {
            readyButtons.Add(box.readyButton);
        }

        if (!interactable) {
            hasSavedInteractable = true;
            readyGameButtonInteractable.Clear();
            foreach (var button in readyButtons) {
                readyGameButtonInteractable.Add(button.GetComponent<Button>().interactable);
            }
            startGameButtonInteractable = StartGameButton.GetComponent<Button>().interactable;
        }

        Selectable[] selects = gameObject.GetComponentsInChildren<Selectable>();
        foreach (Selectable select in selects) {
            if(!readyButtons.Contains(select.gameObject) && select.gameObject != StartGameButton)
                select.interactable = interactable;
        }

        if (interactable) {
            if (hasSavedInteractable) {
                hasSavedInteractable = false;
                for (int i = 0; i < readyButtons.Count; i++) {
                    readyButtons[i].GetComponent<Button>().interactable = readyGameButtonInteractable[i];
                }
                StartGameButton.GetComponent<Button>().interactable = startGameButtonInteractable;
            }
        }
    }

    //Buttons
    public void ReadyUp() {
        lobby.ReadyUp();
    }
    public void LeaveLobby() {
        lobby.LeaveLobby();
    }

    //Updates UI with the correct details
    public void UpdateUI(List<Lobby.PlayerLobbyDetails> playerDetails) {

        bool allReady = true;
        int numSlots = playerBoxes.Count;
        int numPlayers = 0;
        for (int i = 0; i < playerDetails.Count; i++) {
            numPlayers += playerDetails[i].numLocalPlayers;
        }
        if (numPlayers > numSlots)
            Debug.LogError("More players than slots");


        //List<GameObject> readyButtons = new List<GameObject>();
        //List<Text> playerNames = new List<Text>();
        //foreach (var box in playerBoxes) {
        //    readyButtons.Add(box.readyButton);
        //}


        int slotIndex = 0;
        //For every details (connection)
        for (int detailsIndex = 0; detailsIndex < playerDetails.Count; detailsIndex++) {
            if (!playerDetails[detailsIndex].isReady)
                allReady = false;

            //If we havent joined a local player for this connection yet
            if(playerDetails[detailsIndex].numLocalPlayers == 0) {
                //Set the name
                playerBoxes[slotIndex].nameText.text = EMPTY_NAME_TEXT;
                playerBoxes[slotIndex].icon.texture = PlayerConnection.GetIcon(null);
                playerBoxes[slotIndex].icon.color = Color.clear;
                //Set ready button text
                playerBoxes[slotIndex].readyButton.GetComponentInChildren<Text>().text = NOT_READY_TEXT;
                ActivateReadyButton(slotIndex, false);
            }

            //For every local player
            for (int localPlayer = 0; localPlayer < playerDetails[detailsIndex].numLocalPlayers; localPlayer++) {
                //Only if there is a slot available
                if(slotIndex < numSlots) {
                    //Set the name
                    playerBoxes[slotIndex].nameText.text = playerDetails[detailsIndex].name;
                    playerBoxes[slotIndex].icon.texture = PlayerConnection.GetIcon(playerDetails[detailsIndex].connection);
                    playerBoxes[slotIndex].icon.color = Color.white;
                    //Set ready button text
                    if (playerDetails[detailsIndex].isReady) {
                        playerBoxes[slotIndex].readyButton.GetComponentInChildren<Text>().text = READY_TEXT;
                    }
                    else {
                        playerBoxes[slotIndex].readyButton.GetComponentInChildren<Text>().text = NOT_READY_TEXT;
                    }

                    //If these are my details, set this as my button
                    if (playerDetails[detailsIndex].netID == PlayerConnection.myConnection.netId && playerDetails[detailsIndex].numLocalPlayers > 0)
                        ActivateReadyButton(slotIndex, true);
                    else
                        ActivateReadyButton(slotIndex, false);
                }
                slotIndex++;
            }

            //Handle start game button
            if (PlayerConnection.myConnection.isServer) {
                StartGameButton.SetActive(true);
                StartGameButton.GetComponent<Button>().interactable = allReady;
            }
            else {
                StartGameButton.GetComponent<Button>().interactable = false;
                StartGameButton.SetActive(false);
            }
        }
        //For the rest of the slots
        for (; slotIndex < numSlots; slotIndex++) {
            playerBoxes[slotIndex].nameText.text = EMPTY_NAME_TEXT;
            playerBoxes[slotIndex].icon.texture = PlayerConnection.GetIcon(null);
            playerBoxes[slotIndex].icon.color = Color.clear;
            playerBoxes[slotIndex].readyButton.GetComponentInChildren<Text>().text = EMPTY_READY_TEXT;
            ActivateReadyButton(slotIndex, false);
        }

        //Handle Infobox and gameoptions menu
        if (allReady) {
            InfoBoxText.text = INFOBOX_ALL_READY_TEXT;
        }
        else {
            InfoBoxText.text = INFOBOX_NOT_READY_TEXT;
            gameOptionsMenu.Close();
        }

    }

    //Activates the button at the given index and deativates all others
    private void ActivateReadyButton(int index, bool on) {
        if (index > playerBoxes.Count)
            Debug.LogError("ERROR: Cant set ready button to " + index + ". Only " + playerBoxes.Count + "exist.");

        playerBoxes[index].readyButton.GetComponent<Button>().interactable = on;
    }
}
