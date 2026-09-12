using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wilson : EnemyBase
{

    [SerializeField] float setSpeed = 1;
    float currentSpeed;
    [SerializeField] float deltaTimeK = 100;

    [SerializeField] float lerpK = 100;
    float speedLerpTime;

    [SerializeField] float turnDelay = 0.5f;
    [SerializeField] float aggroRange = 4f;

    [SerializeField] float wanderTimer = 4f;
    [SerializeField] LayerMask ground;

    private Vector3 targetPosition;
    private float timer;

    public float movementSpeed = 2f;
    public float wanderRadius = 5f; // Radius of the wander area
    public bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (isAggro)
        {
            Chase();

        } else
        {
            Look();

        }


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

    }

    void Look()
    {
        Vector2 dist = transform.position - player.transform.position;
        if (dist.magnitude <= 4f)
        {
            isAggro = true;
        }
    }

    void Chase()
    {
        Vector2 dist = (player.transform.position - this.transform.position).normalized;

        float lerpSpeed = Mathf.Lerp(currentSpeed, setSpeed, ((speedLerpTime)/(lerpK)));
        
        if (dist.x * transform.localScale.x > 0)
        {
                    rb.linearVelocity = new Vector2((dist.x * lerpSpeed), rb.linearVelocity.y);
        }


        speedLerpTime += Time.deltaTime;

        if (Mathf.Abs(dist.x) < 0.3)
        {
            speedLerpTime = 0;

            if (dist.x > 0)
            {
                StartCoroutine(TurnFlip(true));
            }
            else
            {
                StartCoroutine(TurnFlip(false));
            }
        }
    }

    IEnumerator TurnFlip(bool right)
    {
        yield return new WaitForSeconds(turnDelay);
        currentSpeed = 0;

        if (right)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
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
        RaycastHit2D lookforWalls = Physics2D.Raycast(transform.position, transform.right * wanderRadius * transform.localScale.x, aggroRange, ground);
        if (lookforWalls.collider != null)
        {
            Vector2 offset = new Vector2(lookforWalls.distance, 0);
            //Debug.Log(offset);
            if (lookforWalls.distance < 1)
            {
                //Debug.Log(lookforWalls.distance + gameObject.name);
                Flip();
                lookforWalls = Physics2D.Raycast(transform.position, transform.right * wanderRadius, aggroRange, ground);
                targetPosition = (Vector2)transform.position - offset + Random.insideUnitCircle.normalized * lookforWalls.distance;
            }
            else
            {
                targetPosition = (Vector2)transform.position + offset + Random.insideUnitCircle.normalized * lookforWalls.distance;
            }
        }
        else
        {
            Collider2D[] groundCheck = Physics2D.OverlapCircleAll(transform.right * wanderRadius * transform.localScale.x, 1f, ground);


            if (groundCheck != null)
            {
               
                targetPosition = (Vector2)transform.position + Random.insideUnitCircle.normalized * wanderRadius;

            } else
            {
                return;
            }

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

       // anim.CrossFade("Walking", 0, 0);


        transform.Translate(distance.normalized * movementSpeed * Time.deltaTime, Space.World);



    }
    public override void TakeDamage(float damage, Vector2 knockback)
    {
        if (invincible != true)
        {
            // PlaySoundEffect(enemySFX[eff], false, true, true);
            //SoundFXManager.instance.PlayRandomProximitySoundEffectClip(damageSounds, gameObject, enemyVol);
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

            StartCoroutine("Death");
     
        }

        //  Debug.Log("took damage");
        return;
    }


}
