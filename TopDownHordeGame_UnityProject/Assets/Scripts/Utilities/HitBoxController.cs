using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour {
    [Header("Capital sensative tags")]
    public List<string> triggerTags = new List<string>(); // Events only triggered when an objet with one of these tags is detected
    private bool active = true;
    private List<GameObject> objsInBox = new List<GameObject>();

    public void SetActive(bool b) { active = b; }
    public bool GetActive() { return active; }

    /// <summary>Calls EventObjEnter for every obj already in the hitbox that has an active tag</summary>
    public void ForceEntry() {
        foreach (GameObject obj in objsInBox) {
            if (obj != null) {
                if(Utilities.CompareTags(obj, triggerTags))
                    if (EventObjEnter != null) { EventObjEnter.Invoke(obj); }
            }
        }
    }

    public List<GameObject> Hits() {
        return objsInBox;
    }

    public delegate void ObjEnter(GameObject obj);
    public event ObjEnter EventObjEnter;

    private void OnTriggerEnter2D(Collider2D collision) {
        objsInBox.Add(collision.gameObject);
        if (!active) return;
        if (Utilities.CompareTags(collision.gameObject, triggerTags))
            if (EventObjEnter != null) { EventObjEnter.Invoke(collision.gameObject); }
    }

    public delegate void ObjExit(GameObject obj);
    public event ObjExit EventObjExit;

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
