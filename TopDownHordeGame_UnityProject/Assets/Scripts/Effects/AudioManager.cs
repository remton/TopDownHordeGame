using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour {
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public AudioSource sfxSource;
    AudioSource[] musicSources;
    int activeMusicSource;
    public static AudioManager instance;
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            //Debug.Log("AudioManager already has an instance, one was destoyed");
            Destroy(gameObject);
        }

        musicSources = new AudioSource[2];
        for (int i = 0; i < 2; i++) {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }
    }

    private void Start() {
        if (SaveData.instance != null) {
            SettingsChange(SaveData.instance);
            SaveData.instance.EventSettingsChange += SettingsChange;
        }
    }
    private void OnDestroy() {
        SaveData.instance.EventSettingsChange -= SettingsChange;
    }

    public void SettingsChange(SaveData data) {
        masterVolume = data.settings_volumeMaster;
        musicVolume = data.settings_volumeMusic * masterVolume;
        sfxVolume = data.settings_volumeSFX * masterVolume;
        musicSources[activeMusicSource].volume = musicVolume;
    }

    // SFX //

    public void PlaySound(AudioClip clip, float volumeMultiplier = 1f) {
        sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
    }


    // MUSIC //
    //Fades into new music track
    public void PlayMusic(AudioClip clip, float fadeDuration = 2) {
        if (clip == musicSources[activeMusicSource].clip)
            return;

        int oldSource = activeMusicSource;
        activeMusicSource = (activeMusicSource + 1) % musicSources.Length;
        musicSources[activeMusicSource].loop = true;
        musicSources[activeMusicSource].clip = clip;
        musicSources[activeMusicSource].Play();
        StartCoroutine(AnimateMusicCrossfade(activeMusicSource, oldSource, fadeDuration));
    }
    //Fades out music over the duration
    public void FadeOutMusic(float fadeDuration = 2) {
        int oldSource = activeMusicSource;
        activeMusicSource = (activeMusicSource + 1) % musicSources.Length;
        musicSources[activeMusicSource].Stop();
        StartCoroutine(AnimateMusicCrossfade(activeMusicSource, oldSource, fadeDuration));
    }
    IEnumerator AnimateMusicCrossfade(int newIndex, int oldIndex, float duration) {
        if (duration <= 0) {
            Debug.Log("Instant music switch");
            musicSources[oldIndex].volume = 0;
            musicSources[newIndex].volume = musicVolume;
            yield break;
        }
        //Fade out old
        float timeLeft = duration/2;
        musicSources[newIndex].volume = 0;
        while (timeLeft > 0) {
            float percent = timeLeft / duration;
            timeLeft -= Time.deltaTime;
            musicSources[oldIndex].volume = Mathf.Lerp(0, musicVolume, percent);
            yield return new WaitForEndOfFrame();
        }
        //Fade in new
        timeLeft = duration / 2;
        musicSources[oldIndex].volume = 0;
        while (timeLeft > 0) {
            float percent = timeLeft / duration;
            timeLeft -= Time.deltaTime;
            musicSources[newIndex].volume = Mathf.Lerp(musicVolume, 0, percent);
            yield return new WaitForEndOfFrame();
        }
    }
}