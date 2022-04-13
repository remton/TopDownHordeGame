using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OLD AND SHOULD BE UNUSED
public class OLD_SoundPlayer : MonoBehaviour
{
    //Create a temperary gameobject to play our clip and play it
    public static void Play(AudioClip clip, Vector3 location) {
        GameObject obj = new GameObject("AudioClipObj");
        obj.transform.position = location;
        obj.AddComponent<AudioSource>();
        OLD_SoundPlayer sp = obj.AddComponent<OLD_SoundPlayer>();
        TimedDestroy td = obj.AddComponent<TimedDestroy>();
        td.destroyTime = clip.length;
        sp.PlayClip(clip);
    }
    public static void Play(AudioClip clip, Vector3 location, float volume) {
        GameObject obj = new GameObject("AudioClipObj");
        obj.transform.position = location;
        obj.AddComponent<AudioSource>();
        OLD_SoundPlayer sp = obj.AddComponent<OLD_SoundPlayer>();
        TimedDestroy td = obj.AddComponent<TimedDestroy>();
        td.destroyTime = clip.length;
        sp.PlayClip(clip, volume);
    }
    private AudioSource audioSource;
    private void Awake() {
        Debug.LogWarning("WARNING: OLD SOUND PLAYER STILL USED");
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
