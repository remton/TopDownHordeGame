using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpMenu : MonoBehaviour
{
    GameObject currOverlay;
    public void SetOverlay(GameObject overlay) {
        overlay.SetActive(true);
        currOverlay = overlay;
    }
    public void DisableOverlay() {
        currOverlay.SetActive(false);
    }
    public void LoadMainMenu() {
        //MainMenu scene should be index 0
        SceneManager.LoadScene(4);
    }
}
