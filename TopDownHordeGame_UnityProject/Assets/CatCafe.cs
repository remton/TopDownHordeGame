using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCafe : MonoBehaviour
{
    const bool OPEN_ELAVATOR_ON_START = true;
    public GameObject elavatorAreaLockTrigger;
    public GameObject elavatorCover;

    private void Start() {
        if (OPEN_ELAVATOR_ON_START) {
            OpenElavatorArea();
        }
    }

    public void OpenElavatorArea() {
        elavatorCover.SetActive(false);
    }
}
