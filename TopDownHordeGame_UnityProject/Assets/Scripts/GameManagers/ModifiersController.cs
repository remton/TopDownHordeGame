using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ModifierType {
    allBiggestFan, allHockEye, allZathrak, allLungs, zapp
}

public class ModifiersController : NetworkBehaviour
{
    public GameObject allBiggestFan_Prefab;
    public GameObject allHockEye_Prefab;
    public GameObject allZathrak_Prefab;
    public GameObject allLungs_Prefab;

    private List<RandomChoice> zombieListReplacement = new List<RandomChoice>();
    private bool replaceZombieList = false;

    private List<GameObject> players;

    // --- Apply Modifiers ---
    [Server]
    public void Apply_AllBiggest() {
        zombieListReplacement.Add(new RandomChoice(1, allBiggestFan_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Biggest Fan");
    }

    [Server] 
    public void Apply_AllHockEye() {
        zombieListReplacement.Add(new RandomChoice(1, allHockEye_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Hock Eye");
    }

    [Server]
    public void Apply_AllZathrak() {
        zombieListReplacement.Add(new RandomChoice(1, allZathrak_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Zathrak");
    }

    [Server]
    public void Apply_AllLungs() {
        zombieListReplacement.Add(new RandomChoice(1, allLungs_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Lungs");
    }

    [Server]
    public void Apply_Zapp() {
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
                    case ModifierType.allBiggestFan:
                        Apply_AllBiggest();
                        break;
                    case ModifierType.allHockEye:
                        Apply_AllHockEye();
                        break;
                    case ModifierType.allZathrak:
                        Apply_AllZathrak();
                        break;
                    case ModifierType.allLungs:
                        Apply_AllLungs();
                        break;
                    case ModifierType.zapp:
                        Apply_Zapp();
                        break;
                    default:
                        Debug.LogWarning("Modifer: " + mod.ToString() + " has no implementation!");
                        break;
                }
            }
        }
        if(replaceZombieList)
            RoundController.instance.zombieList = zombieListReplacement;
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
