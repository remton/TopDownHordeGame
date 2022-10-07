using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ModifierType {
    allBasic, allBiggestFan, allHockEye, allLungs, allSplitter, allZathrak, 
    safetyOff, zapp, studentLoans, bloodBullets
}

public class ModifiersController : NetworkBehaviour
{
    public GameObject allBasic_Prefab;
    public GameObject allBiggestFan_Prefab;
    public GameObject allHockEye_Prefab;
    public GameObject allLungs_Prefab;
    public GameObject allSplitter_Prefab;
    public GameObject allZathrak_Prefab;

    private List<RandomChoice> zombieListReplacement = new List<RandomChoice>();
    private bool replaceZombieList = false;

    private List<GameObject> players;

    // --- Apply Modifiers ---
    #region zombie spawn modifiers
    [Server]
    public void Apply_AllBasic() {
        zombieListReplacement.Add(new RandomChoice(1, allBasic_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Basic");
    }
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
    public void Apply_AllLungs() {
        zombieListReplacement.Add(new RandomChoice(1, allLungs_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Lungs");
    }
    [Server]
    public void Apply_AllSplitter() {
        zombieListReplacement.Add(new RandomChoice(1, allSplitter_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Splitter");
    }

    [Server]
    public void Apply_AllZathrak() {
        zombieListReplacement.Add(new RandomChoice(1, allZathrak_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Zathrak");
    }
    #endregion

    [Server]
    public void Apply_SaftyOff() {
        Debug.Log("MODIFIER: Safty Off : (NYI)");
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

    //How the amount lost is determined
    int StudentLoans_GetAmountLost(int round) {
        if (round <= 1)
            return 0;
        return (round - 1) * 500;
    }
    [Server]
    public void Apply_StudentLoans() 
    {
        // change this to change the starting amount
        int startingAmount = 5000; 
        Debug.Log("MODIFIER: Student Loans");
        foreach (GameObject player in players)
        {
            Debug.Log("Loaned money to " + player.GetComponent<PlayerStats>().GetName());
            player.GetComponent<PlayerStats>().AddMoney(startingAmount);
            MoneyEffectManager.instance.CreateEffect(player, player.transform.position, startingAmount);
        }
        RoundController.instance.EventRoundChange += StudentLoans_OnRoundChange;
    }
    [Server]
    private void StudentLoans_OnRoundChange(int round) {
        if (round == RoundController.instance.startRound)
            return;

        foreach (GameObject player in players) {
            Debug.Log("Player " + player.GetComponent<PlayerStats>().GetName() + " lost $" + StudentLoans_GetAmountLost(round));
            player.GetComponent<PlayerStats>().SpendMoney(StudentLoans_GetAmountLost(RoundController.instance.round) * -1);
        }
    }


    [Server]
    public void Apply_BloodBullets()
    {
        foreach (PlayerConnection pconn in MyNetworkManager.instance.GetPlayerConnections()) {
            Apply_BloodBulletsTRPC(pconn.connectionToClient);
        }
        Debug.Log("MODIFIER: Blood Bullets");
    }
    [TargetRpc]
    public void Apply_BloodBulletsTRPC(NetworkConnection conn) {
        foreach (GameObject player in PlayerConnection.myConnection.GetPlayerCharacters()) {
            //Debug.Log("BLOOD BULLETS FOR PLAYER: " + player.name);
            player.GetComponent<PlayerWeaponControl>().EventOwnedWeaponsChange += BloodBulletsOnWeaponsChange;
            BloodBulletsOnWeaponsChange(new List<Weapon>(), player.GetComponent<PlayerWeaponControl>().GetWeapons());
        }
        //Debug.Log("BLOOD BULLETS: Applied");
    }
    private void BloodBulletsOnWeaponsChange(List<Weapon> oldWeapon, List<Weapon> newWeapons)
    {
        foreach (Weapon weapon in newWeapons)
        {
            //Debug.Log("BLOOD BULLETS: OnWeaponsChange ran");
            weapon.SetBloodBullets(true);
        }
    }

    /// <summary> Reads and applys modifiers from GameSettigns </summary>
    public void ApplyModifiers() {
        //Debug.Log("Applying modifiers ...");
        players = PlayerManager.instance.GetAllPlayers();
        foreach (ModifierType mod in System.Enum.GetValues(typeof(ModifierType))) {
            if (GameSettings.instance.ModActive(mod)) {
                switch (mod) {
                    case ModifierType.allBasic:
                        Apply_AllBasic();
                        break;
                    case ModifierType.allBiggestFan:
                        Apply_AllBiggest();
                        break;
                    case ModifierType.allHockEye:
                        Apply_AllHockEye();
                        break;
                    case ModifierType.allLungs:
                        Apply_AllLungs();
                        break;
                    case ModifierType.allSplitter:
                        Apply_AllSplitter();
                        break;
                    case ModifierType.allZathrak:
                        Apply_AllZathrak();
                        break;
                    case ModifierType.safetyOff:
                        Apply_SaftyOff();
                        break;
                    case ModifierType.zapp:
                        Apply_Zapp();
                        break;
                    case ModifierType.studentLoans:
                        Apply_StudentLoans();
                        break;
                    case ModifierType.bloodBullets:
                        Apply_BloodBullets();
                        break;
                    default:
                        Debug.LogWarning("Modifer: " + mod.ToString() + " has no implementation!");
                        break;
                }
            }
        }
        if (replaceZombieList)
            RoundController.instance.zombieList = zombieListReplacement;
    }

    private void OnClientsLoaded() {
        StartCoroutine(SetPlayers());
    }
    IEnumerator SetPlayers() {
        if (!isServer)
            yield break;
        //wait unitl all charcters are spawned
        yield return new WaitUntil(MyNetworkManager.instance.AllPlayerCharactersSpawned);
        //ApplyModifiers
        ApplyModifiers();
    }

    private void Start() {
        if (isServer) {
            MyNetworkManager.instance.ServerEvent_AllClientsReady += OnClientsLoaded;
            if (MyNetworkManager.instance.AllClientsReady())
                OnClientsLoaded();
        }
    }
    private void OnDestroy() {
        if (isServer)
            MyNetworkManager.instance.ServerEvent_AllClientsReady -= OnClientsLoaded;
    }
}
