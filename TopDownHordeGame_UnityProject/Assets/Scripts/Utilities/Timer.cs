using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public delegate void OnTimerEnd();
    public event OnTimerEnd EventTimerEnd;

    //private 

    //public void StartTimer(float time) {

    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
