using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffectController : MonoBehaviour
{
    public float duration; // How long in seconds until the trail fades 
    public float maxDistance;

    public GameObject trailObjPrefab; // The trail to use for this gun

    public void CreateTrail(Vector2 pos1, Vector2 pos2) {
        GameObject trailObj = Instantiate(trailObjPrefab);
        trailObj.GetComponent<BulletTrail>().Init(pos1, pos2, duration);
    }
    public void CreateTrailDir(Vector2 pos1, Vector2 direction) {
        //TODO: Create a line effect in the direction
    }
}
