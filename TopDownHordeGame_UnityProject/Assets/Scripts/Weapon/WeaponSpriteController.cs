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
    [Command(requiresAuthority = false)]
    public void UpdateDirection(Vector2 dir) {
        SetDirection_RPC(dir);
    }
    [ClientRpc]
    private void SetDirection_RPC(Vector2 dir) {
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
    public void SetLaser(bool power) {
        if (power)
            myLaser.GetComponent<LaserSight>().TurnOn();
        else
            myLaser.GetComponent<LaserSight>().TurnOff();
    }
    public void ActivateSprite() {
        Debug.Log(gameObject.name + ": ACTIVATE SPRITE!!");
        spriteObj.SetActive(true);
    }
    public void DeactivateSprite() {
        Debug.Log(gameObject.name + ": DEACTIVATE SPRITE!!");
        spriteObj.SetActive(false);
    }

    private void Awake() {
        xScaleMagnitude = transform.localScale.x;
    }
}
