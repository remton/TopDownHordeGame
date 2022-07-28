using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    private LineRenderer line;
    private bool isOn;
    [SerializeField] private float maxRange;
    private void Awake() {
        line = GetComponent<LineRenderer>();
        TurnOn();
    }
    public void SetRange(float range) {
        maxRange = range;
    }
    public void TurnOff() {
        line.enabled = false;
        isOn = false;
    }
    public void TurnOn() {
        line.enabled = true;
        isOn = true;
    }
    public void DrawLaser(Vector2 direction) {
        if (!isOn)
            return;
        //Debug.Log("Draw");
        direction.Normalize();
        string[] mask = { "BulletCollider", "ZombieHitbox", "Door" };
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, maxRange, LayerMask.GetMask(mask));
        Vector3 hitpos;
        if (hitInfo == true) {
            hitpos = new Vector3(hitInfo.point.x, hitInfo.point.y, transform.position.z);
        }
        else {
            hitpos = new Vector3(transform.position.x + direction.x*maxRange, 
                transform.position.y + direction.y * maxRange, transform.position.z);
        }
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hitpos);
    }

}
