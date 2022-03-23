using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public MusicManager instance;
    public float minTimeLooping;
    public float waitBetweenSongs;
    public List<AudioClip> musicClips;

    private Timer timer;
    private AudioSource source;

    private void Start() {
        if (musicClips.Count == 0) {
            Debug.LogWarning("MusciManager has no music clips!");
            return;
        }
        StartPlaying();
    }
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        source = GetComponent<AudioSource>();
        timer = GetComponent<Timer>();
    }
    //TODO: make clips choose based on whats happening in the game
    private AudioClip GetMusicClip() {
        int rand = Random.Range(0, musicClips.Count);
        return musicClips[rand];
    }
    private void StopLoop() {
        source.loop = false;
        float timeLeftInSong = source.clip.length - source.time;
        timer.CreateTimer(waitBetweenSongs+timeLeftInSong, StartPlaying);
    }
    private void StartPlaying() {
        source.loop = true;
        source.clip = GetMusicClip();
        source.Play();
        timer.CreateTimer(minTimeLooping, StopLoop);
    }
}
