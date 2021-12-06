using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReticleController : MonoBehaviour
{
    private bool useGamePad;
    public List<GameObject> gamepadPlayers = new List<GameObject>();
    public List<GameObject> reticles = new List<GameObject>();
    public GameObject reticlePrefab;
    public Texture2D cursorReplacor;
    public GameObject Canvas;

    //used with gamepad for how far to display the reticle
    public float radius;

    void OnPlayersChanged(List<GameObject> newPlayers) {
        gamepadPlayers.Clear();
        foreach (GameObject reticle in reticles) {
            Destroy(reticle);
        }
        for (int i = 0; i < newPlayers.Count; i++) {
            if (newPlayers[i].GetComponent<PlayerInput>().currentControlScheme == "Gamepad") {
                gamepadPlayers.Add(newPlayers[i]);
                reticles.Add(Instantiate(reticlePrefab, Canvas.transform));
            }
        }
    }

    private void Awake() {
        Cursor.SetCursor(cursorReplacor, Vector2.zero, CursorMode.Auto);
    }

    private void Start() {
        PlayerManager.instance.EventActivePlayersChange += OnPlayersChanged;
    }

    private void Update() {
        int i = 0;
        foreach (GameObject player in gamepadPlayers) {
            Vector3 newReticlePosInWorld = player.GetComponent<PlayerMovement>().GetCurrentLookDir().normalized * radius;
            newReticlePosInWorld += player.transform.position;
            reticles[i].transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, newReticlePosInWorld);
            i++;
        }
    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= OnPlayersChanged;
    }
}
