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
        public bool defaultUnlock;
        public bool active;
        public Image image;
        public Button button;
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
    public override void Open() {
        base.Open();
        foreach (var modOption in modOptions) {
            if (modOption.defaultUnlock)
                SaveData.instance.modifier_unlocks[(int)modOption.mod] = true;
            if (SaveData.instance.modifier_unlocks[(int)modOption.mod])
                EnableOption(modOption);
            else
                DisableOption(modOption);
        }
    }

    public override void Close() {
        base.Close();
        gameOptionsMenu.UpdateModifierList();
    }

    private void EnableOption(ModOption option) {
        option.button.enabled = true;
        option.button.gameObject.SetActive(true);
    }
    private void DisableOption(ModOption option) {
        option.active = false;
        GameSettings.instance.SetModifier(option.mod, option.active);
        option.button.enabled = false;
        option.button.gameObject.SetActive(false);
    }
}
