using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class MusicsManager : MonoBehaviour {

    //public float intensity
    ////Clips arranged low intensity -> high intensity
    //public list<AucioClip> clips

    private Timer timer;
    System.Guid uptimeTimer;

    public AudioClip mainTheme;
    public AudioClip battleTheme;
    [Header("percent chance 0% to 100% that music will play")]
    public int musicChance;
    [Header("Music Uptime range before changing tracks")]
    public float musicUptimeMin;
    public float musicUptimeMax;
    private float uptime;
    [Header("How long fading between tracks takes in seconds")]
    public float fadeDuration;

    private void Awake() {
        timer = GetComponent<Timer>();
    }

    void Start() {
        StartMusic();
    }
    public void StartMusic() {
        MusicsChange();
    }
    public void StopMusic() {
        timer.KillTimer(uptimeTimer);
        AudioManager.instance.FadeOutMusic(0);
    }
    public void MusicsChange() {
        float rand = Random.Range(0, 100);
        if (rand < musicChance) {
            AudioClip clip;
            rand = Random.Range(0, 1);
            if (rand < 0.5f)
                clip = mainTheme;
            else
                clip = battleTheme;
            AudioManager.instance.PlayMusic(clip, fadeDuration);
            //Debug.Log("PLAYING CLIP: " + clip.name);
        }
        else {
            AudioManager.instance.FadeOutMusic(fadeDuration);
        }

        uptime = Random.Range(musicUptimeMin, musicUptimeMax);
        uptimeTimer = timer.CreateTimer(uptime * 60, MusicsChange);
    }
}