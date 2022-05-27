using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSidebarManager : MonoBehaviour
{
    public List<GameObject> sidebars;
    private int counter = 0;

    private void Awake() {
        foreach (GameObject sidebar in sidebars) {
            sidebar.GetComponent<UIPlayerSidebar>().Activate(false);
        }
    }

    public void AddSidebar(GameObject player) {
        if(counter >= sidebars.Count) {
            Debug.Log("Cannot add sidebar. Max sidebars active");
            return;
        }
        UIPlayerSidebar sidebar = sidebars[counter].GetComponent<UIPlayerSidebar>();
        sidebar.Activate(true);
        sidebar.AttachToPlayer(player);
        counter++;
    }
}
