using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengesHolder : MonoBehaviour
{
    private static ChallengesHolder instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }
}
