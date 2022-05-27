using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G18 : Weapon
{
    public int pelletCount;
    public float spreadAngle;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        AudioManager.instance.PlaySound(shootSound);
        FireShot(player, direction, spreadAngle);
    }
}
