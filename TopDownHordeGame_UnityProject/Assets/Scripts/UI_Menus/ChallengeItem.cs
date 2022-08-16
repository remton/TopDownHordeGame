using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeItem : MonoBehaviour
{
    public Image back;
    public Image icon;
    public Color selectedColor;
    public Color deselectedColor;

    private Challenge challenge;
    private GameObject infoBox;
    private bool isInitialized = false;
    public void Init(Challenge challenge, GameObject infoBox) {
        this.challenge = challenge;
        this.infoBox = infoBox;
        back.color = deselectedColor;
        icon.sprite = challenge.icon;
        isInitialized = true;
    }

    public void OnSelect() {
        back.color = selectedColor;
        infoBox.GetComponent<ChallengeInfobox>().SetChallenge(challenge);
        infoBox.GetComponent<ChallengeInfobox>().SetLocation(transform.position);
        infoBox.SetActive(true);
    }
    public void OnDeselect() {
        back.color = deselectedColor;
        infoBox.SetActive(false);
    }
}
