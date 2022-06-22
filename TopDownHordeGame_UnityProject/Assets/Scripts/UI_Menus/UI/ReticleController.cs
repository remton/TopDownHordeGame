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
    private List<GameObject> players = new List<GameObject>();
    public GameObject reticlePrefab;


    //used with gamepad for how far to display the reticle
    public float radius;

    private void Start() {
        PlayerManager.instance.EventActiveLocalPlayersChange += OnPlayersChanged;
        PauseManager.instance.EventPauseStateChange += OnPauseChange;
        OnPlayersChanged(PlayerManager.instance.GetActiveLocalPlayers());
    }
    private void OnDestroy() {
        PlayerManager.instance.EventActiveLocalPlayersChange -= OnPlayersChanged;
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
        UnsubscribePlayerEvents(players);
        SubscribePlayerEvents(newPlayers);
        players = newPlayers;

        Destroy(mouseReticle);
        // Debug.Log("Reticle OnPlayersChanged was called.");
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
                newRet.GetComponent<Reticle>().Init(newPlayers[i]);
                gamepadReticles.Add(newRet);
            }
            else {
                // Using mouse
                playerWithMouse = newPlayers[i];
                mouseReticle = Instantiate(reticlePrefab, transform);
                mouseReticle.GetComponent<Reticle>().Init(newPlayers[i]);
                Cursor.visible = false;
            }
        }
    }
    
    public void UpdatedControls(string controlScheme) {
        Debug.Log("Controls changed to: " + controlScheme);
        OnPlayersChanged(PlayerManager.instance.GetActiveLocalPlayers());
    }

    private void SubscribePlayerEvents(List<GameObject> playerList) {
        foreach (var player in playerList) {
            if(player != null && player.HasComponent<Player>())
                player.GetComponent<Player>().EventControlsChanged += UpdatedControls;
        }
    }
    private void UnsubscribePlayerEvents(List<GameObject> playerList) {
        foreach (var player in playerList) {
            if (player != null && player.HasComponent<Player>())
                player.GetComponent<Player>().EventControlsChanged -= UpdatedControls;
        }
    }

    private void Update() {
        int i = 0;
        foreach (GameObject player in gamepadPlayers) {
            gamepadReticles[i].GetComponent<Reticle>().Activate(player.GetComponent<PlayerWeaponControl>().NeedReticle());
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
