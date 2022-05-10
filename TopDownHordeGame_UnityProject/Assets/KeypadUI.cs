using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeypadUI : Menu
{
    private int numDisplaySize;
    public Text UINumTxt;

    public delegate void numPressed(int num);
    public event numPressed EventNumPressed;
    public delegate void submitPressed();
    public event submitPressed EventSubmitPressed;
    public delegate void cancelPressed();
    public event cancelPressed EventCancelPressed;

    public override void OnCancel() {
        base.OnCancel();
        if (EventCancelPressed != null) { EventCancelPressed.Invoke(); }
        CloseUI();
    }

    public void PressNum(int num) {
        if(EventNumPressed != null) { EventNumPressed.Invoke(num); }
    }
    public void PressSubmit() {
        if (EventSubmitPressed != null) { EventSubmitPressed.Invoke(); }
    }


    private void Start() {
        numDisplaySize = UINumTxt.text.Length - 1;
    }
    public void OpenUI() {
        gameObject.SetActive(true);
        PauseManager.instance.PauseTime();
    }
    public void CloseUI() {
        gameObject.SetActive(false);
        PauseManager.instance.UnpauseTime();
    }
    public void UpdateUI(List<int> numPressed) {
        string newText = "";
        foreach (int num in numPressed) {
            newText += num.ToString();
        }
        for (int i = numPressed.Count; i < numDisplaySize; i++) {
            newText += " ";
        }
        UINumTxt.text = newText;
        Debug.Log("text updated!!");
    }

}
