using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;
    public int numPlayers = 1;
    public List<InputDevice> devices = new List<InputDevice>();

    // MODIFIERS
    [System.Serializable]
    public struct ModifierState {
        public ModifierType mod;
        public bool isActive;
        public string name;
        public ModifierState(ModifierType nMod, bool nActive) {
            mod = nMod;
            isActive = nActive;
            name = mod.ToString();
        }
    }
    public List<ModifierState> modifierStates = new List<ModifierState>();

    public void SetModifier(ModifierType mod, bool active) {
        for (int i = 0; i < modifierStates.Count; i++) {
            ModifierState modState = modifierStates[i];
            if (modState.mod == mod) {
                modState.isActive = active;
                modifierStates[i] = modState;
                //Debug.Log("Set modifier: " + mod.ToString() + " to " + active.ToString());
                return;
            }
        }
        Debug.LogWarning("Failed to set modifier: " + mod.ToString() + ". modifier not found.");
    }
    public bool ModActive(ModifierType mod) {
        for (int i = 0; i < modifierStates.Count; i++) {
            ModifierState modState = modifierStates[i];
            if (modState.mod == mod) {
                return modState.isActive;
            }
        }
        return false;
    }

    public void ResetSettings() {
        numPlayers = 1;
        modifierStates.Clear();
        foreach (ModifierType mod in System.Enum.GetValues(typeof(ModifierType))) {
            modifierStates.Add(new ModifierState(mod, false));
        }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            //Debug.Log("GameSettings already has an instance, one was destoyed");
            Destroy(gameObject);
        }
        ResetSettings();
    }
}
