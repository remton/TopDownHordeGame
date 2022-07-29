using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class MoneyEffectManager : NetworkBehaviour
{
    public static MoneyEffectManager instance;

    

    public GameObject moneyEffectPrefab;
    public float comboTime;

    private Timer timer;
    private System.Guid[] comboTimers;
    private GameObject[] players;
    private MoneyEffect[] latestEffect;
    
    /// <summary>
    /// Absolute garbage code. Waits for startDelay seconds before
    /// getting players to make sure they have spawned.
    /// </summary>
    private float startDelay = 2f;

    /// <summary> [Server] Creates an effect for the given player at pos with amount </summary>
    [Server]
    public void CreateEffect(GameObject player, Vector3 pos, int amount) {
        if (comboTimers == null || players == null || latestEffect == null) {
            Debug.Log("nullreference handled");
            return;
        }

        for (int i = 0; i < players.Length; i++) {
            if (player == players[i]) {
                GameObject obj = Instantiate(moneyEffectPrefab, pos, Quaternion.identity);
                NetworkServer.Spawn(obj);
                if (timer.HasTimer(comboTimers[i])) {
                    //We are still in a combo
                    if (latestEffect[i] != null) {
                        latestEffect[i].HideAmount();
                        amount += latestEffect[i].GetAmount();
                    }
                    timer.SetTimer(comboTimers[i], comboTime, ComboEnd);
                }
                else {
                    comboTimers[i] = timer.CreateTimer(comboTime, ComboEnd);
                }
                obj.GetComponent<MoneyEffect>().SetAmount(amount);
                latestEffect[i] = obj.GetComponent<MoneyEffect>();
            }
        }
    }

    private void ComboEnd() {
        //Do nothing. Timer requires a method to call on timer end.
    }

    private void InitPlayers() {
        players = PlayerManager.instance.GetAllPlayers().ToArray();
        comboTimers = new System.Guid[players.Length];
        latestEffect = new MoneyEffect[players.Length];
        Debug.Log(players.Length + " players");
    }

    private void Start() {
        timer.CreateTimer(startDelay, InitPlayers);
    }

    private void Awake() {
        timer = GetComponent<Timer>();
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
