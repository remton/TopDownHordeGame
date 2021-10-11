using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Assets/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private int damage;
    [SerializeField] private int magSize;
    [SerializeField] private int reserveSize;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireDeley;
    [SerializeField] private float swapSpeed;
    [SerializeField] private float moveSpeedPenalty;

    public virtual void Fire(GameObject player, Vector2 direction) {
        Debug.Log("Base Weapon Fire called!");
    }

}
