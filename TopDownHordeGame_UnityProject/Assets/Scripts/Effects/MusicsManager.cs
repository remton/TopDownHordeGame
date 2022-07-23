using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Timer))]
public class MusicsManager : MonoBehaviour {

    [System.Serializable]
    private struct MusicScene {
        [Mirror.Scene]
        public string scene;
        [Tooltip("Clips arranged low intensity -> high intensity")]
        public List<AudioClip> musics; 
        [Tooltip("percent chance 0% to 100% that music will play")]
        public float musicChance;
        [Header("Music Uptime range in minutes before changing tracks")]
        public float musicUptimeMin;
        public float musicUptimeMax;
        public float silenceUptimeMin;
        public float silenceUptimeMax;
    }
    
    [Header("How long fading between tracks takes in seconds")]
    public float fadeDuration;

    [SerializeField][Header("Add each scene with its own music here")]
    private List<MusicScene> scenes;

    [SerializeField][Header("The default values if a scene is not in scenes")]
    private MusicScene defaultScene;

    private MusicScene currScene;
    private List<AudioClip> activeMusics = new List<AudioClip>();
    private Timer timer;
    System.Guid uptimeTimer;
    private float uptime;

    private void Awake() {
        timer = GetComponent<Timer>();
        SceneManager.activeSceneChanged += SceneChanged;
    }
    private void OnDestroy() {
        SceneManager.activeSceneChanged -= SceneChanged; 
    }

    public void SceneChanged(Scene current, Scene next) {
        //Debug.Log("Scene changed");
        currScene = defaultScene;
        foreach (MusicScene scene in scenes) {
            if (scene.scene == next.path)
            currScene = scene;
        }
        StartCoroutine(SetScene(currScene));
    }
    //Waiting for a frame fixed an issue with loading the scene
    IEnumerator SetScene(MusicScene scene) {
        yield return new WaitForEndOfFrame();
        List<AudioClip> newMusics = scene.musics;
        //Dont restart music if it is the same music as before
        if (newMusics != activeMusics && newMusics.Count != 0) {
            activeMusics = newMusics;
            StartMusic();
        }
    }

    public void StopMusic() {
        timer.KillTimer(uptimeTimer);
        AudioManager.instance.FadeOutMusic(fadeDuration);
    }

    public void StartMusic() {
        if (activeMusics.Count > 0) {
            int rand = Random.Range(0, 100);
            if (rand < currScene.musicChance) {
                rand = Random.Range(0, activeMusics.Count);
                AudioClip clip = activeMusics[rand];
                AudioManager.instance.PlayMusic(clip, fadeDuration);
                uptime = Random.Range(currScene.musicUptimeMin, currScene.musicUptimeMax);
                //Debug.Log("PLAYING CLIP: " + clip.name);
            }
            else {
                AudioManager.instance.FadeOutMusic(fadeDuration);
                uptime = Random.Range(currScene.silenceUptimeMin, currScene.silenceUptimeMax);
            }
            uptimeTimer = timer.CreateTimer(uptime * 60, StartMusic);
        }
        else {
            AudioManager.instance.FadeOutMusic(fadeDuration);
        }
    }
}