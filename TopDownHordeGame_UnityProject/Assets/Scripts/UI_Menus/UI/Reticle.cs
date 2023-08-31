using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Timer))]
public class Reticle : MonoBehaviour
{
    [SerializeField] private GameObject ammoTxtObj;
    private Text ammoTxt;
    public float ammoUptime; //time before ammo text disappears after ammo is changed
    private Timer timer;
    private System.Guid ammoTimer = System.Guid.Empty;
    

    private Animator animator;
    private SpriteRenderer sprite;
    /// <summary> reload animation of the reticle (used only to get the length of the anim) </summary>
    [SerializeField] private AnimationClip reloadAnim;
    [SerializeField] private AnimationClip swapAnim;
    private GameObject player;
    public void Init(GameObject newPlayer) {
        player = newPlayer;
        timer = GetComponent<Timer>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        ammoTxt = ammoTxtObj.GetComponentInChildren<Text>();
        PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
        weaponControl.EventReloadCalled += PlayReloadAnim;
        weaponControl.EventStartSwapWeapon += PlayReloadAnim;
        weaponControl.EventAmmoChanged += AmmoChange;
    }

    public void AmmoChange(int mag, int reserve) {
        ammoTxt.text = mag.ToString() + " / " + (reserve==-1 ? "inf" : reserve.ToString());
        DisplayAmmoText();
        timer.KillTimer(ammoTimer);
        ammoTimer = timer.CreateTimer(ammoUptime, HideAmmoText);
    }
    public void HideAmmoText() {
        ammoTxtObj.SetActive(false);
    }

    public void DisplayAmmoText()
    {
        Debug.Log("displayAmmoTxt = " + player.GetComponent<PlayerWeaponControl>().GetEquippedWeapon().displayAmmoTxt);
        if (!player.GetComponent<PlayerWeaponControl>().GetEquippedWeapon().displayAmmoTxt)
        {
            Debug.Log("Ammo text was not displayed.");
            return;
        }
        ammoTxtObj.SetActive(true);
    }

    public void Activate(bool b) {
        sprite.enabled = b;
    }

    //plays the reload animation over so many seconds
    public void PlayReloadAnim(float secondsForReload) {
        timer.KillTimer(ammoTimer);
        HideAmmoText();
        float reloadSpeedMult = reloadAnim.length / secondsForReload;
        animator.SetFloat("ReloadSpeedMult", reloadSpeedMult);
        animator.SetTrigger("Reload");
    }

    private void OnDestroy() {
        if (player) {
            PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
            weaponControl.EventReloadCalled -= PlayReloadAnim;
            weaponControl.EventStartSwapWeapon -= PlayReloadAnim;
            weaponControl.EventAmmoChanged -= AmmoChange;
        }
    }
}
