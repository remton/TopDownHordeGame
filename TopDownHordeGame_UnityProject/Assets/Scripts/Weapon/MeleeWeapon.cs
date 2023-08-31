using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Timer))]
public class MeleeWeapon : Weapon
{
    [SerializeField] private float reachLength; // how far the weapon moves forward before swinging
    [SerializeField] private int swingAngle; // angle that the weapon sweeps while swinging (in degrees)
    [SerializeField] private float swingTime; // time that the weapon takes to complete its swing
    private Timer timer;
    private bool isAttacking = false; // whether the weapon is currently being swung
    private Vector3 attackStartDirection; // normalized direction the player was facing when the attack started (used for attack swing)

    /* TO DO:
     * Make the weapon move forward, rotate, then move back on fire
     * Make the weapon damage enemies
     * Fix being able to start an attack in the middle of another attack
     */
    public override void Fire(GameObject player, Vector2 direction) // put melee weapon swing here
    {
        if (!isAttacking)
        {
            isAttacking = true;
            timer.CreateTimer(swingTime, EndAttack);
        }
        // start of attack
        attackStartDirection = owner.GetComponent<PlayerMovement>().GetCurrentLookDir().normalized;
    }

    public override void Reload() // prevents Weapon::Reload() from running
    {
        
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    public override void Update() // overrided Update() so that the weapon's position wouldn't be messed up during a swing
    {
        if (!isAttacking)
        {
            base.Update();
        } else
        {
            transform.position = owner.transform.position + (reachLength * attackStartDirection); // modified PlayerWeaponControl::Update() to stop MeleeWeapons from following Reticle while attacking
        }
    }

    protected override void Awake()
    {
        inMag = 1; // this is so the player doesn't have to "reload" when they first get the weapon
        base.Awake();
        timer = GetComponent<Timer>();
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    public void SetIsAttacking(bool newVal)
    {
        isAttacking = newVal;
    }

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
