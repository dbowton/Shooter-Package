using Assets.Scripts.Items.Weapons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected float currentHealth = 40;
    [SerializeField] protected float maxHealth = 40;

    [SerializeField] protected AudioSource hurtSound;
    [SerializeField] protected AudioSource deathSound;
    [SerializeField] protected AudioSource healSound;

    [SerializeField] protected GameObject deathPrefab;

    [SerializeField] protected bool killRootCharacter = false;
    [SerializeField] protected bool destroyOnDeath = false;
    [SerializeField][Tooltip("only used with destryOnDeath")] protected float killTimer = 0f;

    [SerializeField][Tooltip("set to < 0 to keep objects")] float destroyDestroyedPrefabTime = -1f;

    [HideInInspector] public float MaxHealth {get { return maxHealth; }}
    [HideInInspector] public float CurrentHealth {get { return currentHealth; }}

    private List<Damage> damages = new List<Damage>();

    [HideInInspector] public bool active = true;

    public void Update()
    {
        if (!active) return;

        List<Damage> tempDamages = new List<Damage>();
        damages.ForEach(x => tempDamages.Add(x));

        tempDamages.ForEach(
            x =>
            {
                currentHealth = Mathf.Min(currentHealth - x.damage * Time.deltaTime, maxHealth);
                x.duration -= Time.deltaTime;

                if (x.duration <= 0) damages.Remove(x);
                if (currentHealth <= 0) Die();
            });
    }

    public void Damage(Damage damage, float dmgMulti = 1)
    {
        if(gameObject.transform.TryGetComponent<Character>(out Character character)) character.Hit();

        damage.damage *= dmgMulti;

        if(damage.duration == 0)
        {
            currentHealth = Mathf.Min(currentHealth - damage.damage, maxHealth);

            if (currentHealth > 0)
            {
                if (hurtSound && !hurtSound.isPlaying) AudioSource.PlayClipAtPoint(hurtSound.clip, transform.position, hurtSound.volume);
            }
            else Die();
        }
        else
        {
            damages.Add(damage);
            damages = (List<Damage>)damages.OrderBy(x => x.damage);
        }
    }

    public void Heal(float hp)
    {
        currentHealth = Mathf.Min(currentHealth + hp, maxHealth);
        if (healSound && !healSound.isPlaying) AudioSource.PlayClipAtPoint(healSound.clip, transform.position, healSound.volume);
    }

    public void Die()
    {
        if (deathSound && !deathSound.isPlaying) AudioSource.PlayClipAtPoint(deathSound.clip, transform.position, deathSound.volume);
        if (deathPrefab)
        {
            GameObject destroyedObject = Instantiate(deathPrefab, transform.position, transform.rotation);

            if (destroyDestroyedPrefabTime > 0) Destroy(destroyedObject, destroyDestroyedPrefabTime);
        }
            
          
        if(killRootCharacter && transform.TryGetComponent<Character>(out Character character)) character.Die();
        if (destroyOnDeath) Destroy(gameObject, killTimer);
    }
}
