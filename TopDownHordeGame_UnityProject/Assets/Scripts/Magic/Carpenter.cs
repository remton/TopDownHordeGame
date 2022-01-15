using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Carpenter : MonoBehaviour
{
    public int time;
    public MagicType type;
    private List<Window> windows;
    private GameObject[] players;

    private void Awake()
    {
        GetComponent<HitBoxController>().EventObjEnter += Touch;
        GameObject.FindGameObjectsWithTag("Player");
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void Touch(GameObject player)
    {
        GetComponent<HitBoxController>().EventObjEnter -= Touch;
        Debug.Log("Power Up: " + name + " spawned");
        windows = RoundController.instance.GetActiveWindows();
        foreach (Window current in windows)
        {
            current.GetComponent<Window>().FullRepair();
        }
        foreach (GameObject current in players)
        {
            current.GetComponent<PlayerStats>().AddMoney(1200);
        }
        Stop();
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void Stop()
    {
        Debug.Log("Power Up: " + name + " lost");
        Destroy(gameObject);
    }
}
