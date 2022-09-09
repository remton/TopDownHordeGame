using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M16 : Weapon
{
    public float spreadAngle;
    public float burstTime;
    public int burstCount;

    private float timeUntilNextShot;
    private float timebetweenShots;

    private int shotsLeftInBurst;
    private GameObject currPlayer;

    protected override void Awake() {
        base.Awake();
        fireDeley += burstTime;
        timebetweenShots = burstTime/burstCount;
    }

    public override void Fire(GameObject player, Vector2 direction){
        currPlayer = player;
        shotsLeftInBurst = Mathf.Min(burstCount, inMag);
    }
    public override void Update() {
        base.Update();
        if (shotsLeftInBurst > 0) {
            if(timeUntilNextShot <= 0) {
                base.Fire(currPlayer, currPlayer.GetComponent<PlayerMovement>().GetCurrentLookDir());
                FireShot(currPlayer, currPlayer.GetComponent<PlayerMovement>().GetCurrentLookDir(), spreadAngle);
                PlayShootSoundForAll();
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