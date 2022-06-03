using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyMenu : MonoBehaviour
{
    const string EMPTY_NAME_TEXT = "Empty";
    const string JOIN_NAME_TEXT = "Press start/space to join!";
    const string NOT_READY_TEXT = "Not Ready";
    const string READY_TEXT = "Ready";

    const string EVERYONE_READY_TEXT = "Waiting on host . . .";
    const string NOT_EVERYONE_READY_TEXT = "Ready Up!";


    [SerializeField]
    private List<Text> playerNames;
    [SerializeField]
    private List<GameObject> readyButtons;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Lobby lobby;


    public void ReadyUp() {
        lobby.ReadyUp();
    }

    //Updates UI with the correct details
    public void UpdateUI(List<Lobby.PlayerDetails> playerDetails) {
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

            //If we havent joined a local player for this connection yet
            if(playerDetails[detailsIndex].numLocalPlayers == 0) {
                //Set the name
                playerNames[slotIndex].text = JOIN_NAME_TEXT;

                //Set ready button text
                readyButtons[slotIndex].GetComponentInChildren<Text>().text = NOT_READY_TEXT;

                slotIndex++;
                return;
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
                    if (playerDetails[detailsIndex].netID == PlayerConnection.myConnection.netId) {
                        SetMyReadyButton(slotIndex);
                    }
                }
                slotIndex++;
            }
        }
        //For the rest of the slots
        for (int i = slotIndex; i < numSlots; i++) {
            playerNames[i].text = EMPTY_NAME_TEXT;
            readyButtons[i].GetComponentInChildren<Text>().text = NOT_READY_TEXT;
        }
    }

    //Activates the button at the given index and deativates all others
    private void SetMyReadyButton(int index) {
        if (index > readyButtons.Count)
            Debug.LogError("ERROR: Cant set ready button to " + index + ". Only " + readyButtons.Count + "exist.");

        foreach (GameObject obj in readyButtons) {
            obj.GetComponent<Button>().interactable = false;
        }
        readyButtons[index].GetComponent<Button>().interactable = true;
    }
}
