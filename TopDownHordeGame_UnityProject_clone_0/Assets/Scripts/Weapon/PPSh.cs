using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPSh : Weapon
{
    public int pelletCount;
    public float spreadAngle;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        FireShot(player, direction, spreadAngle);
        AudioManager.instance.PlaySound(shootSound);
    }
}
