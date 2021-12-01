using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSidebarManager : MonoBehaviour
{
    public GameObject sidebarPrefab;

    public void AddSidebar(GameObject player) {
        GameObject sidebarObj = Instantiate(sidebarPrefab, transform);
        UIPlayerSidebar sidebar = sidebarObj.GetComponent<UIPlayerSidebar>();
        sidebar.AttachToPlayer(player);
    }
}
