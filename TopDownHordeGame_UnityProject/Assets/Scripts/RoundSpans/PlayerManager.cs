using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public string GameOverScene;

    public GameObject playerPrefab;
    public GameObject spawnPoint;
    public GameObject deadPlayerLocation;
    public PlayerSidebarManager sidebarManager;
    private List<GameObject> players = new List<GameObject>();
    private int numPlayers;

    public List<GameObject> GetActivePlayers() {
        List<GameObject> activePlayers = new List<GameObject>();
        foreach (GameObject player in players) {
            if (!player.GetComponent<PlayerHealth>().GetIsDead())
                activePlayers.Add(player);
        }
        return activePlayers;
    }

    public delegate void OnPlayersChange(List<GameObject> players);
    public event OnPlayersChange EventActivePlayersChange;

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
        if(EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    public void RespawnDeadPlayers() {
        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerHealth>().GetIsDead()) {
                player.transform.position = spawnPoint.transform.position;
                player.SetActive(true);
            }
        }
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    public void OnPlayerDie(GameObject player) {
        player.SetActive(false);
        player.transform.position = deadPlayerLocation.transform.position;
        CheckGameOver();
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }

    public void RemovePlayer(int index) {
        GameObject p = players[index];
        players.RemoveAt(index);
        Destroy(p);
        if (EventActivePlayersChange != null) { EventActivePlayersChange.Invoke(GetActivePlayers()); }
    }
    private void CheckGameOver() {
        foreach (GameObject player in players) {
            if (!player.GetComponent<PlayerHealth>().GetIsDead()) {
                return;
            }
        }
        SceneManager.LoadScene(GameOverScene);
    }
}
