using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifierMenu : Menu
{
    public GameOptionsMenu gameOptionsMenu;

    public Color DisableChoiceColor;
    public Color EnableChoiceColor;

    [System.Serializable]
    private struct ModOption {
        public ModifierType mod;
        public bool active;
        public Image image;
    }

    [SerializeField]
    private List<ModOption> modOptions = new List<ModOption>();

    public void Button_ToggleModifier(string modifierName) {
        ModifierType mod;
        if(System.Enum.TryParse<ModifierType>(modifierName, out mod)) {
            for (int i = 0; i < modOptions.Count; i++) {
                if (mod == modOptions[i].mod) {
                    ModOption option = modOptions[i];
                    option.active = !option.active;
                    modOptions[i] = option;
                    GameSettings.instance.SetModifier(mod, option.active);
                    if (option.active)
                        option.image.color = EnableChoiceColor;
                    else
                        option.image.color = DisableChoiceColor;
                    return;
                }
            }
        }
        else {
            Debug.LogError("Failed to find ModifierType: \"" + modifierName + "\". Check spelling and that it is added to ModifierType enum.");
        }
    }
    public override void Close() {
        base.Close();
        gameOptionsMenu.UpdateModifierList();
    }
}
