using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ZombieHealth : NetworkBehaviour
{
    public GameObject deathEffectObj;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem dieParticles;
    public bool givesMoney;

    public delegate void onDeath();
    public event onDeath EventOnDeath;
    [SerializeField] private AudioClip[] hurtsounds;
    private int chance;
    private bool killed = false;

    public delegate void HealthChange(float newHealth, float newMax);
    public event HealthChange EventHealthChanged;

    [SyncVar(hook = nameof(OnMaxHealthChange))]
    private float maxHealth;
    [SyncVar(hook = nameof(OnHealthChange))]
    private float health;
    [Client]
    private void OnHealthChange(float oldHealth, float newHealth) {
        health = newHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    [Client]
    private void OnMaxHealthChange(float oldMax, float newMax) {
        maxHealth = newMax;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }


    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        health = maxHealth;
    }
    public float GetMaxHealth() {
        return maxHealth;
    }


    public float GetHealthRatio() {
        return (float)health / maxHealth;
    }

    public bool IsDead(){
        return health <= 0;
    }

    private void Start(){
        health = maxHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    [Command(requiresAuthority = false)]
    public void Damage(float amount)
    {
        if (killed)
            return;
        health -= amount;
        if (health <= 0)
            Kill();
        hitParticles.Play();
        chance = Random.Range(0,hurtsounds.Length);
        AudioManager.instance.PlaySound(hurtsounds[chance]);
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    [Server]
    public void Kill()
    {
        dieParticles.gameObject.transform.parent = null;
        Destroy(dieParticles.gameObject, 1);
        dieParticles.Play();

        killed = true;
        Debug.Log(name + ": \"*dies\"");
        if (gameObject.HasComponent<BiggestFanDeath>())
            GetComponent<BiggestFanDeath>().Explode();
        if (!gameObject.HasComponent<DoNotCount>())
            RoundController.instance.ZombieDies();
        Vector3 myLocation = transform.position;
        MagicController.instance.MagicDrop(myLocation);
        if (EventOnDeath != null) EventOnDeath.Invoke();
        Destroy(gameObject);
    }
}
