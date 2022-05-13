using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeypadUI : Menu
{
    private int numDisplaySize;
    public Text UINumTxt;

    public delegate void submitPressed(int[] guess);
    public event submitPressed EventSubmitPressed;
    public delegate void cancelPressed();
    public event cancelPressed EventCancelPressed;

    public List<int> currGuess = new List<int>();

    public override void OnCancel() {
        base.OnCancel();
        if (EventCancelPressed != null) { EventCancelPressed.Invoke(); }
        CloseUI();
    }

    public void Clear() {
        currGuess.Clear();
        UpdateUI(currGuess);
    }


    public void PressNum(int num) {
        currGuess.Add(num);
        UpdateUI(currGuess);
    }
    public void PressSubmit() {
        if (EventSubmitPressed != null) { EventSubmitPressed.Invoke(currGuess.ToArray()); }
    }


    private void Start() {
        numDisplaySize = UINumTxt.text.Length - 1;
        UpdateUI(currGuess);
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
    }

}
