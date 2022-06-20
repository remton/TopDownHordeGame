using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrail : NetworkBehaviour
{
    public bool isFinished;
    public LineRenderer line;
    private float timeLeft;
    private bool initRan = false;

    private void Awake() {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    [ClientRpc]
    public void Init(Vector2 pos1, Vector2 pos2, float dur) {
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);
        line.enabled = true;
        timeLeft = dur;

        initRan = true;
    }
    private void Update() {
        if (initRan) {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0) {
                OnTimeUp();
            }
        }

    }
    private void OnTimeUp() {
        isFinished = true;
        Destroy(gameObject);
    }
}
