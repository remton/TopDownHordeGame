using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Prop : NetworkBehaviour
{
    public bool canBeShot;
    public int hardness; // used to determine bullet penetration

    [Command(requiresAuthority = false)]
    public void ShootCMD() {
        ShootRPC();
    }
    [ClientRpc]
    private void ShootRPC() {
        OnShot();
    }

    protected virtual void OnShot() {
        
    }
}
