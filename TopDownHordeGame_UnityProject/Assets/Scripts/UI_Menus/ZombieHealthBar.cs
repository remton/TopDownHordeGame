using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    public GameObject zombie;
    public ZombieHealth health;
    public GameObject sliderObj;
    public Slider slider;
    Vector3 offset;

    private void Start() {
        offset = transform.position - zombie.transform.position;
        transform.parent = null;
        health.EventOnDeath += ZombieIsDead;
    }
    private void OnDestroy() {
        health.EventOnDeath -= ZombieIsDead;
    }
    private void ZombieIsDead() {
        Destroy(gameObject);
    }

    private void Update() {
        
        float healthRatio = health.GetHealthRatio();
        if (healthRatio >= 1) {
            sliderObj.SetActive(false);
        }
        else {
            transform.position = zombie.transform.position + offset;
            sliderObj.SetActive(true);
            slider.value = healthRatio;
        }
    }
}
