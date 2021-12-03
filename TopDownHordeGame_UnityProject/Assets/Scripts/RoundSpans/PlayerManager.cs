using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spawnPoint;
    public PlayerSidebarManager sidebarManager;
    private List<GameObject> players = new List<GameObject>();
    private int numPlayers;

    public List<GameObject> GetPlayers() { return players; }

    public delegate void OnPlayersChange(List<GameObject> players);
    public event OnPlayersChange EventPlayersChange;

    public static PlayerManager instance;
    private void Awake() {
        if(instance == null) { instance = this; }
        else { Debug.Log("Two playerManagers active. Destroying one..."); }
    }

    private void Start() {
        CreatePlayers();
    }

    public void CreatePlayers() {
        numPlayers = GameSettings.instance.numPlayers;
        for (int i = 0; i < numPlayers; i++) {
            PlayerInput input = PlayerInputManager.instance.JoinPlayer(i, i);
            GameObject playerObj = input.gameObject;
            playerObj.transform.position = spawnPoint.transform.position;
            playerObj.GetComponent<PlayerInput>().camera = Camera.main;
            players.Add(playerObj);
            sidebarManager.AddSidebar(playerObj);
        }
        if(EventPlayersChange != null) { EventPlayersChange.Invoke(players); }
    }

    public void RemovePlayer(int index) {
        GameObject p = players[index];
        players.RemoveAt(index);
        Destroy(p);
        if (EventPlayersChange != null) { EventPlayersChange.Invoke(players); }
    }
}
