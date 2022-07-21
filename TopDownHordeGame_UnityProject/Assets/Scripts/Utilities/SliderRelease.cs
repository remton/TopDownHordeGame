using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class SliderRelease : MonoBehaviour, IPointerUpHandler
{
    public delegate void Release(float value);
    public event Release EventOnRelease;

    private Slider slider;
    private float oldValue;

    void Start() {
        slider = GetComponent<Slider>();
        oldValue = slider.value;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (slider.value != oldValue) {
            oldValue = slider.value;
            if (EventOnRelease != null) { EventOnRelease.Invoke(slider.value); }
        }
    }
}
