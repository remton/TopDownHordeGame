using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour
{
    public static void Play(AudioClip clip, Vector3 location) {
        GameObject obj = new GameObject("AudioClipObj");
        obj.transform.position = location;
        obj.AddComponent<AudioSource>();
        AudioClipPlayer acp = obj.AddComponent<AudioClipPlayer>();
        TimedDestroy td = obj.AddComponent<TimedDestroy>();
        td.destroyTime = clip.length;
        acp.PlayClip(clip);
    }

    private AudioSource audioSource;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }
    private void PlayClip(AudioClip newClip) {
        audioSource.clip = newClip;
        audioSource.Play();
        Destroy(gameObject, newClip.length);
    }

}
