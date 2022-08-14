using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundDisplay : MonoBehaviour
{
    public GameObject popup;

    [SerializeField] private float displayTime;
    private bool isDisplayingPopup;
    private float timeUntilEndPopup;
    private int round;

    public void RoundChange(int r) {
        timeUntilEndPopup = displayTime;
        round = r;
        StartPopup();
    }
    private void StartPopup() {
        popup.SetActive(true);
        popup.GetComponentInChildren<Text>().text = "Round " + round.ToString();
        isDisplayingPopup = true;
    }
    private void EndPopup() {
        popup.SetActive(false);
        isDisplayingPopup = false;
    }
    public bool GetIsDisplayingPopup()
    {
        return isDisplayingPopup;
    }
    private void Update() {
        if (isDisplayingPopup) {
            if(timeUntilEndPopup <= 0) {
                EndPopup();
            }
            timeUntilEndPopup -= Time.deltaTime;
        }
    }

}
