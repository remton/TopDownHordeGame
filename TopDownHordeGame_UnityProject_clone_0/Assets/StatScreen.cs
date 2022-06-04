using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StatScreen : NetworkBehaviour
{
    public static StatScreen instance;

    public PlayerSidebarManager sidebarManager;

    private void Awake() {
        if (instance == null)
            instance = this;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        CloseMenu();
    }

    public static void AddSidebar(GameObject player) {
        if (instance == null || player == null) {
            Debug.LogWarning("Failed to create player sidebar");
            return;
        }
        instance.AddSidebarRPC(player);
    }
    [ClientRpc]
    public void AddSidebarRPC(GameObject player) {
        sidebarManager.AddSidebar(player);
    }


    public static void OpenMenu() {
        if (instance == null)
            return;
        instance.gameObject.SetActive(true);
    }

    public static void CloseMenu() {
        if (instance == null)
            return;
        instance.gameObject.SetActive(false);
    }
}
