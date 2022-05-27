using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M16 : Weapon
{
    public float spreadAngle;
    public float burstTime;
    public int burstCount = 3;

    private float timeUntilNextShot;
    private float timebetweenShots;

    private int shotsLeftInBurst;
    private GameObject currPlayer;

    private void Awake() {
        fireDeley += burstTime;
        timebetweenShots = burstTime/burstCount;
    }

    public override void Fire(GameObject player, Vector2 direction){
        currPlayer = player;
        shotsLeftInBurst = burstCount;
    }
    private void Update() {
        if (shotsLeftInBurst > 0) {
            if(timeUntilNextShot <= 0) {
                base.Fire(currPlayer, currPlayer.GetComponent<PlayerMovement>().GetCurrentLookDir());
                FireShot(currPlayer, currPlayer.GetComponent<PlayerMovement>().GetCurrentLookDir(), spreadAngle);
                AudioManager.instance.PlaySound(shootSound);
                currPlayer.GetComponent<PlayerWeaponControl>().UpdateVisuals();
                shotsLeftInBurst--;
                timeUntilNextShot = timebetweenShots;
            }
            else {
                timeUntilNextShot -= Time.deltaTime;
            }
        }
    }
}