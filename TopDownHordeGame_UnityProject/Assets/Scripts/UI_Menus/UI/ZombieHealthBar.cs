using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour 
{
    const float HIDE_RATIO = 0.95f;
    public ZombieAI zAI;
    public ZombieHealth health;
    public GameObject sliderObj;
    public Slider slider;

    private void Awake() {
        health.EventHealthChanged += UpdateValue;
        zAI.EventOrientationChange += UpdateOrientation;
    }

    private void UpdateOrientation(bool xIsNegative) {
        if (xIsNegative != (transform.localScale.x < 0)) {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void UpdateValue(float newHealth, float newMax) {
        float healthRatio = newHealth / newMax;
        if (healthRatio >= HIDE_RATIO) {
            sliderObj.SetActive(false);
        }
        else {
            sliderObj.SetActive(true);
            slider.value = healthRatio;
        }
    }
}
