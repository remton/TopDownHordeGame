using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockEyeAnimHelper : MonoBehaviour
{
    public delegate void Throw();
    public event Throw EventThrow;
    public void AnimEvent_Throw() {
        if (EventThrow != null) { EventThrow.Invoke(); }
    }
}
