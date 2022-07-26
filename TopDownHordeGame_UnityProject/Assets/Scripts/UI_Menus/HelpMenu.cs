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
    
    protected override void OnUseGamepad() {
        if(currOverlay == null && EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
        }
    }

    public override void OnCancel() {
        if (currOverlay != null)
            DisableOverlay();
        else
            Close();
    }

}
