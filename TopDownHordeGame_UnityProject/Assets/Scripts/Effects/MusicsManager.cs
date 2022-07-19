using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Timer))]
public class MusicsManager : MonoBehaviour {

    private List<AudioClip> activeMusics = new List<AudioClip>();

    // ---- SCENES AND THEIR MUSIC ----
    //dont forget to add the scenes to the set scene method as well
    //Clips arranged low intensity -> high intensity
    [Mirror.Scene] 
    public string menuScene; 
    public List<AudioClip> menuMusics = new List<AudioClip>();
    
    [Mirror.Scene] 
    public string lobbyScene; 
    public List<AudioClip> lobbyMusics = new List<AudioClip>();

    [Mirror.Scene]
    public string gameoverScene;
    public List<AudioClip> gameoverMusics = new List<AudioClip>();

    [Mirror.Scene] 
    public string catcafeScene;
    public List<AudioClip> catcafeMusics = new List<AudioClip>();

    [Mirror.Scene] 
    public string labScene;
    public List<AudioClip> labMusics = new List<AudioClip>();

    private Timer timer;
    System.Guid uptimeTimer;

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
        SceneManager.activeSceneChanged += SceneChanged;
    }
    private void OnDestroy() {
        SceneManager.activeSceneChanged -= SceneChanged; 
    }

    public void SceneChanged(Scene current, Scene next) {
        Debug.Log("Scene changed");
        StartCoroutine(SetScene(next));
    }
    // --- ADD NEW SCENES TO THIS METHOD ----
    IEnumerator SetScene(Scene next) {
        yield return new WaitForEndOfFrame();
        if (next.path == menuScene) {
            activeMusics = menuMusics;
        }
        else if (next.path == lobbyScene) {
            activeMusics = menuMusics;
        }
        else if (next.path == gameoverScene) {
            activeMusics = gameoverMusics;
        }
        else if (next.path == catcafeScene) {
            activeMusics = catcafeMusics;
        }
        else if (next.path == labScene) {
            activeMusics = labMusics;
        }
        else {
            Debug.LogWarning("No music registered for scene, " + next.path);
            Debug.LogWarning("CatCafe: " + catcafeScene);
        }
        StopMusic();
        StartMusic();
    }


    public void StopMusic() {
        timer.KillTimer(uptimeTimer);
        AudioManager.instance.FadeOutMusic(0);
    }

    public void StartMusic() {
        if (activeMusics.Count > 0) {
            int rand = Random.Range(0, 100);
            if (rand < musicChance) {
                rand = Random.Range(0, activeMusics.Count);
                AudioClip clip = activeMusics[rand];
                AudioManager.instance.PlayMusic(clip, fadeDuration);
                //Debug.Log("PLAYING CLIP: " + clip.name);
            }
            else {
                AudioManager.instance.FadeOutMusic(fadeDuration);
            }
            uptime = Random.Range(musicUptimeMin, musicUptimeMax);
            uptimeTimer = timer.CreateTimer(uptime * 60, StartMusic);
        }
        else {
            AudioManager.instance.FadeOutMusic(fadeDuration);
        }
    }
}