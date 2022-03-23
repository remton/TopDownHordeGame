using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    private Animator animator;
    /// <summary> reload animation of the reticle (used only to get the length of the anim) </summary>
    [SerializeField] private AnimationClip reloadAnim;
    [SerializeField] private AnimationClip swapAnim;
    public GameObject player;
    private void Start() {
        animator = GetComponent<Animator>();
        player.GetComponent<PlayerWeaponControl>().EventReloadCalled += PlayReloadAnim;
        player.GetComponent<PlayerWeaponControl>().EventSwapCalled += PlayReloadAnim;
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
