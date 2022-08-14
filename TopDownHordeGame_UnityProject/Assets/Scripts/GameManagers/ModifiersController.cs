using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ModifierType {
    fanClub, zapp
}

public class ModifiersController : NetworkBehaviour
{
    public List<RandomChoice> fanClubList;

    private List<GameObject> players;

    // --- Modifiers ---
    [Server]
    public void ApplyFanClub() {
        Debug.Log("MODIFIER: Fan Club");
        RoundController.instance.zombieList = fanClubList;
    }
    [Server]
    public void ApplyZapp() {
        foreach (GameObject player in players) {
            player.GetComponent<PlayerWeaponControl>().EventOwnedWeaponsChange += ZappOnWeaponsChange;
        }
        Debug.Log("MODIFIER: Zapp");
    }
    private void ZappOnWeaponsChange(List<Weapon> oldWeapon, List<Weapon> newWeapons) {
        foreach (Weapon weapon in newWeapons) {
            if (weapon.gameObject.HasComponent<ElectricfyWeapon>()) {
                weapon.gameObject.GetComponent<ElectricfyWeapon>().active = true;
            }
        }
    }


    /// <summary> Reads and applys modifiers from GameSettigns </summary>
    public void ApplyModifiers() {
        //Debug.Log("Applying modifiers ...");
        players = PlayerManager.instance.GetAllPlayers();
        foreach (ModifierType mod in System.Enum.GetValues(typeof(ModifierType))) {
            if (GameSettings.instance.ModActive(mod)) {
                switch (mod) {
                    case ModifierType.fanClub:
                        ApplyFanClub();
                        break;
                    case ModifierType.zapp:
                        ApplyZapp();
                        break;
                    default:
                        Debug.LogWarning("Modifer: " + mod.ToString() + " has no implementation!");
                        break;
                }
            }
        }
    }

    private void Start() {
        if (isServer) {
            MyNetworkManager.instance.ServerEvent_AllClientsReady += ApplyModifiers;
            if (MyNetworkManager.instance.AllClientsReady())
                ApplyModifiers();
        }
    }
    private void OnDestroy() {
        if (isServer)
            MyNetworkManager.instance.ServerEvent_AllClientsReady -= ApplyModifiers;
    }
}
