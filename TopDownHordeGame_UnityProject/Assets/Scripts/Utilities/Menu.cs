using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Menu parentMenu;
    public GameObject defaultSelectedObject;
    private bool isUsingGamepad = false;

    protected virtual void Update() {
        //handle the cancel and back button
        InputSystemUIInputModule inputUI = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        bool cancel = inputUI.cancel.action.triggered;
        if (cancel) {
            OnCancel();
        }

        //If we click or we dont have a gamepad we are using a keyboard to navigate
        if (Gamepad.current == null)
            OnUseKeyboard();
        if (Mouse.current != null && Mouse.current.leftButton.IsPressed()) {
            OnUseKeyboard();
        }
        if (!isUsingGamepad && Gamepad.current != null) {
            //All the buttons that activate the gamepad
            bool wantsToUse = Gamepad.current.dpad.IsPressed() ||
                Gamepad.current.leftStick.IsPressed() ||
                Gamepad.current.rightStick.IsPressed() ||
                Gamepad.current.buttonSouth.IsPressed() ||
                Gamepad.current.buttonNorth.IsPressed() ||
                Gamepad.current.buttonEast.IsPressed() ||
                Gamepad.current.buttonWest.IsPressed() ||
                Gamepad.current.startButton.IsPressed() ||
                Gamepad.current.selectButton.IsPressed();
            if (wantsToUse)
                OnUseGamepad();
        }
    }

    public virtual void Open() {
        if (parentMenu != null) {
            parentMenu.SetInteractable(false);
        }
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
    }
    public virtual void Close() {
        if (parentMenu != null) {
            parentMenu.SetInteractable(true);
        }
        gameObject.SetActive(false);
        if(parentMenu!=null)
            EventSystem.current.SetSelectedGameObject(parentMenu.defaultSelectedObject);
    }
    public virtual void SetInteractable(bool interactable) {
        Selectable[] selects = gameObject.GetComponentsInChildren<Selectable>();
        foreach (Selectable select in selects) {
            select.interactable = interactable;
        }
    }

    protected virtual void OnUseGamepad() {
        isUsingGamepad = true;
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
    }
    protected virtual void OnUseKeyboard() {
        isUsingGamepad = false;
    }

    public virtual void OnCancel() {
        Close();
    }
}
