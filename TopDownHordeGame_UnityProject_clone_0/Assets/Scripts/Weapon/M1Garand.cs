using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1Garand : Weapon
{
    public override void Fire(GameObject player, Vector2 direction) {
        base.Fire(player, direction);
        AudioManager.instance.PlaySound(shootSound);
        FireShot(player, direction);
    }
}
