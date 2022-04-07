using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpriteController : MonoBehaviour
{
    [SerializeField] private GameObject barralEndObj;
    [SerializeField] private GameObject spriteObj;
    public void UpdateDirection(Vector2 dir) {
        Vector3 lookAtDir = new Vector3(transform.position.x + dir.x, transform.position.y + dir.y, transform.position.z);
        gameObject.transform.LookAt(lookAtDir);
        if((dir.x < 0) != (transform.localScale.y < 0)) {
            spriteObj.transform.localScale = new Vector3(spriteObj.transform.localScale.x, spriteObj.transform.localScale.y * -1, spriteObj.transform.localScale.z);
        }
    }

    public Vector3 BarrelEndPosition() {
        return barralEndObj.transform.position;
    }
}
