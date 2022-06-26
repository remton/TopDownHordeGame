using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AA12 : Weapon
{
    public int pelletCount;
    public float spreadAngle;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x);
        float angleDiff;
        Vector2 fireDir;
        for (int i = 0; i < pelletCount; i++)
        {
            angleDiff = Random.Range((baseAngle - spreadAngle / 2), (baseAngle + spreadAngle / 2));
            fireDir = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
            FireShot(player, fireDir);
        }
    }
}