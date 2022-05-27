using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicsManager : MonoBehaviour {

    public AudioClip mainTheme;
    public AudioClip battleTheme;
    void Start(){
        AudioManager.instance.PlayMusic(mainTheme, 2); 
    }
    public void MusicsChange(){ 
        AudioManager.instance.PlayMusic(battleTheme, 2);
    }
}