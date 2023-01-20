using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Rock : Prop
{
    [SerializeField]
    private ParticleSystem particle;
    protected override void OnShot(Weapon weapon) {
        particle.Play();
    }
}
