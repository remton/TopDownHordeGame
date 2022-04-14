using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaMeter : MonoBehaviour 
    {
    const float HIDE_RATIO = 0.95f;
    public PlayerMovement movement;
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;
    private void Awake() {
        movement.EventOrientationChange += UpdateOrientation;
        movement.EventStaminaChange += UpdateValue;
    }

    private void UpdateOrientation(bool xIsNegative) {
        if (xIsNegative != (transform.localScale.x < 0)) {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void UpdateValue(float newStamina, float newMax) {
        float staminaRatio = newStamina / newMax;
        if (staminaRatio >= HIDE_RATIO) {
            spriteRenderer.enabled = false;
        }
        else {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = sprites[Mathf.RoundToInt(staminaRatio * (sprites.Count - 1))];
        }
    }
}
