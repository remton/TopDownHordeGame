using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpriteController : MonoBehaviour
{
    [SerializeField] private GameObject barralEndObj;
    [SerializeField] private GameObject spriteObj;
    public void UpdateDirection(Vector2 dir) {
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
    }
    public void DeactivateSprite() {
        spriteObj.SetActive(false);
    }
}
