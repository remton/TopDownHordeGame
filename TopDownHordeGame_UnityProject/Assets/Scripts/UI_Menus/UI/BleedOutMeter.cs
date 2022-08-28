using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BleedOutMeter : MonoBehaviour
{
    public string dyingText;
    public Color dyingColor;
    private bool isDying;

    public string revivePromptText;
    public Color revivePromptColor;
    private bool hasRevivePrompt;

    public string revivingText;
    public Color revivingColor;
    private bool isReviving;

    public GameObject player;
    public PlayerHealth health;
    public PlayerRevive revive;
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;
    public Text message;

    private float awakeScaleX;
    public PlayerMovement movement;
    private void Awake() {
        awakeScaleX = transform.localScale.x;
    }
    private void Start() {
        SetBleeingOut(health.GetIsBleedingOut());
        SetRevivePrompt(false);
        SetReviving(false);
    }

    private void Update() {
        //Keep the sprite facing the correct way
        if (movement.transform.localScale.x < 0)
            transform.localScale = new Vector3(-1 * awakeScaleX, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(awakeScaleX, transform.localScale.y, transform.localScale.z);

        if (isDying) {
            UpdateMeter();
        }
    }
    private void UpdateMeter() {
        float bleedOutRatio = revive.GetBleedOutTimeRatio();
        spriteRenderer.sprite = sprites[Mathf.RoundToInt(bleedOutRatio * (sprites.Count - 1))];
    }

    public void UpdateValues(bool bleedingOut, bool revivePrompt, bool reviving) {
        SetBleeingOut(bleedingOut);
        SetRevivePrompt(revivePrompt);
        SetReviving(reviving);
    }

    private void SetBleeingOut(bool b) {
        isDying = b;
        if (isDying) {
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
    }
    private void SetRevivePrompt(bool b) {
        hasRevivePrompt = b;
        if (hasRevivePrompt) {
            message.text = revivePromptText;
            message.resizeTextForBestFit = true;
            message.color = revivePromptColor;
        }
    }
    private void SetReviving(bool b) {
        isReviving = b;
        if (isReviving) {
            message.text = revivingText;
            message.resizeTextForBestFit = true;
            message.color = revivingColor;
        }
    }
}
