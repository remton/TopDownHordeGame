using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private class LineEffect {

        public bool isFinished;
        public LineRenderer line;
        private float timeLeft;

        public LineEffect(LineRenderer copyLine, Vector2 pos1, Vector2 pos2, float dur) {
            line = copyLine;
            line.enabled = true;
            line.SetPosition(0, pos1);
            line.SetPosition(1, pos2);
            timeLeft = dur;
        }
        public void OnUpdate() {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0) {
                OnTimeUp();
            }
        }
        private void OnTimeUp() {
            isFinished = true;
            line.enabled = false;
        }
    }

    private List<LineEffect> lineEffects = new List<LineEffect>(); // The lines to render
    public float duration; // How long in seconds until the trail fades 
    public LineRenderer parentLine;
    public float maxDistance;

    public void CreateTrail(Vector2 pos1, Vector2 pos2) {
        lineEffects.Add(new LineEffect(parentLine, pos1, pos2, duration));
    }
    public void CreateTrailDir(Vector2 pos1, Vector2 direction) {
        //TODO: Create a line effect in the direction
    }

    private void Update() {
        for (int i = 0; i < lineEffects.Count; i++) {
            lineEffects[i].OnUpdate();
            if (lineEffects[i].isFinished) {
                lineEffects.RemoveAt(i);
            }
        }
    }
}
