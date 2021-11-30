using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spawnPoint;
    private List<GameObject> players = new List<GameObject>();
    private int numPlayers;

    public List<GameObject> GetPlayers() { return players; }

    private void Start() {
        CreatePlayers();
    }

    public void CreatePlayers() {
        numPlayers = GameSettings.instance.numPlayers;
        for (int i = 0; i < numPlayers; i++) {
            PlayerInput input = PlayerInput.Instantiate(playerPrefab, i);
            GameObject playerObj = input.gameObject;
            playerObj.transform.position = spawnPoint.transform.position;
            playerObj.GetComponent<PlayerInput>().camera = Camera.main;
            players.Add(playerObj);
        }
    }

    public void RemovePlayer(int index) {
        GameObject p = players[index];
        players.RemoveAt(index);
        Destroy(p);
    }
}
