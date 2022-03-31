using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaMeter : MonoBehaviour
{
    public GameObject player;
    public PlayerMovement movement;
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;
    Vector3 offset;

    Quaternion rotation;
    private void Awake() {
        rotation = transform.rotation;
    }

    private void Start() {
        offset = transform.position - player.transform.position;
        transform.parent = null;
        transform.rotation = rotation;
    }

    private void Update() {
        float staminaRatio = movement.GetStaminaRatio();
        if(staminaRatio >= 1) {
            spriteRenderer.enabled = false;
        }
        else {
            transform.position = player.transform.position + offset;
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = sprites[Mathf.RoundToInt(staminaRatio * (sprites.Count-1))];
        }
        
    }
}
