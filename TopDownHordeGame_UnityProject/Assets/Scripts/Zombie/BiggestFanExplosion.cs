using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggestFanExplosion : MonoBehaviour
{
    [SerializeField] float balanceRadius;
    private int balanceDamage;
    [SerializeField] float throwStrength;
    private void Awake()
    {
        Vector3 location = transform.position;
        Vector3 balanceScale;
        balanceScale.x = balanceRadius;
        balanceScale.y = balanceRadius;
        balanceScale.z = balanceRadius;

        Debug.Log("Resizing explosion object.");
        transform.localScale = balanceScale;
        GetComponent<HitBoxController>().EventObjEnter += DamagePlayers;
        GetComponent<HitBoxController>().EventObjEnter += Knockback;
    }
    public void SetExplosionDamage(int damage)
    {
        balanceDamage = damage;
    }
    public void DamagePlayers(GameObject actor)
    {
        if (actor.CompareTag("Player"))
        {
            actor.GetComponent<PlayerHealth>().DamageCMD(balanceDamage);
        }
    }
    public void Knockback(GameObject creature)
    {
        Vector2 epicenterLocation = transform.position;
        Vector2 creatureLocation = creature.transform.position;
        Vector2 throwDirection = creatureLocation - epicenterLocation;
        Debug.Log(throwDirection.x + "        " + throwDirection.y);
        if (throwDirection.x == 0f || throwDirection.y == 0f)
        {
            Debug.Log("Randomizing direction.");
            throwDirection.x = Random.Range(-1f, 1f);
            throwDirection.y = Random.Range(-1f, 1f);
        }
        throwDirection = throwDirection.normalized;
        Debug.Log(throwDirection.x + "        " + throwDirection.y);
        Vector2 throwAmount = throwDirection * throwStrength;
        creature.GetComponent<Rigidbody2D>().AddForce(throwAmount);
    }

}
