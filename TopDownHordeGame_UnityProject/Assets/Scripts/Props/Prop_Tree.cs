using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Tree : Prop {
    [SerializeField]
    private ParticleSystem particle;
    [SerializeField]
    private ParticleSystem leavesParticle;
    protected override void OnShot() {
        particle.Play();
        leavesParticle.Play();
    }
}
