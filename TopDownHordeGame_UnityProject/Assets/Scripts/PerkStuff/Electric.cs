using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Electric : Perk
{
    private int balanceDamage = 3; // Damage for reload 
    private float balanceRadius = 1.7F; // Radius for reload 
    public GameObject circleObjPrefab;
    public GameObject electricReloadPrefab;
    private GameObject electricReloadObj;
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
    }

    public void DamageZombies(GameObject zombie) {
        zombie.GetComponent<ZombieHealth>().Damage(balanceDamage);
        player.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for electricity hitting someone 
        if (zombie.GetComponent<ZombieHealth>().isDead())
            player.GetComponent<PlayerStats>().AddKill();
    }

    private float timeActive;
    private float timeUntilDestroy = .1F;

    private void Update()
    {   
        if (electricReloadObj)
        {
            if (timeActive >= timeUntilDestroy)
            {
                DestroyReloadObject(electricReloadObj);
                timeActive = 0;
            }
            else
            {
                timeActive += Time.deltaTime;
            }
        }
    }

    public void ElectricReloadDamage(GameObject player)
    {
        this.player = player;
        Debug.Log("Electric Reload should have activated. ");
        electricReloadObj = Instantiate(electricReloadPrefab, transform);
        electricReloadObj.transform.position = player.transform.position;
        Vector3 balanceScale;
        balanceScale.x = balanceRadius;
        balanceScale.y = balanceRadius;
        balanceScale.z = balanceRadius;

        electricReloadObj.transform.localScale = balanceScale;
        electricReloadObj.GetComponent<HitBoxController>().EventObjEnter += DamageZombies;

       }
    private void DestroyReloadObject(GameObject electricReloadObj)
    {
        electricReloadObj.GetComponent<HitBoxController>().EventObjEnter -= DamageZombies;
        Destroy(electricReloadObj);
    }

}
