using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class RunAndJump : MonoBehaviour
{// Movement Variables

    //Other  
    [Header("References / Other")]
    public bool facingRight = true;
    [field:SerializeField] public GameObject zipline { get; private set; }
    [SerializeField] Animator anim;
    [SerializeField] Shooting shoot;
    [SerializeField] Health heal;
    [SerializeField] Respawn respawn;
    //[SerializeField] PlayerAudio pAudio;
    [SerializeField] Ultimate ult;
    private Coroutine hazardCoroutine;
    [SerializeField] GameObject medallionItem;
    

    [field:SerializeField] public bool sitting { get; private set; }
    [field:SerializeField] public bool stunned { get; private set; }
    int animationInitialized = 0;

    //Jumping Variables
    [Header("Jump Settings")]
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowjumpMultiplier = 0.5f;
    public bool grounded { get; private set; }
    [SerializeField] Transform groundCheck;

    [SerializeField] float jumpPower = 190;
    [field:SerializeField] public bool hasJumped { get; private set; } = false;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] int jumps;
    [field:SerializeField] public int currentjumps { get; private set; }

    [SerializeField] Vector3 lastGround;
    [SerializeField] bool hazardDamage;

    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystem fallParticles;
    private bool enableParticles;

    //Medallion Variables
    [Header("Rustburn & UI")]
    [field:SerializeField] public bool hasMedallion;

    [SerializeField] MainMedallion mainMedal;

    [SerializeField] LightControl medalLight;
    [SerializeField] ParticleSystem medalParticles;

    [SerializeField] Animator medalAnim;
    [field:SerializeField] public bool fireBoost { get; private set; }
    [SerializeField] TrailRenderer fireTrail;
    [field:SerializeField] public float fuel;
    [field: SerializeField] public float maxFuel = 1f;
    [SerializeField] float fuelUseRate;
    [SerializeField] float barScale;
    [SerializeField] float boostPower;

    [SerializeField] Image rend2;
    [SerializeField] GameObject fuelBarFX;
    private FadeIn fx1;

    //Running Variables
    [Header("Running")]

    [SerializeField] float a = 0; 
    [SerializeField] float Power = 300;
    [SerializeField] float lerpSpeed;

    public float currentPower = 0;

    private float setPower;
    private float pressDuration;

    //Force Variables
    private Coroutine lerpId = null;
    private int stackedForces = 0;

    //debug variable, discard later
    bool knockedBack;

    [field:SerializeField] public float moving { get; private set; }
    [SerializeField] float maxSpeed;
    Coroutine forceCorot;



    //Wall Sliding
    [Header("Wall Slide")]
    bool isTouchingFront;
    [SerializeField] Transform frontCheck;
    [SerializeField] int maxWallJumps;
    int currWallJumps;
    public bool grabbed { get; private set; }

    [SerializeField] LayerMask whatIsGround;

    [field:SerializeField] public LayerMask whatisAnything { get; private set; }

    [SerializeField] float checkRadius;
    [field:SerializeField] public bool wallSliding {get; private set;}
    [SerializeField] float wallSlideSpeed;
    [SerializeField] ParticleSystem wallPS;
    [SerializeField] TrailRenderer wTrail;

    [Header("Sound Effects")]
    [SerializeField] AudioClip[] jump;
    [SerializeField] AudioClip[] step;
    [SerializeField] AudioClip slideStart;
    [SerializeField] AudioClip[] slide;
    [SerializeField] AudioClip[] rustburn;
    [SerializeField] AudioClip[] refuel;
    

    private bool lpIsPlaying = false;
    public bool riding = false;


    void Start()
    {
        fx1 = this.gameObject.GetComponent<FadeIn>();
        setPower = Power;

        //Get movement component
        currentjumps = jumps;
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();


    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void Update()
    {

        if (stunned && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetKeyDown(KeyCode.E)) && animationInitialized == 0)
        {
            StartCoroutine(StunRecover());
        }


        if (!riding)
        {

            if (shoot.ultAttacking == 2)
            {
                if (!grounded)
                {
                    moving = Input.GetAxisRaw("Horizontal");

                }
                else
                {
                    moving = 0;
                }

            }
            else
            {
                if (shoot.ultAttacking != 1 && !sitting && !riding && !stunned)
                {
                    moving = Input.GetAxisRaw("Horizontal");
                }
                else
                {
                    moving = 0;
                }

            }

            //Flip direction appropriate to direction we are moving in

            if (shoot.isAttacking == false)
            {
                if (facingRight == false && moving > 0)
                {
                    Flip();
                }
                else if (facingRight == true && moving < 0)
                {
                    Flip();

                }
            }

            //this sucks
            if (Input.GetButtonDown("Jump") && currentjumps > 0 && shoot.isAttacking == false && !grabbed && !sitting && !stunned)
            {
                hasJumped = true;

            }

            if (hasMedallion == true && fuel > 0.15f && Input.GetButtonDown("Fire2") && wallSliding == false && !grounded)
            {
                StartCoroutine("RustburnBoost");
            }



            if (Input.GetKeyDown(KeyCode.J) && grounded == true)
            {
                Sit();

            }

            //Wall Running control
            Collider2D isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);

            if (isTouchingFront != null && rb.linearVelocity.y != 0 && !grabbed && currWallJumps > 0)
            {
                //    SoundFXManager.instance.PlaySoundEffectClip(slideStart, transform.position, 1f);
                wallSliding = true;
            }
            else
            {
                wallSliding = false;
                var em = wallPS.emission;
                em.enabled = false;
                wTrail.emitting = false;
            }

            if (wallSliding == true && (currWallJumps > 0))
            {
                if (!grabbed)
                {
                    WallSlide();
                }
            }


            // LedgeCheck();
            fireTrail.emitting = fireBoost;

            // rend2.material.SetFloat("_Value", (fuel / barScale));


            if (Input.GetKeyDown(KeyCode.U))
            {
                AddFuel(maxFuel);
            }

            if (hazardDamage)
            {
                rb.linearDamping = 20;

                if (rb.linearVelocity.y < 5)
                {
                    rb.linearDamping = 450;
                }

            }

            if (rb.linearVelocity.y < -15 && !fireBoost)
            {
                enableParticles = true;
            }
        }
        

        //Get moving


    }
    

    void FixedUpdate()
    {

        if (!riding)
        {
            if (knockedBack)
            {
                StartCoroutine(KnockbackEffect(transform.right, 1f));
                knockedBack = false;
            }


            if (heal.currentHealth <= 0)
            {
                rb.isKinematic = true;

            }

            Collider2D isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);



            //Running (or walking, whatever)
            if (moving != 0 && !grabbed && !sitting && !riding & !stunned)
            {

                if (shoot.isParrying == false && shoot.parrySuccess == false)
                {

                    float dir = Mathf.Sign(this.transform.localScale.x);
                    if (pressDuration < 0)
                    {
                        pressDuration = 0;
                        if (Mathf.Sign(currentPower) != moving)
                        {
                            setPower = currentPower;
                        }

                    }

                    pressDuration += Time.deltaTime;

                    //NEGATIVE MOVE
                    if (setPower <= 0 && Mathf.Sign(setPower) != dir)
                    {
                        currentPower = Mathf.Lerp(setPower, Power + ult.speedBoost, pressDuration / lerpSpeed);
                        //NEGATIVE MOVE, reversing direction
                    }
                    else if (setPower > 0 && Mathf.Sign(setPower) != dir)
                    {
                        currentPower = Mathf.Lerp(-setPower, Power + ult.speedBoost, pressDuration / lerpSpeed);
                        //Positive move, reversing direction
                    }
                    else
                    {
                        currentPower = Mathf.Lerp(currentPower, (Power + ult.speedBoost), pressDuration / lerpSpeed);
                        Debug.Log("Same Direction Same Move- Current Power: " +currentPower * Time.deltaTime * moving);
                        //same direction same move
                    }


                    rb.linearVelocity = new Vector2(currentPower * Time.deltaTime * moving, rb.linearVelocity.y);
                   
                }


            }
            else if (isTouchingFront == null && !grabbed)
            {


                float dir = Mathf.Sign(this.transform.localScale.x);
                if (pressDuration > 0)
                {
                    pressDuration = 0;
                    setPower = currentPower * Mathf.Sign(this.transform.localScale.x);
                }

                pressDuration -= Time.deltaTime;

                // float currentLerp = Mathf.Lerp(setPower, 0, Mathf.Abs(pressDuration) * 1 / lerpSpeed) * Mathf.Sign(transform.localScale.x);
                currentPower = Mathf.Lerp(setPower, 0, Mathf.Abs(pressDuration) * 1 / lerpSpeed);
                // Debug.Log(rb.velocity.x);
                rb.linearVelocity = new Vector2(currentPower * Time.deltaTime, rb.linearVelocity.y);
            }


            //Jumping
            //Replenish Jumps

            if (!grounded)
            {

                Collider2D[] isThereGround = Physics2D.OverlapCircleAll(groundCheck.position, 0.08f, whatisAnything);

                if (rb.linearVelocity.y == 0 && isThereGround.Length > 0 && wallSliding == false)
                {
                    grounded = true;
                }

                fallParticles.gameObject.SetActive(false);

            }


            if (grounded == true)
            {

                currentjumps = jumps;
                currWallJumps = maxWallJumps;

                trail.emitting = false;

                if (enableParticles == true)
                {
                    fallParticles.gameObject.SetActive(true);
                    fallParticles.Emit(1);
                    enableParticles = false;
                }

                if (moving != 0)
                {
                    StartCoroutine(LoopSFX(step));
                }
            } else if (wallSliding)
            {
                currentjumps = jumps;
            }




            if (Mathf.Abs(rb.linearVelocity.y) > 11)
            {
                trail.emitting = true;
            }






            //Puts you in the Air When You Jump (applies force? idk how to word it)
            if (hasJumped)
            {

                SoundFXManager.instance.PlayRandomSoundEffectClip(jump, transform.position, 1f);
                currentjumps -= 1;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * Time.deltaTime);
                grounded = false;
                hasJumped = false;
                setPower = currentPower;
            }



            //Jump Feel
            if (rb.linearVelocity.y < 0 && !wallSliding)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowjumpMultiplier - 1) * Time.deltaTime;

            }


        }


    }

    IEnumerator RustburnBoost()
    {
        fireBoost = true;
        yield return new WaitForSeconds(0.05f);

        if (!ult.ultimateState)
        {
            AddFuel(-0.15f);
        }
        SoundFXManager.instance.PlayRandomSoundEffectClip(rustburn, transform.position, 1f);

        if (moving != 0 && Input.GetAxisRaw("Vertical") == 0)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.gravityScale = 0f;
            StartCoroutine(ForceEffect(transform.right * transform.localScale.x, 1f));
            
            anim.CrossFade("RustburnSide", 0, 0);

            yield return new WaitForSeconds(0.2f);
            rb.gravityScale = 1f;

        }
        else if (Input.GetAxisRaw("Vertical") == 1)
        {
            rb.linearVelocity = Vector2.zero;
            anim.CrossFade("RustburnUp", 0, 0);

            rb.AddForce(transform.up * 16f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
        } else
        {
            rb.linearVelocity = Vector2.zero;
            anim.CrossFade("RustburnDown", 0, 0);

            rb.AddForce(-transform.up * 16f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
        }
        fireBoost = false;

        rb.linearVelocity = new Vector2(0f, (rb.linearVelocity.y)/2);
    }


    void WallSlide()
    {
        rb.gravityScale = 1;
        if (wallSliding == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, wallSlideSpeed, float.MaxValue));
            var em = wallPS.emission;
            em.enabled = true;

            wTrail.emitting = true;
            trail.emitting = false;

            StartCoroutine(LoopSFX(slide));
            
        }
    }

    void HoldWall()
    {
        grabbed = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
    }

    public void Replenish()
    {
        currentjumps = jumps;
      
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(frontCheck.position, checkRadius);
        Gizmos.DrawWireSphere(groundCheck.position, 0.08f);
    }

    public IEnumerator KnockbackEffect(Vector3 otherVector, float factor)
    {

        Vector2 dist = this.transform.position - otherVector;
        float angle = Mathf.Atan2(dist.y, dist.x);

        //Debug.Log(Mathf.Rad2Deg * angle + " degrees.  x: " + Mathf.Abs(Mathf.Cos(angle)) + " y: " + Mathf.Abs(Mathf.Sin(angle)));
      

        
        if (Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(this.transform.localScale.x))
        {
            //same dir same move
            currentPower += a * Mathf.Cos(angle) * factor;

        } else
        {
            setPower += a * Mathf.Cos(angle) * factor;
        }
        

      
        

        pressDuration = 0;
        
        if (forceCorot == null)
        {
            forceCorot = StartCoroutine(LerpLerp(0.1f, setPower));
        } else
        {
            StopCoroutine(forceCorot);
            forceCorot = StartCoroutine(LerpLerp(0.1f, setPower));
        }
        


        rb.linearVelocity = new Vector2(rb.linearVelocity.x, a * Time.deltaTime * Mathf.Sin(angle) * factor);


        /*(currentjumps = 0;
        grounded = false;
        hasJumped = false;
        */
        yield return null;
    }

    public IEnumerator ForceEffect(Vector3 dir, float factor, float power = 300)
    {

        setPower = power;
        float angle = Mathf.Atan2(dir.y, dir.x);
        
        if (Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(this.transform.localScale.x))
        {
            //same dir same move, make sure setPower's sign matches player direction
            //otherwise weird things happen in the Running Clause (lines 305 - 345)
            setPower *= Mathf.Sign(dir.x);
            currentPower += Mathf.Abs(a * Mathf.Cos(angle) * factor);

        } else
        {
            setPower += (a * Mathf.Cos(angle) * factor);
        }
        
        pressDuration = 0;
        
        if (lerpId == null)
        {
            lerpId = StartCoroutine(LerpLerp(0.8f, setPower));
        } else
        {
            stackedForces++;
            StopCoroutine(lerpId);
            lerpId = StartCoroutine(LerpLerp(0.8f, setPower * stackedForces));
            Debug.Log("lerp running, cannot execute, Forces to process: " + stackedForces);
            
        }


        rb.linearVelocity = new Vector2(rb.linearVelocity.x, a * Time.deltaTime * Mathf.Sin(angle) * factor);

        yield return null;
    }


    IEnumerator HazardDamage(Vector2 location, bool safe = false)
    {

        hazardDamage = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * Time.deltaTime);
        


        grounded = false;

        if (safe == false)
        {
            StartCoroutine(heal.TakePDamage(1, Vector2.up));
        }

        yield return new WaitForSeconds(1f);

        if (heal.currentHealth > 0)
        {
            this.transform.position = location;
        }

        rb.linearDamping = 0.5f;
        hazardDamage = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        


        if (coll.gameObject.tag == "Hazard")
        {
            Color blackTrans = new Color(0f, 0f, 0f, 0f);
            StartCoroutine(heal.FadeInOut(blackTrans, Color.black, 0.5f));
            if (heal.currentHealth > 0)
            {
                StartCoroutine(HazardDamage(lastGround));
            }

        }

        if (coll.gameObject.tag == "HazardFade")
        {
            Color blackTrans = new Color(0f, 0f, 0f, 0f);
            StartCoroutine(heal.FadeInOut(blackTrans, Color.black, 0.5f));

            StartCoroutine(HazardDamage(lastGround, true));
        }

        if (coll.gameObject.tag == "FuelDepletion")
        {
            fuel = 0f;
        }

        if (coll.gameObject.tag == "Checkpoint")
        {
            SaveValues();
        }

        
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.layer == 8)
        {
            lastGround = this.transform.position;
        }
    }

    
    public void MedallionUpgrade()
    {
        hasMedallion = true;
       // medal.SetActive(true);
       // fuelBar.SetActive(true);
    }

    void SaveValues()
    {
        float[] savedVal = respawn.savedValues;

        savedVal[1] = hasMedallion ? 1 : 0;
        savedVal[2] = fuel;
    }

    void LoadValues(Respawn resp)
    {

        rb.isKinematic = false;

        float[] savedVal = respawn.savedValues;

        if (savedVal[1] <= 0)
        {
            hasMedallion = false;
           
            medallionItem.SetActive(true);
        }

        fuel = savedVal[2];
    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += LoadValues;
    }

    IEnumerator LerpLerp(float duration, float knockBackFactor)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            
            lerpSpeed = Mathf.Lerp(2, 0.2f, normalizedTime);
            //earlier i had the t value as "normalized time * 2". why???
            setPower = Mathf.Lerp(knockBackFactor, 0, normalizedTime);

            yield return null;
        }


        lerpId = null;
        stackedForces = 0;
    }

    public void AddFuel(float amt)
    {
        SoundFXManager.instance.PlayRandomSoundEffectClip(refuel, transform.position, 1f);

        if ((fuel + amt) >= maxFuel)
        {
            fuel = maxFuel;

        } else
        {
            fuel += amt;
        }

        StartCoroutine(mainMedal.Shake(0.25f, 4f));

        StartCoroutine(fx1.PulseFadeSpr(0.2f, fuelBarFX));
    }

    IEnumerator LoopSFX(AudioClip[] clips)
    {
        if (!lpIsPlaying)
        {
            lpIsPlaying = true;
            int soundID = Random.Range(0, clips.Length);

            SoundFXManager.instance.PlaySoundEffectClip(clips[soundID], transform.position, 1f);
            yield return new WaitForSeconds(clips[soundID].length);
            lpIsPlaying = false;
        }

    }


    public void Sit()
    {
        StartCoroutine("ToggleSit");
    }

    public void Stun()
    {
        stunned = true;
        anim.CrossFade("Stunned", 0, 0);
    }

    IEnumerator StunRecover()
    {
        animationInitialized = 1;
        //play recovery animation
        anim.CrossFade("StunnedRecovery", 0, 0);
        yield return new WaitForSeconds(2f);
        stunned = false;
        animationInitialized = 0;
    }

    IEnumerator ToggleSit()
    {
        if (!sitting)
        {
            sitting = true;
            anim.CrossFade("Sit", 0, 0);
            yield return new WaitForSeconds(0.3f);
            anim.CrossFade("SitLoop", 0, 0);

        } else
        {
            anim.CrossFade("Unsit", 0, 0);
            yield return new WaitForSeconds(0.24f);
            sitting = false;
        }

    }

}