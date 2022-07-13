using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponSpriteController : NetworkBehaviour
{
    private float xScaleMagnitude;

    [SerializeField] private GameObject myLaser;
    [SerializeField] private GameObject barralEndObj;
    public GameObject spriteObj;

    private Timer timer;
    public float sendInterval;

    private void UpdateDirection(Vector2 dir) {
        if (myLaser != null)
            myLaser.GetComponent<LaserSight>().DrawLaser(dir);
        if (dir.x < 0) {
            transform.localScale = new Vector3(xScaleMagnitude * -1, transform.localScale.y, transform.localScale.z);
            dir.y *= -1;
        }
        else {
            transform.localScale = new Vector3(xScaleMagnitude, transform.localScale.y, transform.localScale.z);
        }
        Vector2 lookAtDir = new Vector3(Mathf.Abs(dir.x), dir.y, transform.position.z);
        gameObject.transform.right = lookAtDir;
    }

    public Vector3 BarrelEndPosition() {
        return barralEndObj.transform.position;
    }
    [Command(requiresAuthority = false)]
    public void SetLaser(bool power) {
        SetLaserRPC(power);
    }
    [ClientRpc]
    private void SetLaserRPC(bool power) {
        if (power)
            myLaser.GetComponent<LaserSight>().TurnOn();
        else
            myLaser.GetComponent<LaserSight>().TurnOff();
    }

    public void ActivateSprite() {
        //Debug.Log(gameObject.name + ": ACTIVATE SPRITE!!");
        spriteObj.SetActive(true);
    }
    public void DeactivateSprite() {
        //Debug.Log(gameObject.name + ": DEACTIVATE SPRITE!!");
        spriteObj.SetActive(false);
    }

    private void Awake() {
        timer = GetComponent<Timer>();
        xScaleMagnitude = transform.localScale.x;
    }


    // NETWORKING 
    private Vector2 lastDirectionRecieved;
    private bool sendOnIntervalEnd = false;
    private bool canSend = true;
    [Client]
    public void DirectionChanged(Vector2 dir) {
        UpdateDirection(dir);
        lastDirectionRecieved = dir;
        if (canSend) {
            SendDirectionCommand(dir);
            timer.CreateTimer(sendInterval, SendIntervalEnd);
        }
        else {
            sendOnIntervalEnd = true;
        }
    }
    [Client]
    private void SendIntervalEnd() {
        if (sendOnIntervalEnd) {
            sendOnIntervalEnd = false;
            SendDirectionCommand(lastDirectionRecieved);
            timer.CreateTimer(sendInterval, SendIntervalEnd);
        }
        else {
            canSend = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void SendDirectionCommand(Vector2 dir) {
        RecieveDirection(dir);
        if(enabled && hasAuthority)
            timer.CreateTimer(sendInterval, SendIntervalEnd);
    }
    [ClientRpc]
    public void RecieveDirection(Vector2 dir) {
        lastDirectionRecieved = dir;
        UpdateDirection(dir);
    }
}
