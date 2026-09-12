using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : EnemyBase
{
    [SerializeField] float flyRange;
    [SerializeField] float flyFreq;

    [SerializeField] float flySpeed;


    [SerializeField] protected AudioClip[] parriedSounds;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
     //   rb.AddForce(transform.right * transform.localScale.x * flySpeed, ForceMode2D.Impulse);
        attacking = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Fly();
    }

    void Fly()
    {
        rb.linearVelocity = new Vector2(-transform.localScale.x * flySpeed * Time.deltaTime * 100f, flyRange * (Mathf.Sin(Time.time * flySpeed)));
    }


    public override void TakeDamage(float damage, Vector2 knockback)
    {
        if (invincible != true)
        {
            SoundFXManager.instance.PlayRandomProximitySoundEffectClip(damageSounds, gameObject, enemyVol);
            currentHealth -= damage;

            StartCoroutine("Invincibility");
        }


        if (currentHealth < 0 || currentHealth == 0)
        {
            dead = true;
            int eff = Random.Range(9, 10);
            StartCoroutine("Death");
        }

        return;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Parrier" && attacking == true && Mathf.Sign(transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x))
        {
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
                Shooting shoot = player.GetComponent<Shooting>();


                SoundFXManager.instance.PlayRandomProximitySoundEffectClip(parriedSounds, gameObject, enemyVol);


                if ((currentHealth - shoot.attackDamage * 2) <= 0)
                {
                    dead = true;
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

    }
}
