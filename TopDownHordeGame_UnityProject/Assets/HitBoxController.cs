using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour {
    [SerializeField] private List<string> activeTags = new List<string>(); // Events only triggered when an objet with one of these tags is detected
    private bool active;
    private List<GameObject> objsInBox = new List<GameObject>();

    public void SetActive(bool b) { active = b; }
    public bool GetActive() { return active; }

    /// <summary>
    /// Calls EventObjEnter for every obj already in the hitbox
    /// </summary>
    public void ForceEntry() {
        foreach (GameObject obj in objsInBox) {
            if(Utilities.CompareTags(obj, activeTags))
                if (EventObjEnter != null) { EventObjEnter.Invoke(obj); }
        }
    }

    public delegate void ObjEnter(GameObject obj);
    public event ObjEnter EventObjEnter;

    private void OnTriggerEnter2D(Collider2D collision) {
        objsInBox.Add(collision.gameObject);
        if (!active) return;
        if (Utilities.CompareTags(collision.gameObject, activeTags))
            if (EventObjEnter != null) { EventObjEnter.Invoke(collision.gameObject); }
    }

    public delegate void ObjExit(GameObject obj);
    public event ObjExit EventObjExit;

    private void OnTriggerExit2D(Collider2D collision) {
        objsInBox.Remove(collision.gameObject);
        if (!active) return;
        for (int i = 0; i < activeTags.Count; i++) {
            if (collision.CompareTag(activeTags[i])) {
                if (EventObjExit != null) { EventObjExit.Invoke(collision.gameObject); }
            }
        }
    }
}
