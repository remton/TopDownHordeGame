using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class HelpMenu : Menu
{
    [Tooltip("Should be parallel to other list")]
    public List<GameObject> overlays;
    [Tooltip("Should be parallel to other list")]
    public List<GameObject> buttons;
    
    GameObject currOverlay;
    GameObject currButton;

    public GameObject defaultSelectedObject;

    public void SetOverlay(GameObject overlay) {
        int index = 0;
        for (int i = 0; i < overlays.Count; i++) {
            if (overlay == overlays[i])
                index = i;
        }
        
        overlay.SetActive(true);
        currOverlay = overlay;
        currButton = buttons[index];
    }
    public void DisableOverlay() {
        currOverlay.SetActive(false);
        EventSystem.current.SetSelectedGameObject(currButton);
        currOverlay = null;
        currButton = null;
    }
    public void LoadMainMenu() {
        //MainMenu scene should be index 0
        SceneManager.LoadScene("MainMenu");
    }

    
    protected override void OnUseGamepad() {
        if(currOverlay == null && EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
        }
    }
    protected override void OnUseKeyboard() {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public override void OnCancel() {
        if (currOverlay != null)
            DisableOverlay();
        else
            LoadMainMenu();
    }

}
