using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggestFanDeath: MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    private int balanceDamage;
    public void SetExplosionDamage(int damage)
    {
        balanceDamage = damage;
    }
    public void Explode()
    {
        Debug.Log("Explosion object should instantiate next. ");
        Vector3 location = transform.position;
        Quaternion rot = transform.rotation;
        GameObject obj = Instantiate(explosionPrefab, location, rot);
        obj.GetComponent<BiggestFanExplosion>().SetExplosionDamage(balanceDamage);
    }
}
