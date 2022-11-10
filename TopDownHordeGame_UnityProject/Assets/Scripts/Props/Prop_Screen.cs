using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prop_Screen : Prop
{
    public Text screenTxt;

    public void SetText(string text) {
        screenTxt.text = text;
    }
    public void SetTextColor(Color textColor) {
        screenTxt.color = textColor;
    }
}
