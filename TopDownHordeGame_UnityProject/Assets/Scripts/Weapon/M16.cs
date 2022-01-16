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
        if (GetFireDeley() <= burstTime)
            burstTime = GetFireDeley();
        timebetweenShots = burstTime/burstCount;

    }

    public override void Fire(GameObject player, Vector2 direction){
        currPlayer = player;
        shotsLeftInBurst = burstCount;
    }
    private void Update() {
        if (shotsLeftInBurst > 0) {
            if(timeUntilNextShot <= 0) {
                FireShot();
                shotsLeftInBurst--;
                timeUntilNextShot = timebetweenShots;
            }
            else {
                timeUntilNextShot -= Time.deltaTime;
            }
        }
    }
    private void FireShot() {
        GameObject player = currPlayer;
        Vector2 direction = currPlayer.GetComponent<PlayerMovement>().GetCurrentLookDir();
        base.Fire(player, direction);
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x);
        float angleDiff;
        Vector2 fireDir;
        angleDiff = Random.Range((baseAngle - spreadAngle / 2), (baseAngle + spreadAngle / 2));
        fireDir = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
        FireShot(player, fireDir);
    }
}