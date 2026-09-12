using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shooting : MonoBehaviour
{
    //Refrences
    [SerializeField] RunAndJump rj;
    [SerializeField] Transform firePoint;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Ultimate ult;
    [SerializeField] Pause pause;

    //Attack

    [SerializeField] float attackRange;
    [field:SerializeField] public int attackDamage { get; private set; }
    [SerializeField] float attackTime;
    [SerializeField] LayerMask enemyLayers;
    [field: SerializeField] public bool isAttacking { get; private set; }

    [SerializeField] float maxOrder;
    [field: SerializeField] public int attackOrder { get; private set; }
    [SerializeField] float orderTime;
    private float trueOTime;
    private float setGrav;

    private float aerials = 1;

    [field: SerializeField] public float ultAttacking { get; private set; }
    public bool ultAttAnim { get; private set; }
    [SerializeField] GameObject ultParryProj;
    [SerializeField] GameObject ultAParticles;

    [SerializeField] GameObject ultRamIndicator;
    float attackPressDuration;
    public bool indicatorExists { get; private set; }
    private GameObject indic;

    public Collider2D[] hitEnemies;
    public int kills;


    private float strength = .5f;
    //  [SerializeField] float aerialCheck = 4;

    //Parrying
    [field:SerializeField] public bool isParrying { get; private set; }
    [SerializeField] float parryLength;
    [SerializeField] SpriteRenderer rend;
    [field:SerializeField] public bool parrySuccess { get; private set; }

    private FadeIn fx1;

    [SerializeField] AudioClip[] shoot;
    [SerializeField] AudioClip[] ultShoot;

    // Start is called before the first frame update
    void Start()
    {
        RunAndJump rj = GetComponent<RunAndJump>();
        trueOTime = 0;
  //      setGrav = rb.gravityScale;
        

    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetButton("Fire1") && !isParrying && !isAttacking && !rj.wallSliding && !rj.sitting && ult.ultimateState && ultAttacking == 0)
        {
            
            //start charging ultimate ram
            attackPressDuration += Time.deltaTime;

            if (attackPressDuration > 0.8f && indicatorExists == false)
            {
                RamIndicator();
                indicatorExists = true;
            }

        }

        if (Input.GetButtonUp("Fire1") && !isParrying && !isAttacking && !rj.wallSliding && !rj.sitting && ult.ultimateState && ultAttacking == 0 && pause.isPaused == false)
        {
            if (attackPressDuration > 0.8f)
            {
                StartCoroutine(UltRamInit());
            }
            Destroy(indic);
            indicatorExists = false;
            attackPressDuration = 0f;
        }
       

        if (Input.GetButtonDown("Fire1") && !isParrying && !isAttacking && !rj.wallSliding && !rj.sitting)
        {
            if (ult.ultimateState && ultAttacking == 0)
            {
                if (!rj.grounded && aerials > 0)
                {

                    StartCoroutine("UltAerialInit");
                    SoundFXManager.instance.PlayRandomSoundEffectClip(shoot, transform.position, 1f);

                } else
                {
                    StartCoroutine("AttackMelee");
                }
    
            }
            else
            {
                StartCoroutine("AttackMelee");
                SoundFXManager.instance.PlayRandomSoundEffectClip(shoot, transform.position, 1f);
            }
            
        }

        if (Input.GetButtonDown("Fire2") && rj.grounded == true && isParrying == false && !rj.sitting && isAttacking == false)
        {
            if (!ult.ultimateState)
            {
                StartCoroutine("Parry");
            } else
            {
                StartCoroutine("UltParry");
            }
            
            
        }

        if (trueOTime > 0)
        {
            trueOTime -= Time.deltaTime;

        } else if (trueOTime < 0)
        {
            attackOrder = 0;
            trueOTime = 0;

        }
        
        if (rj.wallSliding || rj.grounded && aerials <= 0)
        {
            aerials = 1;

            if (ultAttacking == 2 && !rj.wallSliding)
            {
                StartCoroutine(FlashLight(ult.ultFuelColor, transform.position, (8f), 1f, 6f, 0f));
                

                StartCoroutine("UltAerialFinish");
              
            }

        }

    }

    void FixedUpdate()
    {
        if (ultAttacking == 1)
        {
            if (Mathf.Abs(rb.linearVelocity.x) > 0.01f && ult.ultimateState == true && isAttacking == true && Mathf.Abs(rj.moving) != 1)
            {
                //ram while not moving
                UltRamCont();
                ultAttacking = 1;

            }
            else if ((Mathf.Abs(rb.linearVelocity.x) > (9.2f) && ult.ultimateState == true && isAttacking == true))
            {
                //ram while moving
                UltRamCont();
                ultAttacking = 1;
            }
            else if (ult.ultimateState == true && isAttacking == true)
            {
                //slowed down

                StartCoroutine(UltRamFinish());

            }
            else if (ultAttacking == 1)
            {
                //ran out of fuel mid attack
                StartCoroutine(UltRamFinish());
            }
        }
       
    }

    IEnumerator FlashLight(Color lColor, Vector3 position, float intensity, float duration, float radius, float delay)
    {
        //yield return new WaitForSeconds(delay);

        LightControl light = firePoint.GetComponentInChildren<LightControl>();

        //colors light
        light.SetColor(lColor);

        light.SetRange(radius);

        //position data of light
        Transform lightPos = light.gameObject.transform;
        lightPos.position = position;

        //light pulse eff
        StartCoroutine(light.Pulse(duration, intensity));
      

        yield return null;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, attackRange);
    }

    IEnumerator AttackMelee()
    {
       
        
    

        StartCoroutine(AttackDur());
        AnimationCheck();

        Collider2D[] hitEnemies = null;

        
        if (!rj.grounded && Input.GetAxisRaw("Vertical") != 0 && aerials > 0)
        {
            //aerial
            


            if (!ult.ultimateState)
            {
                StartCoroutine(FlashLight(Color.red, transform.position, (5.86f / 2.4f), attackTime, 4f, 0.4f));
                hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange * 3.64f, enemyLayers);
                aerials--;

                rb.AddForce(transform.up * attackDamage * Mathf.Clamp(Mathf.Abs(10 / (rb.linearVelocity.y)), 0, 5.8f), ForceMode2D.Impulse);
                rb.gravityScale = 0.6f;
                aerials--;


            } else
            {
                StartCoroutine(FlashLight(Color.red, transform.position + (Vector3.up * 3), (5.86f / 2.4f), attackTime, 4f, 0.4f));
                hitEnemies = Physics2D.OverlapCircleAll(transform.position + (Vector3.up * 3), attackRange * 2.32f, enemyLayers);

                if (rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 1f);
                }

                rb.AddForce((-transform.up * 8f), ForceMode2D.Impulse);
                //rb.gravityScale = 0.1f;
            }





        } else {

            if (!ult.ultimateState)
            {
                //normal
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(FlashLight(Color.red, firePoint.position, 5.86f, attackTime, 1.67f, 0.2f - (0.1f * (attackOrder - 1))));
                hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, attackRange, enemyLayers);
            } else
            {
                //normal
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(FlashLight(ult.ultFuelColor, firePoint.position, 5.86f, attackTime, 1.67f * 1.5f, 0.2f - (0.1f * (attackOrder - 1))));
                hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, attackRange * 1.5f, enemyLayers);
            }
            

        }
        
        if (hitEnemies != null)
        {
            //crappy implementation, fix later

            DamageCheck(hitEnemies, 0.15f);

           
        }

        yield return null;
        

        
    }
    public IEnumerator UltRamInit()
    {
        Destroy(indic);
        indicatorExists = false;
        attackPressDuration = 0f;

        ultAttAnim = true;
        

        Animator anim = gameObject.GetComponent<Animator>();


        anim.CrossFade("PlayerTAttackInit", 0f, 0);

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(rj.ForceEffect(transform.right * transform.localScale.x, 2f));

        rb.linearVelocity += (Vector2.right * transform.localScale.x);

        isAttacking = true;
        ultAttacking = 1;

        LightControl attLight = firePoint.GetComponentInChildren<LightControl>();
        attLight.SetColor(ult.ultFuelColor);
        attLight.SetRange(attackRange * 2.8f);

       
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.gameObject.tag == "Enemy" || coll.collider.gameObject.tag == "EnemyParriable")
        {
            

            if (ultAttacking == 1)
            {
                int prevDamage = attackDamage;
                int ramDamage = attackDamage * 3;

                attackDamage = ramDamage;

                //ram through OR end ram
                EnemyBase enemy = coll.collider.gameObject.GetComponent<EnemyBase>();
                bool isBoss = (coll.collider.gameObject.GetComponent<Boss>() != null);

                if (enemy.currentHealth <= ramDamage && isBoss == false)
                {
                    //ram through
                    //end ram, bounce back
                    Collider2D[] enemies = { coll.collider };
                    DamageCheck(enemies, 0.5f);

                    StartCoroutine(UltRamInit());
                    attackDamage = prevDamage;

                }
                else
                {
                    
                   
                    
                    //end ram, bounce back
                    Collider2D[] enemies = { coll.collider };

                    DamageCheck(enemies, 0.5f, 9f);

                    StartCoroutine(UltRamFinish());
                    attackDamage = prevDamage;
                }

            }



        }


    }

    void RamIndicator()
    {
        Vector3 spawnOffset = new Vector3(0f, 1f, 0f);
        GameObject indicator = Instantiate(ultRamIndicator, this.transform);
        indicator.transform.position += spawnOffset;
        indic = indicator;
    }


    void UltRamCont()
    {
        Animator anim = gameObject.GetComponent<Animator>();

        
        anim.CrossFade("PlayerTAttack", 0f, 0);

        LightControl attLight = firePoint.GetComponentInChildren<LightControl>();
        attLight.SetIntensity(5.86f);


        hitEnemies = Physics2D.OverlapCircleAll(transform.position, 2.56f, enemyLayers);


    }

    IEnumerator UltRamFinish()
    {
        Animator anim = gameObject.GetComponent<Animator>();

        LightControl attLight = firePoint.GetComponentInChildren<LightControl>();
        StartCoroutine(attLight.Pulse(0.8f, 7f));

        isAttacking = false;
        anim.CrossFade("PlayerTAttackOver", 0f, 0);
        // Wait for the animation to end
        yield return new WaitForSeconds(0.1f);
        ultAttacking = 0;
        ultAttAnim = false;

        
    }


    IEnumerator AttackDur()
    {
        trueOTime = orderTime;

        if (attackOrder < maxOrder)
        {
            attackOrder++;

        } else
        {
            attackOrder = 1;
        }


        isAttacking = true;

        if (!rj.grounded && Input.GetAxisRaw("Vertical") != 0)
        {
            //aerial
            yield return new WaitForSeconds(0.4f);

        }
        else
        {
            yield return new WaitForSeconds(attackTime + 0.2f);

        }
        
        isAttacking = false;
        rb.gravityScale = 1f;
    }

    void AnimationCheck()
    {
        Animator anim = gameObject.GetComponent<Animator>();

        
        if (!rj.grounded && Input.GetAxisRaw("Vertical") != 0 && !ult.ultimateState && aerials > 0)
        {
            anim.CrossFade("Aerial", 0f, 0);

        } else
        {   
            if (!ult.ultimateState)
            {
                switch (attackOrder)
                {

                    case 2:
                        anim.CrossFade("Attack2", 0f, 0);
                        break;
                    case 3:
                        anim.CrossFade("Attack3", 0f, 0);
                        break;
                    case 4:
                        anim.CrossFade("Attack4", 0f, 0);
                        break;
                    default:
                        //case 1
                        anim.CrossFade("Attack1", 0f, 0);
                        break;

                }

            } else
            {
                switch (attackOrder)
                {

                    default:
                        //case 1
                        anim.CrossFade("UltMelee1", 0f, 0);
                        break;

                }
            }
            
        }
    }

    void DamageCheck(Collider2D[] enemies, float fuelAdd, float knockBackFactor = 0.25f)
    {
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.gameObject.tag == "Mattress")
            {
                StartCoroutine(rj.KnockbackEffect(enemy.gameObject.transform.position, 1.5f));
                break;
            }

            if (enemy.GetComponent<EnemyBase>() != null)
            {
                enemy.GetComponent<EnemyBase>().TakeDamage(attackDamage, transform.right * transform.localScale.x);
                rj.AddFuel(fuelAdd);
                // enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * 1f, ForceMode2D.Impulse);

            }

            
          

            if (rj.grounded || Input.GetAxisRaw("Vertical") == 0)
            {
                if (enemy.GetComponent<Boss>() != null)
                {
                    if (enemy.GetComponent<Boss>().sentParrying == true)
                    {
                        knockBackFactor = 2f;
                        rb.AddForce(transform.up * 9f, ForceMode2D.Impulse);
                    }
                }

                if (ultAttacking == 1)
                {
                    knockBackFactor = 9f;
                }
                StartCoroutine(rj.KnockbackEffect(enemy.gameObject.transform.position, knockBackFactor));


            }



        }
    }

    IEnumerator Parry()
    {
        isParrying = true;
        //rend.color = Color.blue;
        yield return new WaitForSeconds(parryLength);
        isParrying = false;
        //rend.color = Color.white;
    }

    IEnumerator UltParry()
    {
        
        isParrying = true;
        //rend.color = Color.blue;
        yield return new WaitForSeconds(.3f);

        GameObject proj = Instantiate(ultParryProj, firePoint.position, firePoint.rotation);

        proj.GetComponent<Rigidbody2D>().AddForce(transform.right * 21f * transform.localScale.x, ForceMode2D.Impulse);
        proj.GetComponent<Collider2D>().enabled = false;
        // proj.transform.localScale = Vector2.right * transform.localScale.x;
        Vector3 Scaler = transform.localScale;
        proj.transform.localScale = Scaler;

        StartCoroutine(FlashLight(ult.ultFuelColor, firePoint.position, 5.86f, attackTime, 1.67f / 1.3f, 0.1f));

        Destroy(proj, 1.1f);
        


        yield return new WaitForSeconds(.3f);
        proj.GetComponent<Collider2D>().enabled = true;
        isParrying = false;
        //rend.color = Color.white;

        
    }

    IEnumerator UltAerialInit()
    {
        ultAttAnim = true;
        

        Animator anim = gameObject.GetComponent<Animator>();
        anim.CrossFade("UltAerialBoostDown", 0f, 0);
        rb.gravityScale = 0.3f;
        yield return new WaitForSeconds(.18f);
        StartCoroutine(FlashLight(ult.ultFuelColor, transform.position + (Vector3.up * 1.5f), (4f), 0.5f, 4f, 0.2f));

        GameObject part = Instantiate(ultAParticles, transform.position + (Vector3.up * 1.5f), firePoint.rotation);
        part.GetComponent<ParticleSystem>().Emit(8);
        Destroy(part, .7f);

        yield return new WaitForSeconds(.17f);
        rb.gravityScale = 1f;
        ultAttacking = 2;
        isAttacking = true;
        aerials = 0f;
        // damages above player

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + (Vector3.up * 3), attackRange / 1.3f, enemyLayers);

        if (hitEnemies != null)
        {
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.GetComponent<EnemyBase>() != null)
                {
                    enemy.GetComponent<EnemyBase>().TakeDamage(attackDamage, transform.right * transform.localScale.x);
                }
                rj.AddFuel(0.15f);

                if (rj.grounded || Input.GetAxisRaw("Vertical") == 0)
                {

                    StartCoroutine(rj.KnockbackEffect(enemy.gameObject.transform.position, 0.25f));


                }


            }


        }

       
        //boost down
        rb.AddForce((-transform.up * 17f), ForceMode2D.Impulse);

        anim.CrossFade("UltAerialFalling", 0f, 0);

        yield return new WaitForSeconds(4f);
        StartCoroutine(UltAerialFinish());

    }

    IEnumerator UltAerialFinish()
    {

       

        rb.linearVelocity = Vector2.zero;
        Animator anim = gameObject.GetComponent<Animator>();
        //upper right corner
        Vector3 rightup = (transform.position + (Vector3.right * 3) + (Vector3.up * 3));

        // lower left corner
        Vector3 leftdown = (transform.position - (Vector3.right * 3) - (Vector3.up * 3));

        Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(rightup, leftdown, enemyLayers);

        if (hitEnemies != null)
        {
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.GetComponent<EnemyBase>() != null)
                {
                    enemy.GetComponent<EnemyBase>().TakeDamage(attackDamage, transform.right * transform.localScale.x);
                    rj.AddFuel(0.5f);
                  
                }
              

                if (rj.grounded || Input.GetAxisRaw("Vertical") == 0)
                {

                    StartCoroutine(rj.KnockbackEffect(enemy.gameObject.transform.position, 0.25f));


                }


            }


        }

        anim.CrossFade("UltAerialSlamFloor", 0f, 0);


        yield return new WaitForSeconds(.5f);

        ultAttacking = 0f;
        isAttacking = false;
        ultAttAnim = false;

    }

    public IEnumerator ParrySuccess()
    {
        StartCoroutine(FlashLight(Color.cyan, firePoint.position, 8f, attackTime * 1.5f, 2f, 0.1f));

        rj.AddFuel(0.35f);
        
       
        parrySuccess = true;
        yield return new WaitForSeconds(0.9f);
        parrySuccess = false;
    }

}

