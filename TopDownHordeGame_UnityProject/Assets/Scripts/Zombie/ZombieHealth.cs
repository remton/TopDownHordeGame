using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    //public ParticleSystem particle;
    //public GameObject hitEffectObj;
    public GameObject deathEffectObj;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem dieParticles;
    public bool givesMoney;

    public delegate void onDeath();
    public event onDeath EventOnDeath;
    [SerializeField] private AudioClip[] hurtsounds;
    private int chance;
    private bool killed = false;
    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        health = maxHealth;
    }
    public float GetMaxHealth() {
        return maxHealth;
    }

    private float maxHealth = 1;
    private float health;

    //public delegate void OnHealthChange(float newHealth);
    //public event OnHealthChange EventHealthChange;

    public float GetHealthRatio() {
        return (float)health / maxHealth;
    }

    public bool isDead(){
        return health <= 0;
    }

    private void Start(){
        health = maxHealth;
    }

    public void Damage(float amount)
    {
        if (killed)
            return;
        health -= amount;
        if (health <= 0)
            Kill();
        hitParticles.Play();
        chance = Random.Range(0,hurtsounds.Length);
        //SoundPlayer.Play(hurtsounds[chance], transform.position);
        AudioManager.instance.PlaySound(hurtsounds[chance], transform.position);
        //if (EventHealthChange != null) { EventHealthChange.Invoke(health); }
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
        //if (EventHealthChange != null) { EventHealthChange.Invoke(health); }
    }

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
