using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModifierMenu : Menu
{
    public GameOptionsMenu gameOptionsMenu;

    [System.Serializable]
    public class ModOption {
        public ModifierType mod;
        public string name;
        [TextArea()]
        public string description;
        public bool defaultUnlock;
        [HideInInspector]
        public ModifierItem itemUI;
        public bool active;
    }

    [SerializeField]
    private List<ModOption> modifiers = new List<ModOption>();

    public GameObject modItemHolder;
    public GameObject modItemPrefab;
    private List<GameObject> modItems = new List<GameObject>();

    public void ToggleModifier(ModifierType mod) {
        for (int i = 0; i < modifiers.Count; i++) {
            if(!base.isUsingGamepad)
                EventSystem.current.SetSelectedGameObject(null);

            ModOption option = modifiers[i];
            if (mod == option.mod) {
                if (modifiers[i].itemUI == null) {
                    Debug.LogError("Failed to find modifier ui item for: " + mod);
                    return;
                }
                option.active = !option.active;
                modifiers[i] = option;
                GameSettings.instance.SetModifier(mod, option.active);
                if (option.active)
                    option.itemUI.Activate();
                else
                    option.itemUI.Deactivate();
                return;
            }
        }
    }
    public override void Open() {
        base.Open();
        foreach (var item in modItems) {
            Destroy(item);
        }
        modItems.Clear();

        foreach (var modifier in modifiers) {
            Debug.Log("Modifier " + modifier.name + " was detected.");
            if (modifier.defaultUnlock)
                SaveData.instance.modifier_unlocks[(int)modifier.mod] = true;
            if (SaveData.instance.modifier_unlocks[(int)modifier.mod]) {
                GameObject item = Instantiate(modItemPrefab, modItemHolder.transform);
                modifier.itemUI = item.GetComponent<ModifierItem>();
                item.GetComponent<ModifierItem>().Init(modifier);
                item.GetComponent<ModifierItem>().EventButtonPressed += ToggleModifier;

                if (modifier.active)
                    item.GetComponent<ModifierItem>().Activate();
                else
                    item.GetComponent<ModifierItem>().Deactivate();
                GameSettings.instance.SetModifier(modifier.mod, modifier.active);
                modItems.Add(item);
            }
        }
    }

    public override void Close() {
        base.Close();
        gameOptionsMenu.UpdateModifierList();
    }
}
