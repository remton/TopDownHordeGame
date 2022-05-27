using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public float masterVolume;
    public AudioSource sfxSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;
    public static AudioManager instance;
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Debug.Log("AudioManager already has an instance, one was destoyed");
            Destroy(gameObject);
        }

        musicSources  = new AudioSource[2];
        for (int i = 0; i < 2; i++) {
            GameObject newMusicSource = new GameObject ("Music source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();            
            newMusicSource.transform.parent = transform;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1) {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
    
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }
    public void PlaySound(AudioClip clip, float volumeMultiplier=1f) {
        sfxSource.PlayOneShot(clip, SaveData.instance.settings_volumeSFX * masterVolume * volumeMultiplier);
    }
    IEnumerator AnimateMusicCrossfade(float duration) {
        float percent = 0;

        while(percent < 1) {
            percent += Time.deltaTime * 1/duration; 
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0,SaveData.instance.settings_volumeMusic * masterVolume, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(SaveData.instance.settings_volumeMusic * masterVolume, 0, percent);
            yield return null;
        }
    }
}