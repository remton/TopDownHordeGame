using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FireEffectController : NetworkBehaviour
{
    public float duration; // How long in seconds until the trail fades 
    public float maxDistance;
    public GameObject trailObjPrefab; // The trail to use for this gun
    //public ParticleSystem particle;

    [Command(requiresAuthority = false)]
    public void CreateTrail(Vector2 pos1, Vector2 pos2) {
        GameObject trailObj = Instantiate(trailObjPrefab);
        NetworkServer.Spawn(trailObj);
        trailObj.GetComponent<BulletTrail>().Init(pos1, pos2, duration);
        //PlayEffect();
    }
    //[ClientRpc]
    //public void PlayEffect() {
    //    if (particle != null)
    //        particle.Play();
    //}
}
