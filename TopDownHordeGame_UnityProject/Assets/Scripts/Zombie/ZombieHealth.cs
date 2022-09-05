using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ZombieHealth : NetworkBehaviour
{
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem dieParticles;
    [SerializeField] private float moneyEffectVerticalOffset;

    public bool givesMoney;

    public delegate void onDeath();
    public event onDeath EventOnDeath;
    public bool DontDestroyOnDeath = false;

    [SerializeField] private AudioClip[] hurtsounds;
    private int chance;
    private bool killed = false;
    private bool hurtSoundCooldown = false;

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

    private void ResetHurtSoundCooldown(){
        hurtSoundCooldown = false;
    }

    [Command(requiresAuthority = false)]
    public void DamageCMD(float amount) {
        if (killed)
            return;
        health -= amount;
        if (health <= 0)
            Kill();
        PlayDamageEffects();
    }

    [Command(requiresAuthority = false)]
    public void DamageCMD(float amount, GameObject damager) {
        if (killed)
            return; 
        health -= amount;
        if(!hurtSoundCooldown){
            PlayDamageEffects();
            hurtSoundCooldown = true;
            GetComponent<Timer>().CreateTimer(1, ResetHurtSoundCooldown);
        }

        if (damager != null) {
            if (damager.HasComponent<Player>()) {
                int payForHit = GetComponent<ZombieAI>().payForHit;
                int payForKill = GetComponent<ZombieAI>().payForKill;
                int payAmount = payForHit;
                damager.GetComponent<PlayerStats>().AddMoney(payForHit);
                if (health <= 0) {
                    damager.GetComponent<PlayerStats>().AddMoney(payForKill);
                    payAmount += payForKill;
                    damager.GetComponent<PlayerStats>().AddKill(GetComponent<ZombieAI>().type);
                }
                MoneyEffectManager.instance.CreateEffect(damager, transform.position + new Vector3(0, moneyEffectVerticalOffset, 0), payAmount);
            }
        }
        if (health <= 0) {
            Kill();
        }
    }

    [ClientRpc]
    private void PlayDamageEffects() {
        hitParticles.Play();
        chance = Random.Range(0, hurtsounds.Length);
        AudioManager.instance.PlaySound(hurtsounds[chance]);
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    [Server]
    private void PlayDeathEffects() {
        dieParticles.gameObject.transform.parent = null;
        Destroy(dieParticles.gameObject, 1);
        dieParticles.Play();
        if (EventOnDeath != null) EventOnDeath.Invoke();
        PlayDeathEffectRPC();
    }
    [ClientRpc]
    private void PlayDeathEffectRPC() {
        if (isServer)
            return;

        dieParticles.gameObject.transform.parent = null;
        Destroy(dieParticles.gameObject, 1);
        dieParticles.Play();
        if (EventOnDeath != null) EventOnDeath.Invoke();
    }


    [Server]
    public void Kill()
    {
        Vector3 myLocation = transform.position;
        MagicController.instance.MagicDrop(myLocation);

        if (!gameObject.HasComponent<DoNotCount>())
            RoundController.instance.OnZombieDies();
        if (gameObject.HasComponent<BiggestFanDeath>())
            GetComponent<BiggestFanDeath>().Explode();
        killed = true;

        PlayDeathEffects();

        if (!DontDestroyOnDeath)
            NetworkServer.Destroy(gameObject);
    }
}
