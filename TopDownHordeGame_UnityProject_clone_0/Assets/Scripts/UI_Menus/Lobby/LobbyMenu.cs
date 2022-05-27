using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyMenu : MonoBehaviour
{
    const string EMPTY_NAME_TEXT = "Empty";

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

        for (int i = 0; i < playerNames.Count; i++) {
            if(i < playerDetails.Count) {
                //Set the name
                playerNames[i].text = playerDetails[i].name;

                //Set ready button text
                if (playerDetails[i].isReady) {
                    readyButtons[i].GetComponentInChildren<Text>().text = READY_TEXT;
                }
                else {
                    readyButtons[i].GetComponentInChildren<Text>().text = NOT_READY_TEXT;
                }
                
                //If these are my details, set this as my button
                if(playerDetails[i].netID == PlayerConnection.myConnection.netId) {
                    SetMyReadyButton(i);
                }
            }
            else {
                playerNames[i].text = EMPTY_NAME_TEXT;
                readyButtons[i].GetComponentInChildren<Text>().text = NOT_READY_TEXT;
            }
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
