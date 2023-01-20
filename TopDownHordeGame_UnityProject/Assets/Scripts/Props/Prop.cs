using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Prop : NetworkBehaviour
{
    public bool canBeShot;
    public int hardness; // used to determine bullet penetration

    [Command(requiresAuthority = false)]
    public void ShootCMD(Weapon weapon) {
        ShootRPC(weapon);
    }
    [ClientRpc]
    private void ShootRPC(Weapon weapon) {
        OnShot(weapon);
    }

    protected virtual void OnShot(Weapon weapon) {
        
    }
}
