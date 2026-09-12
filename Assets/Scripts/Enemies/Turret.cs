using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyBase
{
 // Start is called before the first frame update
  
    [SerializeField] private float aggroRange;
    [SerializeField] private int cannonDamage;
    [SerializeField] private Rigidbody2D rb_;
    [SerializeField] private LayerMask playerLayer;
 
    [SerializeField] float shootCooldown;

    [SerializeField] Transform firePoint;
    [SerializeField] GameObject cannonball;
    [SerializeField] float firePower;

    //[SerializeField] GameObject player;

    [SerializeField] Animator anim;

    [SerializeField] bool isElite;

    [SerializeField] LightControl lightC;
    [SerializeField] ParticleSystem shootParticle;

    [SerializeField] AudioClip sparkSFX;


    protected override void Start()
    {
        anim.CrossFade("TurretIdle", 0, 0);
        player = GameObject.FindWithTag("Player");

        initialPos = this.transform.position;

        if (isElite)
        {
            isAggro = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dist = this.transform.position - player.transform.position;
        //enemySound.volume = Mathf.InverseLerp(20, 10, dist.magnitude);
      //  enemyVol = Mathf.InverseLerp(20, 10, (player.transform.position - transform.position).magnitude);
        if (isAggro)
        {
            if (attacking == false && !dead)
            {
                StartCoroutine("Shoot");
            }
            
        } else {
            AggroCheck();
        }
    }

    IEnumerator Shoot()
    {

        attacking = true;

        anim.CrossFade("TurretShoot", 0, 0);

        int eff = Random.Range(3, 4);
        // PlaySoundEffect(enemySFX[eff], false, true, false);
        SoundFXManager.instance.PlayRandomProximitySoundEffectClip(attackSounds, gameObject, enemyVol);
        StartCoroutine(lightC.Pulse(0.6f, 2f));

        shootParticle.gameObject.transform.localScale = new Vector3(this.transform.localScale.x, 1, 1);
        shootParticle.Emit(15);

        GameObject proj = Instantiate(cannonball, firePoint.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().AddForce(transform.right * firePower * this.transform.localScale.x, ForceMode2D.Impulse);

        proj.GetComponent<CannonballControl>().damage = cannonDamage;
      //  Debug.Log("set preoj damage to " + cannonDamage);


        Destroy(proj, 3f);

     
        anim.CrossFade("TurretIdle", 0, 0);

        yield return new WaitForSeconds(shootCooldown);

        attacking = false;

       


    }

    
    void AggroCheck()
    {
        Transform playerPos = player.GetComponent<Transform>();
        
        float distance = Mathf.Sqrt(((playerPos.position.x - this.transform.position.x) * (playerPos.position.x - this.transform.position.x)) + ((playerPos.position.y - this.transform.position.y) * (playerPos.position.y - this.transform.position.y)));
 
        if (distance < aggroRange || isElite)
        {
            isAggro = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, aggroRange);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<CannonballControl>() != null)
        {
            if (other.gameObject.GetComponent<CannonballControl>().parried == true && !isElite)
            {

                // PlaySoundEffect(enemySFX[5], false, true, true);
                SoundFXManager.instance.PlayProximitySoundEffectClip(sparkSFX, gameObject, enemyVol);
                TakeDamage(currentHealth, Vector2.zero);


            } else if (other.gameObject.GetComponent<CannonballControl>().parried == true)
            {

                if (invincible != true)
                {
                    SoundFXManager.instance.PlayProximitySoundEffectClip(sparkSFX, gameObject, enemyVol);

                    currentHealth -= (setHealth / 3);

                    StartCoroutine("Invincibility");

                }


                if (currentHealth < 0 || currentHealth == 0)
                {
                    // PlaySoundEffect(enemySFX[5], false, true, false);
                    // PlaySoundEffect(enemySFX[2], false, true, true);
                    SoundFXManager.instance.PlayRandomProximitySoundEffectClip(deathSounds, gameObject, enemyVol);
                    dead = true;
                    StartCoroutine("Death");
                }
            }

        }
    }

    public override void TakeDamage(float damage, Vector2 knockback)
    {
        if (!isElite)
        {
            if (invincible != true)
            {
                int eff = Random.Range(0, 1);
                //  PlaySoundEffect(enemySFX[eff], false, true, true);
                SoundFXManager.instance.PlayRandomProximitySoundEffectClip(damageSounds, gameObject, enemyVol);
                currentHealth -= damage;

                StartCoroutine("Invincibility");

            }


            if (currentHealth < 0 || currentHealth == 0)
            {
                dead = true;

                // PlaySoundEffect(enemySFX[2], false, true, true);
                SoundFXManager.instance.PlayRandomProximitySoundEffectClip(deathSounds, gameObject, enemyVol);
                Debug.Log("DEAD!");
                if (itemDrop != null && Random.Range(1, 3) == 1)
                {
                    GameObject item = Instantiate(itemDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
                    item.GetComponent<ItemFloat>().dropped = true;
                }

                StartCoroutine("Death");
            }

            //   rb.AddForce(knockback * 2.85f, ForceMode2D.Impulse);
          //  return;
        }
        
    }

    void ResetTurret(Respawn resp)
    {
        base.RespawnEnemy(resp);
        base.ResetEnemy(resp);

        if (dead)
        {
            this.gameObject.transform.position = initialPos;
            isAggro = false;
            currentHealth = setHealth;

            attacking = false;
            dead = false;

            Collider2D[] colliders = this.GetComponents<Collider2D>();
            AudioSource[] sounds = this.GetComponents<AudioSource>();
            deathParticles.SetActive(false);

            foreach (Collider2D coll in colliders)
            {
                coll.enabled = false;
            }

            foreach (AudioSource sound in sounds)
            {
              //  if (sound != damageSound)
              //  {
              //      sound.enabled = true;
              //
              //  }
                sound.Stop();
            }

            if (secondaryParticles != null)
            {
                foreach (GameObject effect in secondaryParticles)
                {
                    effect.SetActive(true);
                }

            }

            rend.enabled = true;
            this.GetComponent<Collider2D>().enabled = true;
        }

        if (isAggro)
        {
            this.gameObject.transform.position = initialPos;
            isAggro = false;


            currentHealth = setHealth;

        }

        if (!isElite)
        {
            attacking = false;
        }
        anim.CrossFade("TurretIdle", 0, 0);
    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += ResetTurret;
    }
}
