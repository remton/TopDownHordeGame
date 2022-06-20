/*HitBoxController.cs
 * Author: Remington Ward
 * Date: 1/24/2022
 * This script is built for the Unity Game Engine. It provides an easy funcionality for other scripts to have a hitbox
 * Any object with this script must have a collider set to trigger that will collide with all needed objects
 * This script has a list of tags (triggerTags) that this hitbox will be affectd by.
 * When an object enters the event EventObjEnter will be onvoked. EventObjExit will likewise be invoked on an objects exit. 
 * These events will pass the object as a parameter.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour {
    // Events only triggered when an object with one of these tags is detected
    [Header("Capital sensative tags!")]
    public List<string> triggerTags = new List<string>(); 

    //Events for when an object enters and exits (remember to unsubscribe if necessary!)
    public delegate void ObjEnter(GameObject obj);
    public event ObjEnter EventObjEnter;
    public delegate void ObjExit(GameObject obj);
    public event ObjExit EventObjExit;

    //If this hitbox should currently be working
    public void SetActive(bool b) { active = b; }
    public bool GetActive() { return active; }

    /// <summary> Calls EventObjEnter for every obj already in the hitbox that has an active tag</summary>
    public void ForceEntry() {
        foreach (GameObject obj in objsInBox) {
            if (obj != null) {
                if(Utilities.CompareTags(obj, triggerTags))
                    if (EventObjEnter != null) { EventObjEnter.Invoke(obj); }
            }
        }
    }
    /// <summary> Returns all current objects in the hitbox with tags </summary>
    public List<GameObject> Hits() {
        List<GameObject> returnList = new List<GameObject>();
        foreach (GameObject obj in objsInBox) {
            foreach (string tag in triggerTags) {
                if (obj.tag == tag)
                    returnList.Add(obj);
            }
        }
        return returnList;
    }

    // ----- Private -----

    private bool active = true;
    private List<GameObject> objsInBox = new List<GameObject>();

    // These functions are called by unity when a gameObject enters and exits the trigger collider respectively
    private void OnTriggerEnter2D(Collider2D collision) {
        objsInBox.Add(collision.gameObject);
        if (!active) return;
        if (Utilities.CompareTags(collision.gameObject, triggerTags))
            if (EventObjEnter != null) { EventObjEnter.Invoke(collision.gameObject); }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        objsInBox.Remove(collision.gameObject);
        if (!active) return;
        for (int i = 0; i < triggerTags.Count; i++) {
            if (collision.CompareTag(triggerTags[i])) {
                if (EventObjExit != null) { EventObjExit.Invoke(collision.gameObject); }
            }
        }
    }
}
