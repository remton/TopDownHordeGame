using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSettings : MonoBehaviour
{
    public int numPlayers;
    public static GameSettings instance;
    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Debug.Log("GameSettings already has an instance, one was destoyed");
            Destroy(gameObject);
        }
    }
}
