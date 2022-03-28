using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    //Create a temperary gameobject to play our clip and play it
    public static void Play(AudioClip clip, Vector3 location) {
        GameObject obj = new GameObject("AudioClipObj");
        obj.transform.position = location;
        obj.AddComponent<AudioSource>();
        SoundPlayer sp = obj.AddComponent<SoundPlayer>();
        TimedDestroy td = obj.AddComponent<TimedDestroy>();
        td.destroyTime = clip.length;
        sp.PlayClip(clip);
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
    private void PlayClip(AudioClip newClip, float volumeMultiplier) {
        audioSource.clip = newClip;
        audioSource.volume = 1 * volumeMultiplier;
        audioSource.Play();
        Destroy(gameObject, newClip.length);
    }

}
