using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerkHolder : MonoBehaviour
{
    public delegate void PerkChanged(List<Perk> perks);
    public event PerkChanged EventPerkChanged;

    private List<Perk> perks = new List<Perk>();
    public void AddPerk(GameObject perkPrefab) {
        perkPrefab.GetComponent<Perk>().OnPerkGained(this.gameObject);
        GameObject perkObj = Instantiate(perkPrefab, transform);
        perkObj.transform.position = gameObject.transform.position;
        perks.Add(perkObj.GetComponent<Perk>());
        if (EventPerkChanged != null) { EventPerkChanged.Invoke(perks); }
    }
    public void RemoveAllPerks()
    {
        for (int i = 0; i < perks.Count; i++)
        {
            perks[i].GetComponent<Perk>().OnPerkLost(this.gameObject);
            Destroy(perks[i].gameObject);
        }
        perks.Clear();
        if (EventPerkChanged != null) { EventPerkChanged.Invoke(perks); }
    }
    public bool HavePerk(GameObject testPerk)
    {
        for (int i = 0; i < perks.Count; i++)
        {
            if (perks[i].type == testPerk.GetComponent<Perk>().type)
            {
                return (true);
            }
        }
        return (false); 
    }
    public void CallElectricDamage(GameObject testPerk)
    {
        for (int i = 0; i < perks.Count; i++)
        {
            if (perks[i].type == testPerk.GetComponent<Perk>().type)
            {
                perks[i].GetComponent<Electric>().ElectricReloadDamage(this.gameObject); 
            }
        }
    }
}
