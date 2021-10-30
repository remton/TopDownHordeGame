using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerkHolder : MonoBehaviour
{
    private List<Perk> perks;

    public void AddPerk(Perk p) {
        p.OnPerkGained(this.gameObject);
        perks.Add(p);
    }
    public void RemoveAllPerks() {
        for (int i = 0; i < perks.Count; i++) {
            perks[i].OnPerkLost(this.gameObject);
        }
        perks.Clear();
    }
}
