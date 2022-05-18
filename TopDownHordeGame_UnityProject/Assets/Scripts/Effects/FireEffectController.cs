using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffectController : MonoBehaviour
{
    public float duration; // How long in seconds until the trail fades 
    public float maxDistance;
    private float volume = 1;
    public GameObject trailObjPrefab; // The trail to use for this gun

    public void CreateTrail(Vector2 pos1, Vector2 pos2) {
        //fireSound = sound;
        GameObject trailObj = Instantiate(trailObjPrefab);
        //AudioSource audio = trailObj.GetComponent<AudioSource>(); 
        //SoundPlayer.Play(fireSound, transform.position, volume);
        trailObj.GetComponent<BulletTrail>().Init(pos1, pos2, duration);
    }
    public void CreateTrailDir(Vector2 pos1, Vector2 direction) {
        direction.Normalize();
        Debug.Log("dir.x: " + direction.x.ToString() + "dir.y: " + direction.y.ToString());

        GameObject trailObj = Instantiate(trailObjPrefab);
        Vector2 pos2 = new Vector2(maxDistance * direction.x, maxDistance * direction.y);
        pos2 += pos1;
        trailObj.GetComponent<BulletTrail>().Init(pos1, pos2, duration);
    }
}
