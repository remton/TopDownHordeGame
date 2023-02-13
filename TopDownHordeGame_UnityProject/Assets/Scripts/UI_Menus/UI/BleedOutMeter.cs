using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BleedOutMeter : MonoBehaviour
{
    public string dyingText;
    public Color dyingColor;
    private bool isBleedingOut;

    public string revivePromptText;
    public Color revivePromptColor;
    private bool hasRevivePrompt;

    public string revivingText;
    public Color revivingColor;
    private bool isReviving;

    public GameObject player;
    public PlayerBleedout bleedout;
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;
    public Text message;

    private float awakeScaleX;
    public PlayerMovement movement;
    private void Awake() {
        awakeScaleX = transform.localScale.x;
    }
    private void Start() {
        SetBleeingOut(bleedout.isBleedingOut);
        SetRevivePrompt(false);
        SetReviving(false);
    }

    private void Update() {
        //Keep the sprite facing the correct way
        if (movement.transform.localScale.x < 0)
            transform.localScale = new Vector3(-1 * awakeScaleX, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(awakeScaleX, transform.localScale.y, transform.localScale.z);

        if (isBleedingOut) {
            UpdateMeter();
        }
    }
    private void UpdateMeter() {
        float bleedOutRatio = bleedout.bleedoutRatio;
        spriteRenderer.sprite = sprites[Mathf.RoundToInt(bleedOutRatio * (sprites.Count - 1))];
    }

    public void SetBleeingOut(bool b) {
        Debug.Log("BleedOutMeter: bleeding out " + b);
        isBleedingOut = b;
        UpdateUI();
    }
    public void SetRevivePrompt(bool b) {
        Debug.Log("BleedOutMeter: revive prompt " + b);
        hasRevivePrompt = b;
        UpdateUI();
    }
    public void SetReviving(bool b) {
        Debug.Log("BleedOutMeter: reviving " + b);
        isReviving = b;
        UpdateUI();
    }
    private void UpdateUI() {
        if (isBleedingOut) {
            spriteRenderer.enabled = true;
            message.gameObject.SetActive(true);
            message.text = dyingText;
            message.resizeTextForBestFit = true;
            message.color = dyingColor;
        }
        else {
            spriteRenderer.enabled = false;
            message.gameObject.SetActive(false);
        }

        if (hasRevivePrompt) {
            message.text = revivePromptText;
            message.resizeTextForBestFit = true;
            message.color = revivePromptColor;
        }

        if (isReviving) {
            message.text = revivingText;
            message.resizeTextForBestFit = true;
            message.color = revivingColor;
        }
    }
}
