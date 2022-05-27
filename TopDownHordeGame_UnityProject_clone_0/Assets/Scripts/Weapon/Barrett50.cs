using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrett50 : Weapon {
    [SerializeField] float fireKnockback;
    public override void Fire(GameObject player, Vector2 direction) {
        base.Fire(player, direction);
        AudioManager.instance.PlaySound(shootSound);
        FireShot(player, direction);
        player.GetComponent<PlayerMovement>().KnockBack(fireKnockback, -direction);
    }
}