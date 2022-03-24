using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReticleController : MonoBehaviour
{
    private GameObject playerWithMouse;
    private GameObject mouseReticle;
    private List<GameObject> gamepadPlayers = new List<GameObject>();
    private List<GameObject> gamepadReticles = new List<GameObject>();
    public GameObject reticlePrefab;

    //used with gamepad for how far to display the reticle
    public float radius;

    private void Start() {
        PlayerManager.instance.EventActivePlayersChange += OnPlayersChanged;
        PauseManager.instance.EventPauseStateChange += OnPauseChange;
        OnPlayersChanged(PlayerManager.instance.GetActivePlayers());
    }
    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= OnPlayersChanged;
        PauseManager.instance.EventPauseStateChange -= OnPauseChange;
        Cursor.visible = true;
    }

    public void OnPauseChange(bool isPaused) {
        if (isPaused) {
            HideReticles();
            Cursor.visible = true;
        }
        else {
            UnHideReticles();
            Cursor.visible = false;
        }
    }

    private void HideReticles() {
        if (mouseReticle != null) {
            mouseReticle.SetActive(false);
        }
        foreach (GameObject reticle in gamepadReticles) {
            reticle.SetActive(false);
        }
    }

    private void UnHideReticles() {
        if (mouseReticle != null) {
            mouseReticle.SetActive(true);
        }
        foreach (GameObject reticle in gamepadReticles) {
            reticle.SetActive(true);
        }
    }

    void OnPlayersChanged(List<GameObject> newPlayers) {
        Destroy(mouseReticle);
//        Debug.Log("Reticle OnPlayersChanged was called.");
        mouseReticle = null;
        playerWithMouse = null;
        foreach (GameObject reticle in gamepadReticles) {
            Destroy(reticle);
        }
        gamepadPlayers.Clear();
        gamepadReticles.Clear();

        //Add reticles
        for (int i = 0; i < newPlayers.Count; i++) {
            if (newPlayers[i].GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
                gamepadPlayers.Add(newPlayers[i]);
                GameObject newRet = Instantiate(reticlePrefab, transform);
                newRet.GetComponent<Reticle>().player = newPlayers[i];
                gamepadReticles.Add(newRet);
            }
            else {
                // Using mouse
                playerWithMouse = newPlayers[i];
                mouseReticle = Instantiate(reticlePrefab, transform);
                mouseReticle.GetComponent<Reticle>().player = newPlayers[i];
                Cursor.visible = false;
            }
        }
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

 
}
