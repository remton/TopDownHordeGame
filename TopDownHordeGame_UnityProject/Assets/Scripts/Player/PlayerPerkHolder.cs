using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerkHolder : MonoBehaviour
{
    private List<Perk> perks;

    public void AddPerk(GameObject perkPrefab) {
        perkPrefab.GetComponent<Perk>().OnPerkGained(this.gameObject);
        GameObject perkObj = Instantiate(perkPrefab, transform);
        perks.Add(perkObj.GetComponent<Perk>());
    }
    public void RemoveAllPerks() {
        for (int i = 0; i < perks.Count; i++) {
            perks[i].GetComponent<Perk>().OnPerkLost(this.gameObject);
            Destroy(perks[i].gameObject);
        }
        perks.Clear();
    }
}
