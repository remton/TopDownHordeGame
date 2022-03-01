using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevivePrompt : MonoBehaviour
{
    public GameObject popupCanvas;
    public GameObject player;
    Vector3 offset;

    private void Start() {
        offset = transform.position - player.transform.position;
        transform.parent = null;
    }

    public void Activate() {
        popupCanvas.SetActive(true);
    }
    public void Deactivate() {
        popupCanvas.SetActive(false);
    }
}
