using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ElectricfyWeapon : NetworkBehaviour
{
    public bool active = false;

    public GameObject lightningboltEffect;
    public float range;
    public int maxChains;
    public float damage;

    private Weapon weapon;
    private List<GameObject> chainedObjs = new List<GameObject>();
    private List<Vector3> effectStarts = new List<Vector3>();
    private List<Vector3> effectEnds = new List<Vector3>();
    private GameObject owner;

    public void OnWeaponFired(GameObject Owner, List<GameObject> Victims, Vector3 startPos, Vector3 endPos) {
        if (!active)
            return;

        this.owner = Owner;
        chainedObjs.Clear();
        effectStarts.Clear();
        effectEnds.Clear();
        ChainRecursive(0, endPos);

        CreateEffectCMD(effectStarts.ToArray(), effectEnds.ToArray());
    }
    private void ChainRecursive(int depth, Vector3 previousPos) {
        //Base case
        if (depth > maxChains)
            return;
        string[] mask = { "ZombieHitbox"};
        Collider2D[] hits = Physics2D.OverlapCircleAll(previousPos, range, LayerMask.GetMask(mask));
        foreach (Collider2D hit in hits) {
            GameObject hitObj = hit.gameObject.GetComponent<DamageHitbox>().owner;
            if (hit != null && !chainedObjs.Contains(hitObj)) {
                chainedObjs.Add(hitObj);
                hitObj.GetComponent<ZombieHealth>().DamageCMD(damage, owner);
                effectStarts.Add(previousPos);
                effectEnds.Add(hitObj.transform.position);
                ChainRecursive(depth + 1, hitObj.transform.position);
                break;
            }
        }
    }
    [Command(requiresAuthority = false)]
    private void CreateEffectCMD(Vector3[] starts, Vector3[] ends) {
        CreateEffectRPC(starts, ends);
    }
    [ClientRpc]
    private void CreateEffectRPC(Vector3[] starts, Vector3[] ends) {
        for (int i = 0; i < starts.Length; i++) {
            GameObject effect = Instantiate(lightningboltEffect);
            effect.GetComponent<LightningboltEffect>().SetPos(starts[i], ends[i]);
        }
    }

    private void Awake() {
        weapon = GetComponent<Weapon>();
        weapon.EventWeaponFired += OnWeaponFired;
    }
    private void OnDestroy() {
        weapon.EventWeaponFired -= OnWeaponFired;
    }
}
