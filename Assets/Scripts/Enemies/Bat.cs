using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : EnemyBase
{

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float aggroRange;
    [SerializeField] private float sprayRange;
    

    private bool fullAttack;

   // [SerializeField] GameObject player;
    [SerializeField] ParticleSystem particleSys;

    //Animation Control
    [SerializeField] Animator anim;
    [SerializeField] bool telegraphed;

    [SerializeField] int playingSound;

    [SerializeField] Transform playerT;

    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerT = player.GetComponent<Transform>();

    
        initialPos = this.gameObject.transform.position;

        currentHealth = setHealth;
        contactDamage = 0;

        var em = particleSys.emission;
    
        em.enabled = false;

        anim.CrossFade("Roosting", 0, 0);

        lpIsPlaying = false;
        base.Start();
    }


    // Update is called once per frame
    void Update()
    {

        Vector2 dist = this.transform.position - playerT.position;

        if (isAggro && !dead)
        {

            if (attacking == false && fullAttack == false)
            {
                anim.CrossFade("Flying", 0, 0);
                AttackB();
            } 
        }
        else
        {
            anim.CrossFade("Roosting", 0, 0);
            AggroCheck();
            fullAttack = false;
            telegraphed = false;
        }
    }

    void AttackB()
    {
        StartCoroutine("Attack");
    }

    IEnumerator Attack()
    {
        fullAttack = true;

        //Chases player 

        //Interpolates position between player plus some additional height


        if (attacking == false)
        {
            for (float t = 0f; t < 3; t += Time.deltaTime)
            {
                float normalizedTime = t / 3;

                Transform playerT = player.GetComponent<Transform>();
                Vector2 newPos = new Vector2(playerT.position.x, playerT.position.y + Random.Range(3.5f, 4.5f));
                transform.position = Vector2.Lerp(transform.position, newPos, Time.deltaTime);

                yield return null;
            }
        }

        if (telegraphed == false)
        {
            anim.CrossFade("Telegraph", 0, 0);
            telegraphed = true;
            attacking = true;
        }
  
        yield return new WaitForSeconds(1f);

        StartCoroutine(LoopSFX(attackSounds));

        //I dont know why i have to "Play" the PS. Should be playing by default? Doesnt work if not here though.
        particleSys.Play();
        var em = particleSys.emission;
        em.enabled = true;

        anim.CrossFade("Attack", 0, 0);
        yield return new WaitForSeconds(0.2f);

        //Maximize attack range
        Vector2 distance = player.GetComponent<Transform>().position - transform.position;

        //Raycast for 3 seconds hitting player if in initial range
        for (float t = 0f; t < 3; t += Time.deltaTime)
        {
            float normalizedTime = t / 3;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, sprayRange, playerLayer);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<Health>() != null && hit.collider.gameObject.GetComponent<Health>().invincible == false)
                {
                    if (!dead)
                    {
                        StartCoroutine(hit.collider.gameObject.GetComponent<Health>().TakePDamage(1, hit.transform.right));
                    }
                   
                }
            }
            yield return null;
        }

        em.enabled = false;
        attacking = false;
        telegraphed = false;
        fullAttack = false;
    }

    IEnumerator BatSFX(float delay, int soundID, bool overwrite)
    {
        playingSound = soundID;

        yield return new WaitForSeconds(delay);
        playingSound = -1;

    }



    void AggroCheck()
    {
        Collider2D aggro = Physics2D.OverlapCircle(transform.position, aggroRange, playerLayer);

        if (aggro != null && aggro.gameObject.tag == "Player")
        {
            isAggro = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

    }

    public override void TakeDamage(float damage, Vector2 knockback)
    {
        if (invincible != true)
        {
            int eff = Random.Range(2, 4);
            SoundFXManager.instance.PlayRandomProximitySoundEffectClip(damageSounds, gameObject, enemyVol);
            currentHealth -= damage;

            StartCoroutine("Invincibility");
        }


        if (currentHealth < 0 || currentHealth == 0)
        {
            int eff = Random.Range(4, 6);
            SoundFXManager.instance.PlayRandomProximitySoundEffectClip(deathSounds, gameObject, enemyVol);
            if (itemDrop != null && Random.Range(1, 3) == 1)
            {
                Instantiate(itemDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
            }

            StartCoroutine("Death");
            
        }

        rb.AddForce(knockback * 2.85f, ForceMode2D.Impulse);
        return;
    }

    void CancelAttack(Respawn resp)
    {
        base.RespawnEnemy(resp);
        base.ResetEnemy(resp);

        dead = false;
        isAggro = false;
    
        this.gameObject.transform.position = initialPos;
        StopCoroutine("Attack");

        var em = particleSys.emission;
        em.enabled = false;

        attacking = false;
        telegraphed = false;
      
        fullAttack = false;

        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<Collider2D>().enabled = true;

        currentHealth = setHealth;
        


    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += CancelAttack;


    }
}