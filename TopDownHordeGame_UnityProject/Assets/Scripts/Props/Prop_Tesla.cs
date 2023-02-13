using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Tesla : Prop
{
    [SerializeField] private GameObject lightningboltEffect;
    [SerializeField] private HitBoxController trigger;
    [SerializeField] private GameObject effectStart;

    protected override void OnShot(Weapon weapon) {
        base.OnShot(weapon);
        GameObject hit = FindActor();
        if (hit = null)
            return;
        GameObject effect = Instantiate(lightningboltEffect);
        effect.GetComponent<LightningboltEffect>().SetPos(effectStart.transform.position, hit.transform.position);
    }
    private GameObject FindActor() {
        List<GameObject> actors = trigger.Hits();
        int rand = Random.Range(0, actors.Count);
        return actors[rand];
    }
}
