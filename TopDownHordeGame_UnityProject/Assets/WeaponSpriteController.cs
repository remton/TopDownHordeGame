using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpriteController : MonoBehaviour
{
    public GameObject laserSightPrefab;
    private GameObject myLaser;
    [SerializeField] private GameObject barralEndObj;
    [SerializeField] private GameObject spriteObj;
    public void UpdateDirection(Vector2 dir) {
        //Debug.DrawLine(BarrelEndPosition(), BarrelEndPosition() + new Vector3(dir.x, dir.y, transform.position.z).normalized * 100, Color.red, 0, false);
        if (myLaser != null)
            myLaser.GetComponent<LaserSight>().DrawLaser(dir);
        if (dir.x < 0)
            dir.y *= -1;
        Vector2 lookAtDir = new Vector3(Mathf.Abs(dir.x), dir.y, transform.position.z);
        gameObject.transform.right = lookAtDir;
    }
    public Vector3 BarrelEndPosition() {
        return barralEndObj.transform.position;
    }
    public void ActivateSprite() {
        spriteObj.SetActive(true);
        AttachLaserSight();
    }
    public void SetLaser(bool power) {
        if (myLaser == null)
            AttachLaserSight();
        if(power)
            myLaser.GetComponent<LaserSight>().TurnOn();
        else
            myLaser.GetComponent<LaserSight>().TurnOff();
    }

    public void DeactivateSprite() {
        RemoveLaserSight();
        spriteObj.SetActive(false);
    }
    public void AttachLaserSight() {
        myLaser = Instantiate(laserSightPrefab, barralEndObj.transform);
        myLaser.transform.position = BarrelEndPosition();
    }
    public void RemoveLaserSight() {
        if (myLaser != null) {
            Destroy(myLaser);
            myLaser = null;
        }
    }
}
