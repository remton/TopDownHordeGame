using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrett50 : Weapon {
    public override void Fire(GameObject player, Vector2 direction) {
        base.Fire(player, direction);
        FireShot(player, direction);
    }
}