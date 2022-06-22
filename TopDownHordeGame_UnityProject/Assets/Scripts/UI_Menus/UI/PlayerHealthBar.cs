using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    const float HIDE_RATIO = 0.95f;
    public PlayerHealth health;
    public PlayerMovement movement;
    public GameObject sliderObj;
    public Slider slider;

    private float awakeScaleX;
    private void Awake() {
        awakeScaleX = transform.localScale.x;
        health.EventHealthChanged += UpdateValue;
    }

    private void Update() {
        if (movement.transform.localScale.x < 0)
            transform.localScale = new Vector3(-1 * awakeScaleX, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(awakeScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void UpdateValue(float newHealth, float newMax) {
        float healthRatio = newHealth/newMax;
        //Debug.Log("Health Ratio: " + healthRatio);
        if (healthRatio >= HIDE_RATIO) {
            sliderObj.SetActive(false);
        }
        else {
            sliderObj.SetActive(true);
            slider.value = healthRatio;
        }
    }
}
