using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
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
    public void PlaySound(AudioClip clip, Vector3 pos) {
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        //AudioSource.PlayClipAtPoint (clip, transform.position, sfxVolume * masterVolume);
    }
    IEnumerator AnimateMusicCrossfade(float duration) {
        float percent = 0;

        while(percent < 1) {
            percent += Time.deltaTime * 1/duration; 
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0,musicVolume * masterVolume, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolume * masterVolume, 0, percent);
            yield return null;
        }
    }

}