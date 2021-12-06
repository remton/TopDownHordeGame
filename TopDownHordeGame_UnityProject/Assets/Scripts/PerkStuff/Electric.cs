using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Electric : Perk
{
    private int balanceDamage = 3; // Damage for reload 
    private float balanceRadius = 0.4F; // Radius for reload 
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
//        player.GetComponent<PlayerWeaponControl>().electricDamage = (balanceDamage);
//        player.GetComponent<PlayerWeaponControl>().electricRadius = (balanceRadius);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
//        player.GetComponent<PlayerWeaponControl>().electricDamage = (1 / balanceDamage);
//        player.GetComponent<PlayerWeaponControl>().electricRadius = (1 / balanceRadius);
    }

    public void ElectricReloadDamage(GameObject player)
    {
        Debug.Log("Electric Reload should have activated. ");
        // To Do: Cause the reload to create an shockwave visual effect and damage all zombies it touches.
/*        CircleCollider2D myCircle
        RaycastHit2D effectRay = Physics2D.Raycast(player.transform.position, circle, Mathf.Infinity, LayerMask.GetMask("BulletCollider"));
        Vector2 startPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        if (effectRay)
        {
            Vector2 hitPoint = effectRay.point;
            Vector2 endPos = new Vector3(hitPoint.x, hitPoint.y, 0);
//            effectController.CreateTrail(startPos, endPos);
        }
        else
        {
//            effectController.CreateTrailDir(startPos, direction);
        }

        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(player.transform.position, circle, Mathf.Infinity, LayerMask.GetMask("Zombie"));
        
            GameObject zombieHit = hitInfos[i].transform.gameObject;
            if (zombieHit.CompareTag("Zombie"))
            {
                zombieHit.GetComponent<ZombieHealth>().Damage(balanceDamage);
                player.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for electricity hitting someone 
                if (zombieHit.GetComponent<ZombieHealth>().isDead())
                    player.GetComponent<PlayerStats>().AddKill();
        } */
    }
}
