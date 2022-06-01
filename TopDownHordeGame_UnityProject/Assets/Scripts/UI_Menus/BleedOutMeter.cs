using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedOutMeter : MonoBehaviour
{
    public GameObject player;
    public PlayerHealth health;
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites;

    private void Update() {
        if (!health.IsBleedingOut()) {
            spriteRenderer.enabled = false;
        }
        else {
            float bleedOutRatio = health.GetBleedOutTimeRatio();
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = sprites[Mathf.RoundToInt(bleedOutRatio * (sprites.Count-1))];
        }
    }
}
