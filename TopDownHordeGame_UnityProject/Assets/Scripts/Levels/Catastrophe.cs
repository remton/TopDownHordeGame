using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(Timer))]
public class Catastrophe : NetworkBehaviour {
    public static string LevelName = "Catastrophe";

    [SerializeField] private float countdownTime;
    [SerializeField] private float countdownMusicStartTime;
    [SerializeField] private List<AudioClip> countdownMusic;
    private bool countdownMusicStarted;
    [SerializeField] private List<Prop_Screen> countdownScreens;
    public AudioClip timerBeepSound;
    public float timerBeepVolume;

    [SerializeField] private List<GameObject> powerDependents;
    [SerializeField] private List<GameObject> darknessObjs;
    public Color darknessPowerOnColor;
    public Color darknessPowerOffColor;
    [SerializeField] private Interactable powerSwitch;
    [SerializeField] private Interactable countdownDisableSwitch;

    [SerializeField] AudioClip powerOnSound;
    [SerializeField] float powerOnSoundVolume;
    [SerializeField] AudioClip powerOffSound;
    [SerializeField] float powerOffSoundVolume;

    private Timer timer;
    private System.Guid countdownID;
    private float countdownTimeLeft;
    private int prevCountdownTimeLeft;
    public bool isPowerOn { get; internal set; }

    public int countdownReward;
    private bool disabledCountdown = false;

    [SerializeField] AudioClip explosionSound;
    [SerializeField] float explosionSoundVolume;

    private void Awake() {
        timer = GetComponent<Timer>();
        countdownTimeLeft = countdownTime;
    }
    private void Update() {
        ClientUpdate();
    }
    [Client]
    private void ClientUpdate() {
        if (!disabledCountdown) {
            countdownTimeLeft = timer.TimeLeft(countdownID);
            foreach (Prop_Screen screen in countdownScreens) {
                string txt = "DETENATION IN \n" + Utilities.FormatTime(countdownTimeLeft);
                if (isPowerOn)
                    screen.SetText(txt);
                else
                    screen.SetText("");
            }
            if (Mathf.RoundToInt(countdownTimeLeft) < prevCountdownTimeLeft) {
                AudioManager.instance.PlaySound(timerBeepSound, timerBeepVolume);
            }
            if(!countdownMusicStarted && timer.HasTimer(countdownID) && countdownTimeLeft < countdownMusicStartTime) {
                countdownMusicStarted = true;
                Debug.Log("COUNTDOWN MUSIC");
                MusicsManager.instance.SetMusic(countdownMusic, 1);
            }

            prevCountdownTimeLeft = Mathf.RoundToInt(countdownTimeLeft);
        }
    }

    public override void OnStartServer() {
        base.OnStartServer();
        SceneLoader.instance.AddPostLoad(TurnOffPower);
        RoundController.instance.EventRoundChange += OnRoundChange;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        powerSwitch.EventOnInteract += PowerSwitch;
        countdownDisableSwitch.EventOnInteract += DisableCountdown;
    }

    private void SetLights(bool on) {
        Color darknessColor;
        if (on)
            darknessColor = darknessPowerOnColor;
        else
            darknessColor = darknessPowerOffColor;

        foreach (GameObject obj in darknessObjs) {
            if (obj.HasComponent<SpriteRenderer>())
                obj.GetComponent<SpriteRenderer>().color = darknessColor;
            if(obj.HasComponent<Tilemap>())
                obj.GetComponent<Tilemap>().color = darknessColor;
        }
        foreach (GameObject item in powerDependents) {
            if (item == null)
                continue;
            if (item.HasComponent<Interactable>())
                item.GetComponent<Interactable>().interactable = on;
            if (item.HasComponent<Light>()) {
                if(on)
                    item.GetComponent<Light>().TurnOn();
                else
                    item.GetComponent<Light>().TurnOff();
            }
        }
    }

    [Client]
    private void StartCountdown() {
        if (!timer.HasTimer(countdownID)) {
            countdownID = timer.CreateTimer(countdownTime, CountdownEnd);
            //Debug.Log("Starting COUNTDOWN");
        }
        else {
            //Debug.Log("unpause COUNTDOWN");
            timer.UnpauseTimer(countdownID);
        }
    }
    [Client]
    private void PauseCountdown() {
        //Debug.Log("pause COUNTDOWN");
        timer.PauseTimer(countdownID);
    }
    [Client]
    private void CountdownEnd() {
        if (isServer) {
            StartCoroutine(ExplodeMap());
        }
    }

    [Server]
    private IEnumerator ExplodeMap() {
        TurnOffPower();
        for (int i = 0; i < 15; i++) {
            AudioManager.instance.PlaySound(explosionSound, explosionSoundVolume);
            CameraController.instance.Shake(Random.Range(2f, i*2f));
            yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
        }
        PlayerManager.instance.EndGame();
    }

    [Server]
    private void OnRoundChange(int round) {
        if (round % 2 == 0)
            TurnOffPower();
    }


    [Command(requiresAuthority = false)]
    public void PowerSwitch(GameObject player) {
        Debug.Log("POWER SWITCH");
        if (isPowerOn)
            TurnOffPower();
        else
            TurnOnPower();
    } 

    [Command(requiresAuthority = false)]
    public void DisableCountdown(GameObject player) {
        if (disabledCountdown)
            return;
        player.GetComponent<PlayerStats>().AddMoney(countdownReward);
        MoneyEffectManager.instance.CreateEffect(player, player.transform.position, countdownReward);
        if (timer.HasTimer(countdownID)) {
            disabledCountdown = true;
            timer.KillTimer(countdownID);
        }
        DisableCountdownRPC();
    }
    [ClientRpc]
    public void DisableCountdownRPC() {
        if (timer.HasTimer(countdownID)) {
            disabledCountdown = true;
            timer.KillTimer(countdownID);
        }
    }

    [Server]
    private void TurnOffPower() {
        isPowerOn = false;
        SetLights(false);
        TurnOffPowerRPC();
    }
    [ClientRpc]
    private void TurnOffPowerRPC() {
        AudioManager.instance.PlaySound(powerOffSound, powerOffSoundVolume);
        isPowerOn = false;
        PauseCountdown();
        SetLights(false);
    }


    [Server]
    private void TurnOnPower() {
        isPowerOn = true;
        SetLights(true);
        TurnOnPowerRPC();
    }
    [ClientRpc]
    private void TurnOnPowerRPC() {
        AudioManager.instance.PlaySound(powerOnSound, powerOnSoundVolume);
        isPowerOn = true;
        StartCountdown();
        SetLights(true);
    }
}
