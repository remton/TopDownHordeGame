using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public GameObject player;
    public PlayerHealth health;
    public GameObject sliderObj;
    public Slider slider;
    Vector3 offset;

    private void Start() {
        offset = transform.position - player.transform.position;
        transform.parent = null;
    }

    private void Update() {
        float healthRatio = health.GetHealthRatio();
        if (healthRatio >= 1) {
            sliderObj.SetActive(false);
        }
        else {
            transform.position = player.transform.position + offset;
            sliderObj.SetActive(true);
            slider.value = healthRatio;
        }
    }
}
