using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] GameObject insideNode;

    [SerializeField] private int health;
    private bool isOpen = false;
    public void Damage(int d) {
        if (health <= 0)
            isOpen = true;
        else health -= d;
    }

    public void Heal(int h) {
        health += h;
        isOpen = false;
    }

    public bool GetIsOpen() {
        return isOpen;
    }

    public void MoveToInside(GameObject obj) {
        obj.transform.position = new Vector3(insideNode.transform.position.x, insideNode.transform.position.y, obj.transform.position.z);
    }

}
