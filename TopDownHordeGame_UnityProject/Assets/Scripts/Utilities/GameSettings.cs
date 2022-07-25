using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;
    public int numPlayers = 1;
    public bool modifier_fanClub = false;

    public List<InputDevice> devices = new List<InputDevice>();

    public void ResetSettings() {
        numPlayers = 1;
        modifier_fanClub = false;
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            //Debug.Log("GameSettings already has an instance, one was destoyed");
            Destroy(gameObject);
        }
    }
}
