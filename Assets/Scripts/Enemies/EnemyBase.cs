using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] protected GameObject player;

    [SerializeField] protected float setHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] public int contactDamage;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected GameObject itemDrop;
    public Vector3 initialPos;

    private protected bool invincible;
    [SerializeField] protected float invincibilityTime;
    [SerializeField] protected SpriteRenderer rend;

    [SerializeField] protected bool isAggro;
    [SerializeField] protected bool attacking;

    //Material
    [SerializeField] protected Material invincibleMaterial;
    [SerializeField] protected Material normalMaterial;

    [SerializeField] protected bool dead;
    //Particles
    [SerializeField] protected GameObject deathParticles;
    [SerializeField] protected GameObject[] secondaryParticles;

    //Audio
    [SerializeField] protected AudioClip[] damageSounds;
    [SerializeField] protected AudioClip[] deathSounds;
    [SerializeField] protected AudioClip[] attackSounds;
    [SerializeField] protected AudioClip[] telegraphSounds;
    [SerializeField] protected AudioClip[] idleSounds;


    [SerializeField] protected float enemyVol = 1f;
    

    protected bool lpIsPlaying = false;
    [SerializeField] bool killEligible = true;

    protected virtual void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rend = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");

      

        dead = false;
        currentHealth = setHealth;
        contactDamage = 1;

        initialPos = transform.position;

        DeathPart(false);

    }

    public virtual void TakeDamage(float damage, Vector2 knockback)
    {
        if (invincible != true)
        {
            currentHealth -= damage;
            rb.AddForce(knockback * 2.85f, ForceMode2D.Impulse);
            StartCoroutine("Invincibility");
        }
        
 
        if (currentHealth <= 0)
        {
            dead = true;
            StartCoroutine("Death");
        }
        return;
    }

    protected virtual IEnumerator Invincibility()
    {
        invincible = true;
        rend.material = invincibleMaterial;
        rend.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;
        rend.material = normalMaterial;

    }

    protected virtual IEnumerator Death()
    {
        dead = true;

        rend.enabled = false;
        invincible = false;
        Collider2D[] colliders = this.GetComponents<Collider2D>();

        DeathPart(true);

        SoundFXManager.instance.PlayRandomSoundEffectClip(deathSounds, transform.position, 1f);

        
       

        yield return null;

        gameObject.SetActive(false);

        player.GetComponent<Respawn>().deadEnemies.Add(this.gameObject);

        if (killEligible == true)
        {
            player.GetComponent<Shooting>().kills += 1;

            if (player.GetComponent<Ultimate>().ultimateState == true)
            {
                if (player.GetComponent<Health>().currentHealth < player.GetComponent<Health>().setHealth)
                {
                    player.GetComponent<Health>().ReplenishHealth(1);
                }
                else
                {
                    player.GetComponent<RunAndJump>().AddFuel(player.GetComponent<RunAndJump>().maxFuel);
                }

            }
        }


    }

    protected void DeathPart(bool die)
    {
        if (deathParticles != null)
        {
            ParticleSystem part = deathParticles.GetComponent<ParticleSystem>();

            if (die)
            {
                //remove from this object so its done turned invisible
                deathParticles.transform.SetParent(null);
                part.Stop();
                part.Play();

            } else
            {
                deathParticles.transform.SetParent(this.transform);
                deathParticles.transform.localPosition = Vector3.zero;
                part.Stop();
            }

        }

        if (secondaryParticles != null)
        {
            foreach (GameObject effect in secondaryParticles)
            {
                effect.SetActive(!die);
                Debug.Log("Set Secondary Particles to be enabled: " + !die);
            }

        }

    }

    /*
    public virtual void PlaySoundEffect(AudioClip clip, bool loop, bool overwrite, bool deathSource)
    {
        if(!deathSource)
        {
            if (overwrite)
            {
                enemySound.Stop();

            }

            enemySound.clip = clip;
            enemySound.loop = loop;
            enemySound.Play();

        } else
        {
            if (overwrite)
            {
                damageSound.Stop();

            }

            damageSound.clip = clip;
            damageSound.loop = loop;
            damageSound.Play();
        }

        
        
    }*/

    /*
    void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.gameObject.tag == "Parrier")
        {
            Vector2 parrierPos = other.gameObject.transform.position;
            Vector2 thisPos = transform.position;
            Vector2 dist = thisPos - parrierPos;

            TakeDamage(1, dist);
            player.GetComponent<RunAndJump>().AddFuel(.15f);
        }
        
    }
    */
    public virtual void RespawnEnemy(Respawn resp)
    {
        if (dead)
        {
            rend.material = normalMaterial;

            this.gameObject.transform.position = initialPos;
            isAggro = false;

            currentHealth = setHealth;
           
            attacking = false;
            dead = false;

            Collider2D[] colliders = this.GetComponents<Collider2D>();
            AudioSource[] sounds = this.GetComponents<AudioSource>();
            

            foreach (Collider2D coll in colliders)
            {
                coll.enabled = false;
            }
            foreach (AudioSource sound in sounds)
            {
                sound.Stop();
            }

            DeathPart(false);

            rend.enabled = true;
            this.GetComponent<Collider2D>().enabled = true;
            
        }   
    }

    public virtual void ResetEnemy(Respawn resp)
    {
        //Debug.Log("called base");
        if (isAggro)
        {
            this.gameObject.transform.position = initialPos;
            isAggro = false;
          

            currentHealth = setHealth;
            attacking = false;
        }

    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += RespawnEnemy;
        Respawn.OnRespawnEvent += ResetEnemy;
    }

    protected virtual IEnumerator LoopSFX(AudioClip[] clips)
    {
        if (!lpIsPlaying)
        {
            lpIsPlaying = true;
            int soundID = Random.Range(0, clips.Length);

            SoundFXManager.instance.PlayProximitySoundEffectClip(clips[soundID], gameObject, 1f);
            yield return new WaitForSeconds(clips[soundID].length);
            lpIsPlaying = false;
        }

    }

}
