using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElectricfyWeapon))]
public class Taser : Weapon
{
    //[SerializeField] private float spreadAngle;
    //public override void Fire(GameObject player, Vector2 direction) {
    //    base.Fire(player, direction);
    //    direction.Normalize();
    //    float baseAngle = Mathf.Atan2(direction.y, direction.x);
    //    float angleDiff = -spreadAngle / 2;
    //    Vector2 fireDir;
    //    for (int i = 0; i < 2; i++) {
    //        fireDir = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
    //        FireShot(player, fireDir);
    //        angleDiff += spreadAngle;
    //    }
    //}
    [SerializeField] private float spreadAngle;
    public override void Fire(GameObject player, Vector2 direction) {
        base.Fire(player, direction);
        FireShot(player, direction, spreadAngle);
    }
}
