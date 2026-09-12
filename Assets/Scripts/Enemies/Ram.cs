using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ram : EnemyBase
{
    // Start is called before the first frame update

    [SerializeField] private float aggroRange;
    [SerializeField] private int ramDamage;
    [SerializeField] private float ramSpeed;
    [SerializeField] private Rigidbody2D rb_;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask levelLayer;
    [SerializeField] Collider2D wanderRegion;
  
    //if no wander region is assigned then use wander radius
    public float wanderRadius = 5f; // Radius of the wander area
    public float wanderTimer = 6f; // Time interval to change wander direction
    public float movementSpeed = 2f; // Movement speed of the AI

    private Vector3 targetPosition;
    private float timer;
    public bool facingRight = true;

    [SerializeField] Animator anim;

    [SerializeField] Transform playerT;

    [SerializeField] protected AudioClip[] parriedSounds;
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerT = player.GetComponent<Transform>();

        timer = wanderTimer;
        GetNewRandomPosition();


        initialPos = this.transform.position;

        if (wanderRegion != null)
        {
            wanderRegion.gameObject.transform.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
       // enemyVol = Mathf.InverseLerp(20, 10, (player.transform.position - transform.position).magnitude);
        if (dead)
        {
            gameObject.tag = "Untagged";

        } else
        {
            gameObject.tag = "EnemyParriable";
        }
        
            Vector2 dist = this.transform.position - playerT.position;

            See();
            //Aggro Detection


            //Wandering
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
               // PlaySoundEffect(enemySFX[0], true, false, false);
                GetNewRandomPosition();
                timer = 0;
            }
            else if (!isAggro)
            {


                MoveTowardsPosition();
            }

        StartCoroutine(LoopSFX(idleSounds));

    }

    void See()
    {
        
        RaycastHit2D see = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, aggroRange, playerLayer);
        Debug.DrawRay(transform.position, transform.right * transform.localScale.x);
      
        if (see.collider != null && isAggro == false)
        {
            isAggro = true;
            StartCoroutine("RamAttack");
        }
 
    }

    IEnumerator RamAttack()
    {
        int eff = Random.Range(3, 4);
        //PlaySoundEffect(enemySFX[eff], false, true, false);
        SoundFXManager.instance.PlayRandomProximitySoundEffectClip(telegraphSounds, gameObject, enemyVol);
        attacking = true;
        anim.CrossFade("Telegraph", 0, 0);
        yield return new WaitForSeconds(1);

        eff = Random.Range(1, 2);
        // PlaySoundEffect(enemySFX[eff], false, true, true);
        SoundFXManager.instance.PlayRandomProximitySoundEffectClip(attackSounds, gameObject, enemyVol);
        anim.CrossFade("Ramming", 0, 0);
     
        rb_.AddForce((transform.right * transform.localScale.x) * ramSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(2);
        


        attacking = false;
        isAggro = false;

        yield return new WaitForSeconds(1);
        See();

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 endpoint = new Vector2(transform.position.x + aggroRange * transform.localScale.x, transform.position.y);
        Gizmos.DrawLine(transform.position, endpoint);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void GetNewRandomPosition()
    {
        RaycastHit2D lookforWalls = Physics2D.Raycast(transform.position, transform.right * wanderRadius, aggroRange, levelLayer);

        if (lookforWalls.collider != null)
        {

            

            Vector2 offset = new Vector2(lookforWalls.distance, 0);
           //Debug.Log(offset);
            if (lookforWalls.distance < 1)
            {

                //Debug.Log(lookforWalls.distance + gameObject.name);

                Flip();

                lookforWalls = Physics2D.Raycast(transform.position, transform.right * wanderRadius, aggroRange, levelLayer);
            }
            
            targetPosition = (Vector2)transform.position + offset + Random.insideUnitCircle.normalized * lookforWalls.distance;
            

        } else
        {
            targetPosition = (Vector2)transform.position + Random.insideUnitCircle.normalized * wanderRadius;
        }

        Vector3 distance = new Vector3(targetPosition.x - transform.position.x, 0, 0);
        if (distance.x < 0 && facingRight == true)
        {

            Flip();

        }
        else if (distance.x > 0 && facingRight == false)
        {

            Flip();

        }

    }

    void MoveTowardsPosition()
    {
        
        Vector3 distance = new Vector3(targetPosition.x - transform.position.x, 0, 0);
        
        anim.CrossFade("Walking", 0, 0);

        
        transform.Translate(distance.normalized * movementSpeed * Time.deltaTime, Space.World);
        

       
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Shooting>() != null) 
            {
                Shooting shoot = other.gameObject.GetComponent<Shooting>();

                if (shoot.isParrying == true && attacking == true && Mathf.Sign(other.gameObject.transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x))
                {
                    int eff = Random.Range(7, 8);
                    // PlaySoundEffect(enemySFX[eff], false, true, true);
                    SoundFXManager.instance.PlayRandomProximitySoundEffectClip(parriedSounds, gameObject, enemyVol);
                    Flip();
                    rb_.AddForce((transform.right * transform.localScale.x) * ramSpeed, ForceMode2D.Impulse);

                    anim.CrossFade("Telegraph", 0, 0);
                    

                    if ((currentHealth - shoot.attackDamage) <= 0)
                    {
                        dead = true;

                        eff = Random.Range(9, 10);
                      //  PlaySoundEffect(enemySFX[eff], false, true, true);

                       // Instantiate(itemDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
                        StartCoroutine("Death");

                    } else
                    {
                        if (invincible != true)
                        {
                            currentHealth -= (shoot.attackDamage * 2);
                            StartCoroutine("Invincibility");
                            
                        }


                    }

                    shoot.StartCoroutine(shoot.ParrySuccess());
                    
                }
            }

        }

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hazard")
        {

            TakeDamage(999, Vector2.up);
        }

    
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Parrier" && attacking == true && Mathf.Sign(transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x))
        {

            //parry control
            bool parry = false;

            if (other.gameObject.tag == "Player" && player.GetComponent<Shooting>().isParrying)
            {
                parry = true;
            }

            if (other.gameObject.tag == "Parrier")
            {
                parry = true;
            }

            if (parry)
            {

                Shooting shoot = playerT.gameObject.GetComponent<Shooting>();

                int eff = Random.Range(7, 8);
                // PlaySoundEffect(enemySFX[eff], false, true, true);
                SoundFXManager.instance.PlayRandomProximitySoundEffectClip(parriedSounds, gameObject, enemyVol);
                Flip();
                rb_.AddForce((transform.right * playerT.localScale.x) * (ramSpeed / 1.5f), ForceMode2D.Impulse);

                anim.CrossFade("Telegraph", 0, 0);


                if ((currentHealth - shoot.attackDamage) <= 0)
                {
                    dead = true;

                    eff = Random.Range(9, 10);
                    //PlaySoundEffect(enemySFX[eff], false, true, true);

                    //Instantiate(itemDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
                    StartCoroutine("Death");

                }
                else
                {
                    if (invincible != true)
                    {
                        currentHealth -= (shoot.attackDamage * 2);
                        StartCoroutine("Invincibility");

                    }


                }

                shoot.StartCoroutine(shoot.ParrySuccess());


            }

        }
      

        /*
        if (other.gameObject.tag == "Parrier")
        {
            Vector2 parrierPos = other.gameObject.transform.position;
            Vector2 thisPos = transform.position;
            Vector2 dist = thisPos - parrierPos;

            TakeDamage(1, dist);
            playerT.gameObject.GetComponent<RunAndJump>().AddFuel(.15f);
        }
        */
    }

    void RamDebug(Color color, string message)
    {
        this.rend.color = color;
        Debug.Log("Changed Color because of: " + message);
    }

    public override void TakeDamage(float damage, Vector2 knockback)
    {
        if (invincible != true)
        {
            int eff = Random.Range(5, 6);
            // PlaySoundEffect(enemySFX[eff], false, true, true);
            SoundFXManager.instance.PlayRandomProximitySoundEffectClip(damageSounds, gameObject, enemyVol);
            currentHealth -= damage;

            StartCoroutine("Invincibility");

            rb.AddForce(knockback * 2.85f, ForceMode2D.Impulse);
            Flip();
        }


        if (currentHealth < 0 || currentHealth == 0)
        {
            dead = true;

            int eff = Random.Range(9, 10);
           // PlaySoundEffect(enemySFX[eff], false, true, true);
           // Debug.Log("DEAD!");
            if (itemDrop != null && Random.Range(1, 3) == 1)
            {
                //GameObject item = Instantiate(itemDrop, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
                //item.GetComponent<ItemFloat>().dropped = true;
            }

            StartCoroutine("Death");
        }

      //  Debug.Log("took damage");
        return;
    }

    
}
