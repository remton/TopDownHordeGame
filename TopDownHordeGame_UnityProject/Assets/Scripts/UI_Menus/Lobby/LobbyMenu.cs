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

    [SerializeField]
    private GameObject StartGameButton;
    [SerializeField]
    private List<Text> playerNames;
    [SerializeField]
    private List<GameObject> readyButtons;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text InfoBoxText;
    [SerializeField]
    private Lobby lobby;

    [SerializeField]
    private GameOptionsMenu gameOptionsMenu;

    public override void OnCancel() {
        //Do nothing
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
        int numSlots = playerNames.Count;
        int numPlayers = 0;
        for (int i = 0; i < playerDetails.Count; i++) {
            numPlayers += playerDetails[i].numLocalPlayers;
        }
        if (numPlayers > numSlots)
            Debug.LogError("More players than slots");

        int slotIndex = 0;
        //For every details (connection)
        for (int detailsIndex = 0; detailsIndex < playerDetails.Count; detailsIndex++) {
            if (!playerDetails[detailsIndex].isReady)
                allReady = false;

            //If we havent joined a local player for this connection yet
            if(playerDetails[detailsIndex].numLocalPlayers == 0) {
                //Set the name
                playerNames[slotIndex].text = EMPTY_NAME_TEXT;

                //Set ready button text
                readyButtons[slotIndex].GetComponentInChildren<Text>().text = NOT_READY_TEXT;
                ActivateReadyButton(slotIndex, false);
            }

            //For every local player
            for (int localPlayer = 0; localPlayer < playerDetails[detailsIndex].numLocalPlayers; localPlayer++) {
                //Only if there is a slot available
                if(slotIndex < numSlots) {
                    //Set the name
                    playerNames[slotIndex].text = playerDetails[detailsIndex].name;

                    //Set ready button text
                    if (playerDetails[detailsIndex].isReady) {
                        readyButtons[slotIndex].GetComponentInChildren<Text>().text = READY_TEXT;
                    }
                    else {
                        readyButtons[slotIndex].GetComponentInChildren<Text>().text = NOT_READY_TEXT;
                    }

                    //If these are my details, set this as my button
                    if (playerDetails[detailsIndex].netID == PlayerConnection.myConnection.netId && playerDetails[detailsIndex].hasDevice)
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
            playerNames[slotIndex].text = EMPTY_NAME_TEXT;
            readyButtons[slotIndex].GetComponentInChildren<Text>().text = EMPTY_READY_TEXT;
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
        if (index > readyButtons.Count)
            Debug.LogError("ERROR: Cant set ready button to " + index + ". Only " + readyButtons.Count + "exist.");

        readyButtons[index].GetComponent<Button>().interactable = on;
    }
}
