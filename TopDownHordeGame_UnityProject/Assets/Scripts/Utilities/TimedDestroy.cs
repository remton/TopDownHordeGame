using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float destroyTime;
    // Start is called before the first frame update
    protected void Start(){
        Debug.Log("Object is being destroyed in TimedDestroy Start() method.");
        Destroy(gameObject, destroyTime);
    }
}
