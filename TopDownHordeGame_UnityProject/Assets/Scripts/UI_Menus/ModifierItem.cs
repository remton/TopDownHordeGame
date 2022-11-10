using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifierItem : MonoBehaviour
{

    public Text nameTxt;
    public Text descriptionTxt;
    public Image image;

    public Color disableChoiceColor;
    public Color enableChoiceColor;

    public delegate void ButtonPressed(ModifierType type);
    public event ButtonPressed EventButtonPressed;

    private ModifierType mod;

    public void Init(ModifierMenu.ModOption modOption) {
        mod = modOption.mod;
        nameTxt.text = modOption.name;
        descriptionTxt.text = modOption.description;
    }

    public void Activate() {
        image.color = enableChoiceColor;
    }
    public void Deactivate() {
        image.color = disableChoiceColor;
    }

    public void ButtonPress() {
        if (EventButtonPressed != null) { EventButtonPressed.Invoke(mod); }
    }
}
