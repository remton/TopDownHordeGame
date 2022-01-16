using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M16 : Weapon
{
    public float spreadAngle;
    public override void Fire(GameObject player, Vector2 direction)
    {
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x);
        float angleDiff;
        angleDiff = Random.Range((baseAngle - spreadAngle / 2), (baseAngle + spreadAngle / 2));
        Vector2 fireDir;
        fireDir = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
        base.Fire(player, direction);
        FireShot(player, fireDir);

    }
}