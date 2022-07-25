using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ModifiersController : NetworkBehaviour
{
    // --- modifier flags ---
    public bool fanClub { get; internal set; }
    public List<RandomChoice> fanClubList;

    /// <summary> Reads and applys modifiers from GameSettigns </summary>
    public void ApplyModifiers() {
        ReadModifiers();
        if (fanClub)
            ApplyFanClub();
    }

    // --- Modifiers ---
    [Server]
    public void ApplyFanClub() {
        Debug.Log("Applying fan club...");
        RoundController.instance.zombieList = fanClubList;
    }


    // --- Private methods ---
    private void ReadModifiers() {
        GameSettings settings = GameSettings.instance;
        fanClub = settings.modifier_fanClub;
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
