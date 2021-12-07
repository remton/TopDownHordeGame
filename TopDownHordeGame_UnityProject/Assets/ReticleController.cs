using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReticleController : MonoBehaviour
{
    private bool useGamePad;
    private GameObject playerWithMouse;
    private GameObject mouseReticle;
    private List<GameObject> gamepadPlayers = new List<GameObject>();
    private List<GameObject> gamepadReticles = new List<GameObject>();
    public GameObject reticlePrefab;

    //used with gamepad for how far to display the reticle
    public float radius;

    void OnPlayersChanged(List<GameObject> newPlayers) {
        Destroy(mouseReticle);
        mouseReticle = null;
        playerWithMouse = null;
        foreach (GameObject reticle in gamepadReticles) {
            Destroy(reticle);
        }
        gamepadPlayers.Clear();
        gamepadReticles.Clear();


        for (int i = 0; i < newPlayers.Count; i++) {
            if (newPlayers[i].GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
                gamepadPlayers.Add(newPlayers[i]);
                gamepadReticles.Add(Instantiate(reticlePrefab, transform));
            }
            else {
                // Using mouse
                playerWithMouse = newPlayers[i];
                mouseReticle = Instantiate(reticlePrefab, transform);
                Cursor.visible = false;
            }
        }
        
    }

    private void Start() {
        PlayerManager.instance.EventActivePlayersChange += OnPlayersChanged;
        OnPlayersChanged(PlayerManager.instance.GetActivePlayers());
    }

    private void Update() {
        int i = 0;
        foreach (GameObject player in gamepadPlayers) {
            Vector3 newReticlePosInWorld = player.GetComponent<PlayerMovement>().GetCurrentLookDir().normalized * radius;
            newReticlePosInWorld += player.transform.position;
            gamepadReticles[i].transform.position = newReticlePosInWorld;
            i++;
        }

        if (playerWithMouse) {
            Vector2 mouseScreenPos = playerWithMouse.GetComponent<PlayerMovement>().mouseScreenPos;
            Vector3 newReticlePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            newReticlePos.z = 0;
            mouseReticle.transform.position = newReticlePos;
        }
    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= OnPlayersChanged;
    }
}
