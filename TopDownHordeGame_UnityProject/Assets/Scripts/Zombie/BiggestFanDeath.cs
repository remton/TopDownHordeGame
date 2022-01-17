using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggestFanDeath: MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    public void Explode()
    {
        Debug.Log("Explosion object should instantiate next. ");
        Vector3 location = transform.position;
        Quaternion rot = transform.rotation;
        Instantiate(explosionPrefab, location, rot);
    }
}
