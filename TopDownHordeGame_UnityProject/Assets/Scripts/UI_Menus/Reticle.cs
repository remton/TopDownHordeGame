using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sprite;
    /// <summary> reload animation of the reticle (used only to get the length of the anim) </summary>
    [SerializeField] private AnimationClip reloadAnim;
    [SerializeField] private AnimationClip swapAnim;
    public GameObject player;
    private void Start() {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        player.GetComponent<PlayerWeaponControl>().EventReloadCalled += PlayReloadAnim;
        player.GetComponent<PlayerWeaponControl>().EventSwapCalled += PlayReloadAnim;
    }

    public void Activate(bool b) {
        sprite.enabled = b;
    }

    //plays the reload animation over so many seconds
    public void PlayReloadAnim(float secondsForReload) {
        float reloadSpeedMult = reloadAnim.length / secondsForReload;
        animator.SetFloat("ReloadSpeedMult", reloadSpeedMult);
        animator.SetTrigger("Reload");
    }

    private void OnDestroy() {
        if (player) {
            player.GetComponent<PlayerWeaponControl>().EventReloadCalled -= PlayReloadAnim;
            player.GetComponent<PlayerWeaponControl>().EventSwapCalled -= PlayReloadAnim;
        }
    }
}
