using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifierMenu : Menu
{
    public GameOptionsMenu gameOptionsMenu;

    public List<GameObject> ChoiceButtons;
    public Color DisableChoiceColor;
    public Color EnableChoiceColor;

    private bool fanClub = false;
    public Image fanClubImage;

    public void Button_ToggleFanClub() {
        fanClub = !fanClub;
        GameSettings.instance.modifier_fanClub = fanClub;
        if (fanClub)
            fanClubImage.color = EnableChoiceColor;
        else
            fanClubImage.color = DisableChoiceColor;
    }

    public override void Close() {
        base.Close();
        gameOptionsMenu.UpdateModifierList();
    }
}
