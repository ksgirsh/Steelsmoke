using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Boss : EnemyBase
{
    [Header("General Management")]
    [SerializeField] GameObject corpse;
    [SerializeField] float duration = 3f;
    [SerializeField] Image healthBar;

    bool draining = false;
    bool fading = false;

    float nHealth;

    [SerializeField] int lives = 3;
    //goes outwards
    [SerializeField] Animator[] hearts;

    [Header("UI Effects + Other")]
    /*
    [SerializeField] Image[] gradients;
    [SerializeField] Color gradColor;
    */
    [SerializeField] MusicController musicContr;
    [SerializeField] GameObject bossUI;
    [SerializeField] PauseLore lore;
    [SerializeField] Pause pause;
    [SerializeField] GameObject[] aspectRatioBars;

    [SerializeField] TextMeshProUGUI yapText;
    [SerializeField] GameObject bossIcon;

    [SerializeField] Transform yapPosInital;
    [SerializeField] Transform yapPosFinal;

    [SerializeField] Image[] fadeIns;
    [SerializeField] GameObject bossDefeatColor;

    private Animator anim;
    public int attackID;
    bool bossTriggered = false;
    bool facingRight;

    private Coroutine currentAtt;
    private Coroutine currentSecondaryAtt;

    [SerializeField] Cutscene cutscene;
    [SerializeField] Cutscene death;
    [SerializeField] GameObject[] bossTriggers;

    [SerializeField] ParticleSystem sparks;
    [SerializeField] ParticleSystem smoke;


    [SerializeField] GameObject shadow;
    private FadeIn fade;

    [Header("Boss Attack Parameters")]
    [SerializeField] Transform topRight;
    [SerializeField] Transform bottomLeft;

    [SerializeField] GameObject kunaiLaser;
    [SerializeField] GameObject kunai;

    [SerializeField] GameObject pillarLaser;
    [SerializeField] GameObject flamePillar;

    [SerializeField] LayerMask laserLayers;
    [SerializeField] GameObject flameThrow;
    [SerializeField] Transform firePoint;

    public bool sentParrying = false;
    public string CoroutineDebug;

    public float attSpeed = 1f;

    private List<GameObject> instances;
    private int playerDeaths;
    private Inspect insp;
    private RunAndJump rj;
   

    //SoundFXManager.instance.PlayRandomProximitySoundEffectClip(telegraphSounds, gameObject, enemyVol);

    // Start is called before the first frame update
    protected override void Start()
    {
        

        instances = new List<GameObject>();
        fade = GetComponent<FadeIn>();
        anim = GetComponent<Animator>();
        bossTriggered = false;
        bossUI.SetActive(false);

        currentHealth = setHealth;
        base.Start();

        GameProgression deathCheck = player.GetComponent<GameProgression>();
        if (deathCheck.progress >= 2)
        {
            /*
            Transform corpseParent = transform.parent;
            Vector3 corpsePos = new Vector3(453.497009f, 11.2189999f, 0);
            GameObject corpseInstance = Instantiate(corpse, corpsePos, transform.rotation, corpseParent);

            foreach (GameObject trigger in bossTriggers)
            {
                Destroy(trigger);
            }
            */
            gameObject.SetActive(false);
            this.enabled = false;

        }


        nHealth = (currentHealth / setHealth);
        currentAtt = StartCoroutine(EmptyCorout());
        currentSecondaryAtt = currentAtt;

        attSpeed = 1f;

        var emissionS = sparks.emission;
        emissionS.enabled = false;

        emissionS = smoke.emission;
        emissionS.enabled = false;

        RunAndJump rj = player.GetComponent<RunAndJump>();
        Inspect insp = player.GetComponent<Inspect>();
    }

    void PlayAudio(AudioClip[] array, int rangeA, int rangeB)
    {
        //SOUND LIST:
        //Tele--
        /*  0-1 dash
            2-3 fly up
            4-5 divebrandish
            6 kunaibrandishhome
            7-8 kunai brandish spray
            9-10 kunai laser
            11 kunai center laser
        */
        //Attack
        /* 0-1 dash
         * 2-3 sentdivedown
         * 4 sentKnife

        */

        List<AudioClip> newArray = new List<AudioClip>();

        if (rangeA == rangeB)
        {
            SoundFXManager.instance.PlaySoundEffectClip(array[rangeA], transform.position, enemyVol);
        } else
        {
            //add elements of array from range a to range b to new list
            for (int i = rangeA; i < rangeB; i++)
            {
                newArray.Add(array[i]);
            }

            SoundFXManager.instance.PlayRandomSoundEffectClip(newArray.ToArray(), transform.position, enemyVol);
        }


    }

    IEnumerator EmptyCorout()
    {
        yield return null;
    }

    //WOW this is ugly
    void ResetBoss(Respawn resp)
    {
        if (player.GetComponent<GameProgression>().progress < 2)
        {
            
            transform.position = initialPos;
            StopAllCoroutines();

            transform.position = initialPos;
            anim.CrossFade("sentStand", 0, 0);

            bossTriggered = false;
            StartCoroutine(AspectRatioAnimation(0.1f, 1));
            bossUI.SetActive(false);
            ResetLives();
            currentHealth = setHealth;
            nHealth = (currentHealth / setHealth);
            currentAtt = StartCoroutine(EmptyCorout());
            currentSecondaryAtt = currentAtt;
            attSpeed = 1f;

            cutscene.start = false;
            cutscene.currentAction = 0;
            rb.gravityScale = 1f;

            attackID = 0;
            attacking = false;
            fading = false;
            playerDeaths++;
            rb.linearDamping = 1f;

            Color resetColor = new Color(1f, 1f, 1f, 0f);
            yapText.color = resetColor;
            //fix weird dash glitch where sent gets stuck in floor after respawn?
            Vector3 setPos = new Vector3(initialPos.x, initialPos.y + 1f, initialPos.z);
            lore.LoadText(0);
            transform.position = initialPos;

            var emissionS = sparks.emission;
            emissionS.enabled = false;

            emissionS = smoke.emission;
            emissionS.enabled = false;
        } else
        {
            return;
        }

    }

    void ResetLives()
    {
        lives = 3;
        foreach (Animator ani in hearts)
        {
            ani.CrossFade("Active", 0, 0);
        }
    }
    void OnEnable()
    {
        Respawn.OnRespawnEvent += ResetBoss;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    //normalizes a point in "Arena Space" (eg. the space within the topRight & bottomLeft area
    Vector2 NormalizePoint(Vector2 vec)
    {
        
        Vector2 bL2 = new Vector2(bottomLeft.position.x, bottomLeft.position.y);

        //distance from "origin"
        Vector2 dist = vec - bL2;

        Vector2 totalDist = topRight.position - bottomLeft.position;

        float normx = dist.x / totalDist.x;
        float normy = dist.y / totalDist.y;
        return new Vector2(normx, normy);
    }

    //converts from Arena Space to World Space
    Vector2 UnNormalizePoint(Vector2 norm)
    {
        Vector2 totalDist = topRight.position - bottomLeft.position;

        float worldX = (norm.x * totalDist.x) + bottomLeft.position.x;
        float worldY = (norm.y * totalDist.y) + bottomLeft.position.y;
        return new Vector2(worldX, worldY);
    }


    void Update()
    {
        if (yapText != null)
        {
            if (pause.isPaused)
            {
                yapText.gameObject.SetActive(false);
            }
            else
            {
                yapText.gameObject.SetActive(true);
            }
        }


        if (bossTriggered)
        {
            if (Input.GetKeyDown(KeyCode.Q) && player.GetComponent<RunAndJump>().fuel > 0.9f)
            {
                int randomSay = Random.Range(0, 3);
                if (randomSay == 0)
                {
                    StartCoroutine(Say("Flashy."));
                    StartCoroutine(IconFace(3, 3f));

                } else if (randomSay == 1)
                {
                    StartCoroutine(Say("Blue fire. How threatening."));
                    StartCoroutine(IconFace(3, 3f));
                }



                foreach (GameObject bar in aspectRatioBars)
                {
                    StartCoroutine(fade.PulseColorSpr(0.75f, bar, Color.blue));
                }
            }

            if (Input.GetKey(KeyCode.J) && player.GetComponent<RunAndJump>().grounded == true && player.GetComponent<RunAndJump>().sitting == false)
            {
                int randomSay = Random.Range(0, 3);
                if (randomSay == 0)
                {
                    StartCoroutine(Say("Are you... sitting down?"));
                    StartCoroutine(IconFace(4, 3f));
                }
                else if (randomSay == 1)
                {
                    StartCoroutine(Say("Now is not the time for relaxation!"));
                    StartCoroutine(IconFace(4, 3f));

                }
                else if (randomSay == 2)
                {
                    StartCoroutine(Say("Get up. We're not done."));
                    StartCoroutine(IconFace(4, 3f));
                }

            }
        }

        if (bossTriggered && attackID != 1)
        {
            AttackPattern();
        }

        FlipCheck();

        if (sentParrying == true & attackID != 9)
        {
            sentParrying = false;
        }

        if (instances != null)
        {
            ListCheck();
        }

    }

    void ListCheck()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] == null)
            {
                instances.Remove(instances[i]);
            }
        }
    }

    void FlipCheck()
    {
        Vector3 playerPos = player.transform.position;

        Vector3 dist = playerPos - transform.position;
        //not flamethrowing or in cOrner
        if (Mathf.Sign(dist.x) != Mathf.Sign(transform.localScale.x) && attackID != 4 && attackID != 6)
        {
            Flip();
        }
    }

    public void AttackPattern()
    {
        //i need to use a switch statement instead of spamming ifs like yandev. im not him. im not him. im not him. im a decent programmer. im a decent programmer. im a decent programmer. This IS dogshit programming though
        if (attackID == 0 && attacking == false)
        {
            int randomAttack = Random.Range(1, 6);


            //Phase 1 (Prefer DIVE)
            if (LessThanLives(3) == false)
            {
                randomAttack = 1;

                if (LessThanHealth(0.6f))
                {
                    randomAttack = Random.Range(1, 4);
                }

            }

            //Phase 2 (Either DASH or KUNAI)
            if (lives == 2)
            {
                randomAttack = Random.Range(1, 5);

                //no ground dash, will still dive from corner and center
                if (randomAttack == 1)
                {
                    randomAttack = 5;
                }
            }


            //PHASE 3 (Either DASH or FIRE)
            if (lives == 1)
            {
                if (LessThanHealth(0.9f) == false)
                {
                    randomAttack = 2;

                } else
                {
                    randomAttack = Random.Range(1, 6);
                    if (randomAttack == 2)
                    {
                        int doSideInstead = Random.Range(0, 2);
                        if (doSideInstead == 1)
                        {
                            randomAttack = 4;
                        }
                    }

                }
            }

            //PHASE 4
            //----------------------- 


            if (randomAttack == 2)
            {
                StopCoroutine(currentAtt);

                //so that they dont teleport on top of the player
                Vector2 centerPos = UnNormalizePoint(new Vector2(0.5f, 0.5f));
                Vector2 distFromCenter = (Vector2)player.transform.position - centerPos;

                if (distFromCenter.magnitude > 2f)
                {
                    currentAtt = StartCoroutine(GoToCenter());
                    CoroutineDebug = "CENTER";

                } else
                {
                    //go to corner if player is right at center
                    currentAtt = StartCoroutine(GoToRandomCorner());
                    CoroutineDebug = "CORNER";
                }


                
                
            }

            if (randomAttack == 3)
            {
                StopCoroutine(currentAtt);

                currentAtt = StartCoroutine(GoToRandomCorner());
                CoroutineDebug = "CORNER";
            }



            //int randomAttack = 2;
            if (rb.linearVelocity == Vector2.zero && GroundCheck() == true && randomAttack == 1)
            {
                StopCoroutine(currentAtt);
                currentAtt = StartCoroutine(DashToPlayer(false));
                CoroutineDebug = "DASH";
            }

            if (rb.linearVelocity == Vector2.zero && GroundCheck() == true && randomAttack == 5)
            {
                StopCoroutine(currentAtt);

                currentAtt = StartCoroutine(Parry(3f));
                CoroutineDebug = "PARRY";


            }



            if (randomAttack == 4)
            {
                StopCoroutine(currentAtt);

                currentAtt = StartCoroutine(GoToRandomSide());
                CoroutineDebug = "SIDE";
            }

        }
    }

    IEnumerator DiveToPlayer()
    {
        rb.gravityScale = 0;
        attacking = true;
        Vector3 playerPos = player.transform.position;

        Vector3 dist = playerPos - transform.position;
        Vector3 dir = dist.normalized;
        //skew direction downwards-- hit ground in front of player




        //get distance of player from ground

        float groundY = bottomLeft.position.y;
        float distFromGround = (playerPos.y - groundY);


        dir -= (Vector3.up * (distFromGround / 75f));

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;
        rb.AddForce(dir * rb.mass * 45f, ForceMode2D.Impulse);
        Debug.Log(dir * rb.mass * 45f);

        PlayAudio(attackSounds, 2, 2);
        anim.CrossFade("sentDive", 0, 0);
        yield return new WaitForSeconds(2f);
        anim.CrossFade("sentDefault", 0, 0);
        yield return new WaitForSeconds(1f);
        attackID = 0;
        attacking = false;
    }

    IEnumerator DashToPlayer(bool fromDive)
    {
        StopCoroutine(currentAtt);
        rb.linearDamping = 1f;
        int randomSay = Random.Range(1, 9);

        if (randomSay == 9)
        {
             StartCoroutine(Say("You can't outrun me."));
        }

        attacking = true;
        attackID = 1;

        if (fromDive == false)
        {
            anim.CrossFade("sentDashTele", 0, 0);

            PlayAudio(telegraphSounds, 0, 1);

            yield return new WaitForSeconds(1.4f);
        }
                
               

        Vector3 playerPos = player.transform.position;

        Vector3 dist = playerPos - transform.position;
        Vector3 dir = dist.normalized;

        rb.AddForce(Vector2.right * Mathf.Sign(dir.x) * 45f * rb.mass, ForceMode2D.Impulse);
        anim.CrossFade("sentDash", 0, 0);
        PlayAudio(attackSounds, 0, 1);

        yield return new WaitForSeconds(2f * attSpeed);
        anim.CrossFade("sentDefault", 0, 0);
        yield return new WaitForSeconds(1f * attSpeed);
        attackID = 0;
        attacking = false;

    }

    IEnumerator GoToCenter()
    {

        anim.CrossFade("sentFly", 0, 0);
        PlayAudio(telegraphSounds, 2, 3);

        //attack id = 5, possibly 10 or 11
        attacking = true;
        yield return new WaitForSeconds(0.25f);

        //everything 5 and above are areal attacks
        anim.CrossFade("sentCenterInit", 0, 0);

        attackID = 5;
        rb.gravityScale = 0;
        rb.linearDamping = 8f;


        Vector2 center = new Vector2(0.5f, 0.5f);


        Vector2 dir = ((UnNormalizePoint(center)) - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.position = (UnNormalizePoint(center));

        if (dir.y < 0)
        {
            rb.AddForce(new Vector2(dir.x, transform.up.y) * 16f * rb.mass, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(dir * 16f * rb.mass, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(2f);

        int randomAttack = Random.Range(0, 3);

        switch (lives)
        {

            case 3:
                //only dash
                randomAttack = 0;
                break;

            case 2:
                //either dash or kunai
                randomAttack = Random.Range(0, 101);

                if (randomAttack <= 60)
                {
                    //60% chance to dash
                    randomAttack = 0;
                } else
                {
                    //40% chance to kunai
                    randomAttack = 1;
                }
                break;


            case 1:
                // either dash or do fire pillars (should be rare)
                randomAttack = Random.Range(0, 101);

                if (randomAttack > 70)
                {
                    randomAttack = 2;

                } else if (randomAttack > 55)
                {
                    randomAttack = 1;

                } else
                {
                    randomAttack = 0;
                }
                break;

            default:
                break;
        }



        if (randomAttack == 1)
        {
            anim.CrossFade("sentCenterKunai", 0, 0);
            PlayAudio(telegraphSounds, 7, 8);

        }
        else if (randomAttack == 0)
        {
            anim.CrossFade("sentCenterDive", 0, 0);
            PlayAudio(telegraphSounds, 4, 5);

        }
        else if (randomAttack == 2)
        {
            anim.CrossFade("sentCenterPillar", 0, 0);
            PlayAudio(telegraphSounds, 11, 11);
        }



        yield return new WaitForSeconds(1f);

        if (randomAttack == 1)
        {
            StartCoroutine(Say("Choose your spot carefully."));
            attackID = 10;
            yield return new WaitForSeconds(1f);



            currentSecondaryAtt = StartCoroutine(KunaiSpray(16, -90, 22.5f, 3f, 0f, (Mathf.Abs(lives - 3))));

            SoundFXManager.instance.PlaySoundEffectClip(telegraphSounds[11], transform.position, enemyVol);

            yield return new WaitForSeconds(3f);

            SoundFXManager.instance.PlaySoundEffectClip(attackSounds[5], transform.position, enemyVol);
            
            if (lives < 2)
            {
                yield return new WaitForSeconds((float)lives + 1);
                //rb.gravityScale = 1f;
                attackID = 8;
                rb.linearDamping = 1f;
                currentAtt = StartCoroutine(DiveToPlayer());
                CoroutineDebug = "DIVE";
            }
            else
            {
                yield return new WaitForSeconds(1f);
                rb.gravityScale = 1f;
                attackID = 0;
                rb.linearDamping = 1f;
                attacking = false;
            }

        }
        else if (randomAttack == 2)
        {
            StartCoroutine(Say("Let's test your memory."));
            yield return new WaitForSeconds(1f);

            attackID = 11;

            Vector2 pillarPos = new Vector2(0.5f, 0.001f);
            //delay, lifetime, flashLength, pillarIntervalDelay, amount
            currentSecondaryAtt = StartCoroutine(PillarAssault(1f, 0.7f, 0.5f, 1f, (3f + (Mathf.Abs(3f - lives)))));

            //yield return new WaitForSeconds((2f + (2f * 3f)) + 1.2f);
            yield return new WaitForSeconds((0.5f + 2f) * (3f + (Mathf.Abs(3f - lives))));
            //yield return new WaitForSeconds((pillarIntervalDelay + flashLength + delay + lifetime) * pillarAmount);



            rb.gravityScale = 1f;
            attackID = 0;
            rb.linearDamping = 1f;
            attacking = false;

        }
        else
        {

            //rb.gravityScale = 1f;
            attackID = 8;
            rb.linearDamping = 1f;
            currentAtt = StartCoroutine(DiveToPlayer());
            CoroutineDebug = "DIVE";
        }

    }

    IEnumerator GoToRandomCorner()
    {
        PlayAudio(telegraphSounds, 2, 3);
        anim.CrossFade("sentFly", 0, 0);
        //attack id = 6, 7
        attacking = true;
        yield return new WaitForSeconds(0.25f);



        //0 is left, 1 is right
        int corner = Random.Range(0, 2);
       

        //check so that sent doesnt teleport on TOP of player
        /*

            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

            float cornerDistA = (playerPos - (UnNormalizePoint(new Vector2(0, 0)))).magnitude;
            float cornerDistB = (playerPos - (UnNormalizePoint(new Vector2(1, 0)))).magnitude;

            if (cornerDistA < 2f)
            {
                corner = 1;

            }
            else if (cornerDistB < 2f)
            {
                corner = 0;
            }
        */

        //actually nevermind. if sent teleports on the player they deserve it
        if (DistFromPlayer() < 3f)
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

            float cornerDistA = (playerPos - (UnNormalizePoint(new Vector2(0, 0)))).magnitude;
            float cornerDistB = (playerPos - (UnNormalizePoint(new Vector2(1, 0)))).magnitude;

            if (cornerDistA < 2f)
            {
                corner = 1;

            }
            else if (cornerDistB < 2f)
            {
                corner = 0;
            }
        }

        //everything 5 and above are areal attacks
        attackID = 6;
        rb.gravityScale = 0;
        rb.linearDamping = 8f;

        Vector2 cornerV = new Vector2(Mathf.Abs((float)corner - 0.1f), 0.6f);
        Vector2 trueCorner = new Vector2((float)corner, 1f);


        Vector2 dir = ((UnNormalizePoint(trueCorner)) - (UnNormalizePoint(cornerV))).normalized;


        transform.position = (UnNormalizePoint(cornerV));

        int randomAttack = Random.Range(0, 3);
        rb.AddForce(dir * 16f * rb.mass, ForceMode2D.Impulse);
        anim.CrossFade("sentClimbInit", 0, 0);


        //face towards player 
        if (corner == 0)
        {
            facingRight = true;
            Vector3 Scaler = transform.localScale;
            Scaler.x = 1;
            transform.localScale = Scaler;
        } else
        {
            facingRight = false;
            Vector3 Scaler = transform.localScale;
            Scaler.x = -1;
            transform.localScale = Scaler;
        }


        switch (lives) {

            case 3:
                //only dash
                randomAttack = 0;
                break;

            case 2:

                //either dash or kunai
                randomAttack = Random.Range(0, 101);

                if (randomAttack > 40)
                {
                    randomAttack = 1;

                } else
                {
                    randomAttack = 0;
                }
                break;

            case 1:

                //either dash or fire pillar
                randomAttack = Random.Range(0, 101);

                if (randomAttack > 60)
                {
                    randomAttack = 2;
                } else
                {
                    randomAttack = 0;
                }
                break;

            default:
                break;
                
        }






        yield return new WaitForSeconds(2.2f * attSpeed);


        if (randomAttack == 1)
        {
            StartCoroutine(Say("Catch this!"));


            anim.CrossFade("sentClimbKunai", 0, 0);

            int willHome = Random.Range(0, 4);
            //int willHome = 3;

            

            if (willHome != 3)
            {
                PlayAudio(telegraphSounds, 7, 8);

                yield return new WaitForSeconds(1f * attSpeed);
                currentSecondaryAtt = StartCoroutine(KunaiSpray(6, -45, 10, 2f * attSpeed, 0.25f, (Mathf.Abs(3 - lives) + 1)));
                yield return new WaitForSeconds(((6 * 0.25f) + (2f * attSpeed)) + 0.2f);

            } else
            {
                SoundFXManager.instance.PlaySoundEffectClip(telegraphSounds[6], transform.position, enemyVol);

                yield return new WaitForSeconds(1f * attSpeed);
                currentSecondaryAtt = StartCoroutine(KunaiHome(8, 1f, 0.56f));
                yield return new WaitForSeconds(8f * 0.56f);

            }

            if (lives < 3)
            {
                anim.CrossFade("sentClimbDive", 0, 0);
                PlayAudio(telegraphSounds, 4, 5);

                yield return new WaitForSeconds(attSpeed);
               // rb.gravityScale = 1f;
                attackID = 8;
                rb.linearDamping = 1f;
                currentAtt = StartCoroutine(DiveToPlayer());
                CoroutineDebug = "DIVE";
            }
            else
            { 
                yield return new WaitForSeconds(attSpeed);
                rb.gravityScale = 1f;
                attackID = 0;
                rb.linearDamping = 1f;
                attacking = false;
            }




        }
        else if (randomAttack == 0)
        {
            anim.CrossFade("sentClimbDive", 0, 0);
            PlayAudio(telegraphSounds, 4, 5);

            yield return new WaitForSeconds(1f * attSpeed);

           // rb.gravityScale = 1f;
            attackID = 8;
            rb.linearDamping = 1f;
            currentAtt = StartCoroutine(DiveToPlayer());
            CoroutineDebug = "DIVE";

        } else if (randomAttack == 2)
        {
            if (lives > 0)
            {
                StartCoroutine(Say("Parry this."));
            } else
            {
                StartCoroutine(Say("Die!"));
            }


            anim.CrossFade("sentClimbFire", 0, 0);
            currentSecondaryAtt = StartCoroutine(PillarAtPlayer((Mathf.Abs(3 - lives) + 2), 1f, 0.8f * attSpeed, 0.25f, 1.8f));
            yield return new WaitForSeconds(((1.8f + 0.25f) * (Mathf.Abs(3f - lives) + 1f)) - 3f);

            if (lives < 3)
            {
                yield return new WaitForSeconds(1.35f * attSpeed);
               // rb.gravityScale = 1f;
                attackID = 8;
                rb.linearDamping = 1f;
                currentAtt = StartCoroutine(DiveToPlayer());
                CoroutineDebug = "DIVE";
            }
        }

    }


    IEnumerator GoToRandomSide()
    {
        anim.CrossFade("sentCapeHide", 0, 0);
        //attack id = 2, 3, possibly 4
        attacking = true;
        yield return new WaitForSeconds(0.62f);

        anim.CrossFade("sentSide", 0, 0);
        //0 is left, 1 is right
        int side = 0;

        //teleport to side farthest from player
        
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

            float cornerDistA = (playerPos - (UnNormalizePoint(new Vector2(0, 0)))).magnitude;
            float cornerDistB = (playerPos - (UnNormalizePoint(new Vector2(1, 0)))).magnitude;

            if (cornerDistB >= cornerDistA)
            {
                side = 1;

            }

        //everything 2 - 5 are grounded attacks
        attackID = 2 + side;


        Vector2 sideV = new Vector2(Mathf.Abs((float)side - 0.1f), 0.5f);
        Vector2 trueSide = new Vector2((float)side, 0f);


        Vector2 dir = ((UnNormalizePoint(trueSide)) - (UnNormalizePoint(sideV))).normalized;


        transform.position = (UnNormalizePoint(sideV));

        rb.AddForce(dir * 16f * rb.mass, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f * attSpeed);

        int randomAttack = Random.Range(0, 3);
        //int randomAttack = 2;

        switch (lives)
        {
            case 2:
                //only kunai
                randomAttack = 1;
                break;

            case 1:
                //prefer flamethrower, can kunai too
                randomAttack = Random.Range(0, 101);
                if (randomAttack > 30)
                {
                    randomAttack = 2;

                } else
                {
                    randomAttack = 1;
                }
                break;

            default:
                break;
        }


        if (randomAttack == 1)
        {
            anim.CrossFade("sentSideKunai", 0, 0);
            currentAtt = StartCoroutine(KunaiAttack(0, 0.8f, 2, -0.28f, false));
            PlayAudio(telegraphSounds, 9, 10);

            CoroutineDebug = "KNIFE";
            yield return new WaitForSeconds(1.3f);

            attackID = 0;
            attacking = false;

        } else if (randomAttack == 2)
        {
            yield return new WaitForSeconds(0.3f);
            //flamethrower already manages attackID & attacking flow
            currentAtt = StartCoroutine(Flamethrower(side, 15f, 3.5f * attSpeed));
            CoroutineDebug = "FLAME";

        } else
        {
            attackID = 0;
            attacking = false;
        }



    }

    public IEnumerator ParryDash()
    {


        if ((currentHealth - 2) > 0)
        {

            StopCoroutine(currentAtt);
            StopCoroutine(currentSecondaryAtt);

            Vector3 playerPos = player.transform.position;

            Vector3 dist = playerPos - transform.position;

            rb.linearVelocity = Vector2.zero;
            TakeDamage(2, Vector2.zero);

            rb.AddForce(Vector2.right * Mathf.Sign(dist.x) * -15f * rb.mass, ForceMode2D.Impulse);

            anim.CrossFade("sentParried", 0, 0);
            yield return new WaitForSeconds(0.3f);

            rb.linearDamping = 8f;

            anim.CrossFade("sentDefault", 0, 0);
            yield return new WaitForSeconds(0.5f + (0.8f * attSpeed));

            rb.linearVelocity = Vector2.zero;
            rb.linearDamping = 1f;

            attackID = 0;
            attacking = false;
        } else
        {
            TakeDamage(2, Vector2.zero);
            //allows stagger to play out as normal
        }




    }

    //NO NORMALIZATION-- PASS IN WORLD-SPACE DISTANCES
    IEnumerator Flamethrower(int corner, float distFromCorner, float duration)
    {
        anim.CrossFade("sentFlamethrower", 0, 0);
        attackID = 4;
        attacking = true;

        PlayAudio(attackSounds, 8, 8);
        yield return new WaitForSeconds(0.4f);
        //RIGHT NOW you are either in left corner or right
        // 0 is left, 1 is right

        float totalDist = 0f;
        Vector2 cornerPos = Vector2.zero;
        if (corner > 0)
        {
            totalDist = (1.0f - distFromCorner);

            //bottom right. These are in world space so no unnormalization is needed
            cornerPos = new Vector2(topRight.position.x, bottomLeft.position.y);

        } else
        {
            totalDist = Mathf.Abs(0.0f - distFromCorner);

            //bottom left.  These are in world space so no unnormalization is needed
            cornerPos = new Vector2(bottomLeft.position.x, bottomLeft.position.y);
        }


        //no y change cause i dont want sentinel to glitch thru terrain or sum shit. cosntant is in there so that sentinel doesnt end up literally INSIDE the wall (same reason)
        Vector2 endPos = new Vector2(cornerPos.x + totalDist + 1f, transform.position.y);

        //if this returns one of those stupid VECTOR2 errors im killing myself
        Vector2 initialPos = transform.position;
        Vector2 flamePos = new Vector2((firePoint.position.x + (1.7f * transform.localScale.x)), firePoint.position.y);

        
        GameObject flame = Instantiate(flameThrow, flamePos, Quaternion.identity, this.transform);
        instances.Add(flame);

        Destroy(flame, duration);

        //lerp distFromCorner tiles in opposite direction while spewing flames
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            transform.position = Vector2.Lerp(initialPos, endPos, (i / duration));
            yield return null;
        }

        attackID = 0;
        attacking = false;
    }

    


    IEnumerator KunaiSpray(int amt, float initialAngle, float angleInterval, float telegraphDelay, float iterationDelay, int repeatAmt)
    {
        for (int i = 0; i < amt; i++)
        {
            float turnAmt = i * angleInterval;

            float thisAngle = initialAngle + turnAmt;
            if (i != 0 && i % 2 == 0)
            {
                thisAngle = initialAngle - turnAmt;
                //even iteration, rotate counterclockwise from initial angle
            }

            if (amt < 15)
            {
                PlayAudio(telegraphSounds, 9, 10);
            }

            StartCoroutine(KunaiAttack(thisAngle, telegraphDelay, repeatAmt, 0f, (amt > 15)));

            

            yield return new WaitForSeconds(iterationDelay);
        }
    }

    IEnumerator KunaiHome(int amt, float telegraphDelay, float iterationDelay)
    {
        for (int i = 0; i < amt; i++)
        {

            Vector2 distFromPlayer = (player.transform.position - transform.position);


            float thisAngle = Mathf.Rad2Deg * Mathf.Atan2(distFromPlayer.y, distFromPlayer.x);


            if (transform.localScale.x < 0)
            {
                thisAngle = (180 - thisAngle);
            }
            StartCoroutine(KunaiAttack(thisAngle, telegraphDelay, 1, 0f, false));
            yield return new WaitForSeconds(iterationDelay);
        }
    }

    IEnumerator KunaiAttack(float angle, float delay, int repetitions, float yOffset, bool center)
    {

        float initFacingDir = transform.localScale.x;

        //telegraph attack
        float vX = Mathf.Cos(Mathf.Deg2Rad * angle) * initFacingDir;
        float vY = Mathf.Sin(Mathf.Deg2Rad * angle);
        Vector2 kunaiVector = new Vector2(vX, vY);
        Vector2 initPos = new Vector2(transform.position.x, transform.position.y + yOffset);

        RaycastHit2D laserRay = Physics2D.Raycast(initPos, kunaiVector, 50f, laserLayers);

        GameObject laserObj = Instantiate(kunaiLaser, transform.position, transform.rotation);

        instances.Add(laserObj);

        LineRenderer laserLine = laserObj.GetComponent<LineRenderer>();
        
        //this is in local space, it should be where the ray hits the level buttt im not used to raycasts 
        Vector2 localLaserPoint = new Vector2(kunaiVector.x * laserRay.distance, kunaiVector.y * laserRay.distance + yOffset);

        //converting to world space SHOULD be simple, fingers crossed this works
        Vector2 kunaiReal = kunaiVector + (Vector2)transform.position;
        Vector2 laserEndReal = localLaserPoint + (Vector2)transform.position;

        Vector3 finalPos = new Vector3(laserEndReal.x, laserEndReal.y, 0f);

        //only 2 points in line, so take last point
        laserLine.SetPosition(0, initPos);
        laserLine.SetPosition(1, finalPos);

        StartCoroutine(FlashLaser(laserLine, (delay - 0.05f)));
        Destroy(laserObj, delay);
        
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < repetitions; i++)
        {
            if (center == false)
            {
                SoundFXManager.instance.PlaySoundEffectClip(attackSounds[4], transform.position, enemyVol);
            }


            FireKunai(angle, 30f, initPos, initFacingDir);
            yield return new WaitForSeconds(0.3f);
        }


    }

    IEnumerator Stagger(float dur)
    {
        attackID = -1;

        StopCoroutine(currentAtt);
        StopCoroutine(currentSecondaryAtt);

        if (instances != null)
        {
            KillInstances();

        }


        SoundFXManager.instance.PlaySoundEffectClip(deathSounds[0], transform.position, enemyVol);
        StartCoroutine(musicContr.PulseVolume(1.5f));
        //flash aspectratioBars
        foreach (GameObject bar in aspectRatioBars)
        {
            StartCoroutine(fade.PulseColorSpr(0.75f, bar, Color.white));
        }

        attacking = true;
        anim.CrossFade("sentStagger", 0, 0);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;
        
        rb.AddForce((((-transform.right) * transform.localScale.x) - transform.up) * 30f * rb.mass, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dur);

       
        
        anim.CrossFade("sentDefault", 0, 0);
        attackID = 0;
        attacking = false;
    }

    IEnumerator PillarAssault(float delay, float lifetime, float flashLength, float pillarIntervalDelay, float pillarAmount)
    {


        //we want RANDOM ORDER OF PILLARS
        List<Vector2> pillarPositions = new List<Vector2>();

        for (float i = 1; i < (pillarAmount + 1); i++)
        {

            float newX = (i / pillarAmount);

            //i know i can use modulo but fuck you 
            if (pillarAmount == 3 && i == 3)
            {
                newX = 0.9f;
            }

            pillarPositions.Add(new Vector2(newX - 0.1f, 0f));


        }

        int initialCount = pillarPositions.Count;
    

        for (int i = 0; i < initialCount; i++)
        {
            int randomIndex = Random.Range(0, pillarPositions.Count);

            StartCoroutine(FlamePillarAttack(pillarPositions[randomIndex], (delay + (pillarIntervalDelay * initialCount)), lifetime, flashLength));
            //total time for each attack is (flashlength + delay + lifetime)

            pillarPositions.Remove(pillarPositions[randomIndex]);

            yield return new WaitForSeconds(pillarIntervalDelay);
        }

        //total time for entire assault
        //yield return new WaitForSeconds((flashlength + delay + lifetime + pillarIntervalDelay) * pillarAmount);


    }

    IEnumerator PillarAtPlayer(int amt, float loopDelay, float pillarLifetime, float flashLength, float telegraphDelay)
    {
        for (int i = 0; i < amt; i++)
        {
            Vector2 spawnPos = NormalizePoint(player.transform.position);
            spawnPos.y = 0;

            StartCoroutine(FlamePillarAttack(spawnPos, telegraphDelay, pillarLifetime, flashLength));

            yield return new WaitForSeconds(loopDelay);
        }
    }

    IEnumerator FlamePillarAttack(Vector2 normalIPos, float delay, float lifetime, float flashLength)
    {
        Vector2 initPos = UnNormalizePoint(normalIPos);

        RaycastHit2D laserRay = Physics2D.Raycast(initPos, transform.up, 50f, laserLayers);

        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        GameObject laserObj = Instantiate(pillarLaser, transform.position, rotation);
        instances.Add(laserObj);

        LineRenderer laserLine = laserObj.GetComponent<LineRenderer>();

        Vector2 localLaserPoint = new Vector2(0f, laserRay.distance);
        Vector2 laserEndReal = localLaserPoint + initPos;

       

        Vector3 finalEndPos = new Vector3(laserEndReal.x, laserEndReal.y, 0f);

        laserLine.SetPosition(0, initPos);
        laserLine.SetPosition(1, finalEndPos);

        Destroy(laserObj, delay + flashLength);

        if (flashLength > 0)
        {
            StartCoroutine(FlashLaser(laserLine, flashLength));
            PlayAudio(telegraphSounds, 12, 13);
        }

       
        yield return new WaitForSeconds(delay);

        float pillarRange = 13f;

        Vector2 pillarPosI = new Vector2(initPos.x, initPos.y + (laserRay.distance / 2) - pillarRange);

        Vector2 pillarPosF = new Vector2(initPos.x, initPos.y + (laserRay.distance / 2) + pillarRange);

        GameObject pillarAttack = Instantiate(flamePillar, pillarPosI, rotation);
        instances.Add(pillarAttack);
        PlayAudio(attackSounds, 6, 7);

        //pillar moving up
        for (float i = 0; i < lifetime; i += Time.deltaTime)
        {
            Vector2 lerpV = Vector2.Lerp(pillarPosI, pillarPosF, (i / lifetime));
            pillarAttack.transform.position = lerpV;

            yield return null;
        }

        Destroy(pillarAttack, lifetime + 0.5f);

    }

    IEnumerator FlashLaser(LineRenderer line, float duration)
    {
        line.material = new Material(Shader.Find("Sprites/Default"));
        Gradient cGrad = line.colorGradient;
        List<float> initAlphaValues = new List<float>();
        List<float> initTimeValues = new List<float>();

        //record initial values of gradient
        for (int i = 0; i < cGrad.alphaKeys.Length; i++)
        {
            //get inital alpha of gradient keys
            GradientAlphaKey alp = cGrad.alphaKeys[i];
            initAlphaValues.Add(alp.alpha);

        }

        for (int i = 0; i < cGrad.alphaKeys.Length; i++)
        {
            //get inital time of gradient keys
            GradientAlphaKey alp = cGrad.alphaKeys[i];
            initTimeValues.Add(alp.time);

        }

        List<GradientAlphaKey> keyList = new List<GradientAlphaKey>();

        

        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            if (line != null)
            {
                Gradient newGrad = new Gradient();
                //runs as time passes
                keyList.Clear();

                for (int j = 0; j < cGrad.alphaKeys.Length; j++)
                {

                    //runs for each alpha key

                    //lerp alpha key
                    float lerpAlpha = Mathf.Lerp(initAlphaValues[j], 0f, (i / duration));

                    //Make new alpha key, then add to list
                    GradientAlphaKey jAlphaKey = new GradientAlphaKey(lerpAlpha, initTimeValues[j]);
                    keyList.Add(jAlphaKey);


                }
                //Set gradient to list of alpha keys

                newGrad.SetKeys(cGrad.colorKeys, keyList.ToArray());
                line.colorGradient = newGrad;
                
            }
            yield return null;

        }

        //get array of alpha keys
        GradientAlphaKey[] alpArr = cGrad.alphaKeys;

        for (int i = 0; i < alpArr.Length; i++)
        {
            alpArr[i] = new GradientAlphaKey(0f, alpArr[i].time);
        }

        //set currentGradient to finalGradient
        cGrad.SetKeys(cGrad.colorKeys, alpArr);
        line.colorGradient = cGrad;
    }



    void FireKunai(float angle, float force, Vector2 initVect, float initDir)
    {
        float vX = Mathf.Cos(Mathf.Deg2Rad * angle) * initDir;
        float vY = Mathf.Sin(Mathf.Deg2Rad * angle);
        Vector2 kunaiVector = new Vector2(vX, vY);

        //probably better way of phrasing this
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        if (transform.localScale.x < 0)
        {
           rotation = Quaternion.Euler(0, 0, (180 - angle));
        }


        GameObject kunObj = Instantiate(kunai, initVect, rotation);
        instances.Add(kunObj);

        kunObj.GetComponent<Rigidbody2D>().AddForce(kunaiVector * force, ForceMode2D.Impulse);
        Destroy(kunObj, 5f);
    }


    IEnumerator Parry(float dur)
    {

        attacking = true;

        //parry attack ID is like 9 i guess man i dont know
        attackID = 9;


        int randomSay = Random.Range(0, 3);

        if (randomSay == 1)
        {
            StartCoroutine(Say("Come at me."));
            StartCoroutine(IconFace(3, (dur + (0.7f * attSpeed))));

        }

        anim.CrossFade("sentTaunt", 0, 0);
        PlayAudio(telegraphSounds, 14, 14);
        yield return new WaitForSeconds(0.7f * attSpeed);

        sentParrying = true;
        //anim.CrossFade("sentParry", 0, 0);
        yield return new WaitForSeconds(dur);
        sentParrying = false;
        attackID = 0;
        
        attacking = false;
    }

    IEnumerator SentParrySuccess()
    {
        
        Shooting shoot = player.GetComponent<Shooting>();
        RunAndJump rj = player.GetComponent<RunAndJump>();
        Health health = player.GetComponent<Health>();
        //stop parry coroutine
        StopCoroutine(currentAtt);
        attackID = 12;
        
        anim.CrossFade("sentParrySuccess", 0, 0);
        
        yield return new WaitForSeconds(0.3f);
        sentParrying = false;
        //shoot.StopAllCoroutines();
        shoot.enabled = false;
        //rj.grabbed = true;

        StartCoroutine(health.TakePDamage(2, -transform.right));
        PlayAudio(attackSounds, 9, 9);
        rj.AddFuel(0.01f);
        rj.fuel = 0;

        foreach (GameObject bar in aspectRatioBars)
        {
            StartCoroutine(fade.PulseColorSpr(0.75f, bar, Color.red));
        }

        yield return new WaitForSeconds(1f);

        anim.CrossFade("sentDefault", 0, 0);
        shoot.enabled = true;
        //rj.grabbed = false;

        attackID = 0;
        attacking = false;

       
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            //parry control
            if (coll.gameObject.GetComponent<Shooting>().isParrying == true)
            {
                //is dashing
                if (attackID == 1 && attacking == true && (Mathf.Sign(coll.gameObject.transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x)))
                {

                    StartCoroutine(ParryDash());
                    StartCoroutine(coll.gameObject.GetComponent<Shooting>().ParrySuccess());
                }
                //is diving
                if (attackID == 8 && attacking == true && (Mathf.Sign(coll.gameObject.transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x)))
                {

                    StartCoroutine(ParryDash());
                    StartCoroutine(coll.gameObject.GetComponent<Shooting>().ParrySuccess());
                }

            } else
            {
                //is dashing
                if (attackID == 1 && attacking == true)
                {

                    rb.linearVelocity = Vector2.zero;
                    Vector3 playerPos = player.transform.position;
                    Vector3 dist = playerPos - transform.position;
                    rb.AddForce(Vector2.right * Mathf.Sign(dist.x) * -4f * rb.mass, ForceMode2D.Impulse);

                }
            }
        }


        if (LayerMask.LayerToName(coll.gameObject.layer) == "Ground")
        {
            //hit the ground while diving
            if (attackID == 8)
            {
                currentAtt = StartCoroutine("DiveLand");
                CoroutineDebug = "LAND";

            }
        }
    }

    IEnumerator DiveLand()
    {
        SoundFXManager.instance.PlaySoundEffectClip(attackSounds[3], transform.position, enemyVol);
        rb.gravityScale = 1f;
        anim.CrossFade("sentDiveLand", 0, 0);
        rb.linearVelocity = Vector2.zero;
        StopCoroutine(currentAtt);

        if (lives < 3)
        {
            attackID = 1;
            StopCoroutine(currentAtt);
            yield return new WaitForSeconds(0.5f);
            anim.CrossFade("sentDiveToDash", 0, 0);
            yield return new WaitForSeconds(0.3f);
            attacking = true;
            currentAtt = StartCoroutine(DashToPlayer(true));
            CoroutineDebug = "DASH";
        } else
        {
            yield return new WaitForSeconds(2f);
            anim.CrossFade("sentDefault", 0, 0);
            yield return new WaitForSeconds(0.1f);
            attackID = 0;
            attacking = false;

        }
    }

    IEnumerator EndFight()
    {
        lore.LoadText(0);
        rb.gravityScale = 1f;
        bossTriggered = false;
        attackID = -2;
        StopCoroutine(currentAtt);
        StopCoroutine(currentSecondaryAtt);
        Time.timeScale = 0.0f;
        //change Time.fixedDeltaTime too;

        musicContr.music.Stop();
        SoundFXManager.instance.PlaySoundEffectClip(deathSounds[1], transform.position, enemyVol);
        GameObject effect = Instantiate(bossDefeatColor, transform.position, Quaternion.identity, shadow.transform);
        UIManager.instance.playerUI.SetActive(false);
        UIManager.instance.bossUI.SetActive(false);

        yield return new WaitForSecondsRealtime(3.2f);
        StartCoroutine(fade.PulseFadeSpr(0.34f, effect));
        Destroy(effect, 0.4f);

        SoundFXManager.instance.PlaySoundEffectClip(damageSounds[3], transform.position, enemyVol);

        UIManager.instance.playerUI.SetActive(true);
        UIManager.instance.bossUI.SetActive(true);
        StartCoroutine(AspectRatioAnimation(1f, 1));

        Time.timeScale = 1.0f;

        StartCoroutine(Say("Enough."));
        
        
        attacking = false;


        anim.CrossFade("sentCapeHide", 0, 0);
        yield return new WaitForSeconds(0.62f);

        int side = 0;
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

        Vector2 newPos = new Vector2(0.5f, 0.34f);


      //  Vector2 trueSide = new Vector2(Mathf.Abs((float)side - 0.15f), 0.25f);
        transform.position = (UnNormalizePoint(newPos));

        anim.CrossFade("sentSide", 0, 0);

        yield return new WaitForSeconds(1.25f);
        death.PlayCutscene();
    }


    public void TriggerBoss()
    {
        StartCoroutine(TriggerCoroutine());
    }

    IEnumerator TriggerCoroutine()
    {
        bossUI.SetActive(true);
        StartCoroutine(AspectRatioAnimation(1f, 0));
        lore.LoadText(6);
        lore.bossAggro = true;
        SetHealth(0f);
        SetDecay(1f);
        StartCoroutine(BossBarAnimation(0.8f));
        yield return new WaitForSeconds(2f);
        anim.CrossFade("sentWingStand", 0, 0);
        yield return new WaitForSeconds(1.5f);

        if (playerDeaths > 0)
        {
            StartCoroutine(Say("Back again?"));
        } else
        {
            StartCoroutine(Say("I'll make this quick."));
        }
        
        anim.CrossFade("sentDefault", 0, 0);
        bossTriggered = true;
    }

    float DistFromPlayer()
    {
        Vector2 dist = player.transform.position - transform.position;
        return dist.magnitude;
    }

    bool LessThanLives(float n)
    {
        if (lives < n)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //hLim is normalized
    bool LessThanHealth(float hLim)
    {
        if (hLim > (currentHealth / setHealth))
        {
            return true;

        } else
        {
            return false;
        }
    }

    bool GroundCheck(float radius = 0.32f)
    {
        Vector3 groundCheckPos = transform.position - (Vector3.up * 3f);
        LayerMask groundLayer = 3;
        Collider2D[] groundCheckColliders = Physics2D.OverlapCircleAll(groundCheckPos, radius, groundLayer);

        bool isThereGround = (groundCheckColliders != null);
        return isThereGround;

    }

    void KillInstances()
    {
        if (instances.Count > 0)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] != null)
                {
                    Destroy(instances[i]);
                    //will automatically be cleared from list due to func in Update()

                }

            }
        }

    }







    ///ANIMATION UI STUFF
    ///











    void SetDecay(float p)
    {
        healthBar.material.SetFloat("_WhiteDecay", p);
    }

    void SetHealth(float p)
    {
        healthBar.material.SetFloat("_Health", p);
    }

    IEnumerator HealthEff(float initHealth)
    {
        
       
        float nIHealth = (initHealth / setHealth);

        SetDecay(nIHealth);
        

        draining = true;


        float lerpF = nIHealth;

        while (lerpF > nHealth)
        {
            //using lerp isnt actually constant
            //lerpF = Mathf.Lerp(nIHealth, nHealth, (i));

            lerpF = Mathf.Clamp((lerpF - (Time.deltaTime / (duration * 10))), nHealth, nIHealth);

            SetDecay(lerpF);
            yield return null;
        }
        draining = false;

    }

    
    public override void TakeDamage(float damage, Vector2 knockback)
    {
        if (sentParrying == false)
        {
            if (invincible != true)
            {
                currentHealth -= damage;
                PlayAudio(damageSounds, 0, 2);

                StartCoroutine("Invincibility");

                nHealth = (currentHealth / setHealth);
                SetHealth(nHealth);

                if (!draining)
                {
                    StartCoroutine(HealthEff((currentHealth + damage)));
                }


            }

            if (currentHealth <= 0 && lives > 0)
            {
                StartCoroutine(BreakHeart((lives - 1)));
                lives--;
                IconLives();
                attSpeed -= 0.15f;

                currentHealth = setHealth;
                SetHealth(currentHealth / setHealth);
                draining = false;

                StartCoroutine(Stagger(5f));

            }
            else if (currentHealth <= 0 && bossTriggered)
            {
                StartCoroutine(EndFight());
            }

            StartCoroutine(fade.PulseFadeSpr(0.15f, shadow));

        } else 
        {
            StartCoroutine(Say("Caught you."));
            StartCoroutine(IconFace(3, 1.4f));
            StartCoroutine(SentParrySuccess());

            shadow.GetComponent<SpriteRenderer>().color = Color.white;
            StartCoroutine(fade.PulseFadeSpr(0.3f, shadow));
            shadow.GetComponent<SpriteRenderer>().color = Color.red;
        }
        

        return;
    }
    //stupid change that ill correct later
    public void GuaranteedDamage(float damage, Vector2 knockback)
    {
        
            if (invincible != true)
            {
                currentHealth -= damage;
                PlayAudio(damageSounds, 0, 2);

                StartCoroutine("Invincibility");

                nHealth = (currentHealth / setHealth);
                SetHealth(nHealth);

                if (!draining)
                {
                    StartCoroutine(HealthEff((currentHealth + damage)));
                }


            }

            if (currentHealth <= 0 && lives > 0)
            {
                StartCoroutine(BreakHeart((lives - 1)));
                lives--;
                IconLives();
                attSpeed -= 0.15f;

                currentHealth = setHealth;
                SetHealth(currentHealth / setHealth);
                draining = false;

                StartCoroutine(Stagger(5f));

            }
            else if (currentHealth <= 0 && bossTriggered)
            {
                StartCoroutine(EndFight());
            }

            StartCoroutine(fade.PulseFadeSpr(0.15f, shadow));

        
    }

    protected override IEnumerator Invincibility()
    {
        invincible = true;

        yield return new WaitForSeconds(invincibilityTime);

        invincible = false;

    }


    IEnumerator Say(string yap)
    {
        if (!fading)
        {
            yapText.text = yap;

            //average reading speed is around 24 characters per second
            float duration = yap.Length / 24f;

            StartCoroutine(SayDur(duration));
            StartCoroutine(ShakeIcon(0.25f));
            StartCoroutine(MoveText(1f));

            StartCoroutine(FadeTextIn(1f));

            yield return new WaitForSeconds(0.75f + duration * 1.5f);

            StartCoroutine(FadeTextOut(duration / (4/3)));
        }

    }

    IEnumerator SayDur(float duration = 3f)
    {
        //average reading speed is around 24 characters per second
        fading = true;
        yield return new WaitForSeconds(duration);
        fading = false;
    }

    IEnumerator BreakHeart(int iD)
    {
        StartCoroutine(IconFace(1, 5f));
        IconLives();

        if (lives == 3)
        {
            StartCoroutine(Say("Gah..."));

            


        } else if (lives == 1)
        {
            StartCoroutine(Say("I'm not dead yet."));
            var emission = smoke.emission;
            emission.enabled = true;

        } else if (lives == 2)
        {
            StartCoroutine(Say("Urgh..."));
            var emission = sparks.emission;
            emission.enabled = true;

        }
        
        hearts[iD].CrossFade("Heartbreak", 0, 0);
        yield return new WaitForSeconds(0.3f);

        hearts[iD].CrossFade("Broken", 0, 0);

        
    }

    IEnumerator BossBarAnimation(float dur)
    {
        StartCoroutine(BossBarFadeIn(dur));
        yield return new WaitForSeconds(dur);
        StartCoroutine(BossBarFill(dur * 4));
    }
    IEnumerator BossBarFadeIn(float dur)
    {
        float alpha = 0;

        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            foreach (Image img in fadeIns)
            {
                Color c = img.color;
                Color newC = new Color(c.r, c.g, c.b, alpha);
                img.color = newC;
            }

            alpha = Mathf.Lerp(0, 1, (i / dur));
            yield return null;
        }

        
    }

    IEnumerator BossBarFill(float dur)
    {
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float nH = Mathf.Lerp(0, 1, (i / dur));
            currentHealth = (nH * setHealth);
            SetHealth(nH);

            yield return null;
        }
    }

    IEnumerator MoveText(float dur)
    {
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float normI = Mathf.Lerp(0, 1, (i / dur));
            float berpT = BezierBlend(normI);

            yapText.gameObject.transform.position = Vector2.Lerp(yapPosInital.position, yapPosFinal.position, berpT);
            yield return null;
        }
    }

    IEnumerator FadeTextIn(float dur)
    {
        Color initColor = yapText.color;
        yapText.outlineColor = Color.white;


        Color fullColor = new Color(initColor.r, initColor.g, initColor.b, 1f);
        Color emptyColor = new Color(initColor.r, initColor.g, initColor.b, 0f);

        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float normI = Mathf.Lerp(0, 1, (i / dur));
            float berpT = BezierBlend(normI);



            Color berpColor = Color.Lerp(emptyColor, fullColor, berpT);
            Color berpOutlineColor = Color.Lerp(Color.white, Color.black, berpT);

            yapText.color = berpColor;
            yapText.outlineColor = berpOutlineColor;
            yield return null;
        }

        yapText.color = fullColor;
        yapText.outlineColor = Color.black;
    }

    IEnumerator FadeTextOut(float dur)
    {
        Color initColor = yapText.color;
        Color fullColor = new Color(initColor.r, initColor.g, initColor.b, 1f);
        Color emptyColor = new Color(initColor.r, initColor.g, initColor.b, 0f);

        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float normI = Mathf.Lerp(0, 1, (i / dur));
            float berpT = BezierBlend(normI);



            Color berpColor = Color.Lerp(fullColor, emptyColor, berpT);

            yapText.color = berpColor;
            yield return null;
        }
    }

    float BezierBlend(float t)
    {
        float bez = t * t * (3.0f - 2.0f * t);

        if (bez <= 0)
        {
            return 0;

        } else if (bez >= 1)
        {
            return 1;

        } else
        {
            return bez;
        }

        return bez;
    }

    IEnumerator ShakeIcon(float duration)
    {

        var icTrans = bossIcon.gameObject.GetComponent<RectTransform>();

        var initialPos = icTrans.anchoredPosition;

        var maxTrembleTime = 1;
        var currentTime = 0.0f;

        float shakeIntensity = 35f;

        while (duration > 0f)
        {
            


            Vector2 move = Random.insideUnitCircle * 10;
            Vector2 dir = move.normalized;

            icTrans.anchoredPosition += (dir / 10f) * shakeIntensity;
            yield return new WaitForSeconds(Time.deltaTime);
            icTrans.anchoredPosition -= (dir / 10f) * shakeIntensity;
            


            yield return null; // wait until next frame
            duration -= Time.deltaTime;
        }

        icTrans.anchoredPosition = initialPos;
    }

    IEnumerator IconFace(int expression, float duration)
    {
        //0 is default
        //1 is hurt
        //2 is enraged
        //3 is smug
        //4 is confused

        Animator[] faceArr = bossIcon.GetComponentsInChildren<Animator>();

        Animator face = faceArr[1];

        if (lives > 0)
        {
            switch (expression)
            {
                case 1:
                    face.CrossFade("hurt", 0, 0);
                    break;
                case 2:
                    face.CrossFade("enraged", 0, 0);
                    break;
                case 3:
                    face.CrossFade("smug", 0, 0);
                    break;
                case 4:
                    face.CrossFade("confused", 0, 0);
                    break;
                default:
                    face.CrossFade("default", 0, 0);
                    break;
            }

            yield return new WaitForSeconds(duration);

            face.CrossFade("default", 0, 0);
        }




    }

    void IconLives()
    {
        Animator carapace = bossIcon.GetComponentInChildren<Animator>();

        carapace.SetInteger("Lives", (lives));


        if (lives < 1)
        {

            Animator[] faceArr = bossIcon.GetComponentsInChildren<Animator>();

            Animator face = faceArr[1];

            face.SetBool("LastLife", true);
        }
    }

    IEnumerator AspectRatioAnimation(float dur, int dir)
    {

        //dir = 1 then animation plays backwards
        //index 0 is top bar, index 1 is bottom bar (like how it looks in inspector, top to bottom order)

        //Coroutine Move(TopBar, initialPos, targetPos, duration)
        //Coroutine Move(ButtonBar, initialPos, targetPos, duration)
        //magic numbers bad, figure out l8r
        Vector3 targetBottomPos = new Vector3(10.9716797f, -288.000031f, 0f);
        Vector3 targetTopPos = new Vector3(35.4469604f, 275f, 0f);
        Vector3 initialBottomPos = new Vector3(10.9716797f, -369.000031f, 0f);
        Vector3 initialTopPos = new Vector3(35.4469604f, 377f, 0f);

        if (dir > 0)
        {
            
            for (float i = 0; i < dur; i += Time.deltaTime)
            {
                aspectRatioBars[0].transform.localPosition = Vector3.Lerp(targetTopPos, initialTopPos, i);
                aspectRatioBars[1].transform.localPosition = Vector3.Lerp(targetBottomPos, initialBottomPos, i);
                yield return null;
            }
            aspectRatioBars[0].transform.localPosition = initialTopPos;
            aspectRatioBars[1].transform.localPosition = initialBottomPos;

        } else
        {
            for (float i = 0; i < dur; i += Time.deltaTime)
            {
                aspectRatioBars[0].transform.localPosition = Vector3.Lerp(initialTopPos, targetTopPos, i);
                aspectRatioBars[1].transform.localPosition = Vector3.Lerp(initialBottomPos, targetBottomPos, i);
                yield return null;
            }

            aspectRatioBars[0].transform.localPosition = targetTopPos;
            aspectRatioBars[1].transform.localPosition = targetBottomPos;

        }
    }
}
