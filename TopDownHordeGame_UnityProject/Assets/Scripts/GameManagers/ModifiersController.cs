using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum ModifierType {
    allBasic, allBiggestFan, allHockEye, allLungs, allSplitter, allZathrak, 
    safetyOff, zapp, studentLoans, bloodBullets, csws, pingPong
}

[RequireComponent(typeof(Timer))]
public class ModifiersController : NetworkBehaviour
{
    public GameObject allBasic_Prefab;
    public GameObject allBiggestFan_Prefab;
    public GameObject allHockEye_Prefab;
    public GameObject allLungs_Prefab;
    public GameObject allSplitter_Prefab;
    public GameObject allZathrak_Prefab;

    private List<RandomChoice<GameObject>> zombieListReplacement = new List<RandomChoice<GameObject>>();
    private bool replaceZombieList = false;
    private Timer timer;
    private List<GameObject> players;

    // --- Apply Modifiers ---
    #region zombie spawn modifiers
    [Server]
    public void Apply_AllBasic() {
        zombieListReplacement.Add(new RandomChoice<GameObject>(1, allBasic_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Basic");
    }
    [Server]
    public void Apply_AllBiggest() {
        zombieListReplacement.Add(new RandomChoice<GameObject>(1, allBiggestFan_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Biggest Fan");
    }

    [Server]
    public void Apply_AllHockEye() {
        zombieListReplacement.Add(new RandomChoice<GameObject>(1, allHockEye_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Hock Eye");
    }
    [Server]
    public void Apply_AllLungs() {
        zombieListReplacement.Add(new RandomChoice<GameObject>(1, allLungs_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Lungs");
    }
    [Server]
    public void Apply_AllSplitter() {
        zombieListReplacement.Add(new RandomChoice<GameObject>(1, allSplitter_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Splitter");
    }

    [Server]
    public void Apply_AllZathrak() {
        zombieListReplacement.Add(new RandomChoice<GameObject>(1, allZathrak_Prefab));
        replaceZombieList = true;
        Debug.Log("MODIFIER: All Zathrak");
    }
    #endregion

    [Server]
    public void Apply_SaftyOff() {
        Debug.Log("MODIFIER: Safty Off");
        foreach (GameObject player in players) {
            player.GetComponent<PlayerHealth>().SetFriendlyFire(true);
        }
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

    float PINGPONG_LUNG_KNOCKBACK = 500;
    float PINGPONG_EYE_KNOCKBACK = 400;
    [Server]
    public void Apply_PingPong(){
        Debug.Log("MODIFIER: Ping Pong");
        ZombieLunge.SetPingPong(PINGPONG_LUNG_KNOCKBACK);
        HockEyeEye.SetPingPong(PINGPONG_EYE_KNOCKBACK);
        // if we add any other zombies that don't lunge, we will need to update this function
    }

    private const float CSWS_STRENGTH = 0.5f; // HP lost per second when not moving
    private const float CSWS_UPDATE_TIME = 0.5f;
    [Server]
    public void Apply_CSWS() // can't stop won't stop
    {
        Debug.Log("MODIFIER: CSWS");
        CSWS_StartClients();
    }
    [ClientRpc]
    private void CSWS_StartClients() {
        timer.CreateTimer(CSWS_UPDATE_TIME, CSWS_Damage);
    }
    [Client]
    private void CSWS_Damage() {
        foreach (var player in PlayerManager.instance.GetActiveLocalPlayers()) {
            //If the player hasn't moved since the last check for CSWS
            if (player.GetComponent<PlayerMovement>().lastMoved > CSWS_UPDATE_TIME) {
                player.GetComponent<PlayerHealth>().DamageCMD(CSWS_STRENGTH / 2, false, false, true);
            }
        }
        timer.CreateTimer(CSWS_UPDATE_TIME, CSWS_Damage);
    }

    // change this to change the starting amount
    int STUDENTLOANS_START_AMOUNT = 5000;
    [Server]
    public void Apply_StudentLoans() 
    {
        Debug.Log("MODIFIER: Student Loans");
        RoundController.instance.EventRoundChange += StudentLoans_OnRoundChange;
    }
    [Server]
    private void StudentLoans_OnRoundChange(int round) {
        Debug.Log("STUDENT LOANS: Round change: " + round);
        //Start Round
        if (round == RoundController.instance.startRound) {
            foreach (GameObject player in players) {
                Debug.Log(player.GetComponent<PlayerStats>().GetName() + "Gained $" + STUDENTLOANS_START_AMOUNT);
                player.GetComponent<PlayerStats>().AddMoney(STUDENTLOANS_START_AMOUNT);
                MoneyEffectManager.instance.CreateEffect(player, player.transform.position, STUDENTLOANS_START_AMOUNT);
            }
            return;
        }
        //Other Rounds
        foreach (GameObject player in players) {
            Debug.Log(player.GetComponent<PlayerStats>().GetName() + " lost $" + StudentLoans_GetAmountLost(round));
            player.GetComponent<PlayerStats>().SpendMoney(StudentLoans_GetAmountLost(RoundController.instance.round));
        }
    }
    //How the amount lost each round is determined
    int StudentLoans_GetAmountLost(int round) {
        if (round <= 1)
            return 0;
        return (round - 1) * 500;
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
                    case ModifierType.pingPong:
                        Apply_PingPong();
                        break;
                    case ModifierType.csws:
                        Apply_CSWS();
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

    private void Awake() {
        timer = GetComponent<Timer>();
    }
    public override void OnStartServer() {
        base.OnStartServer();
        SceneLoader.instance.AddPostLoad(ApplyModifiers);
    }
}
