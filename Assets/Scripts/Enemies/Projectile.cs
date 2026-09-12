using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [field:SerializeField] public int contactDamage { get; private set; }
    private Rigidbody2D rb;
    public bool parried { get; private set; }

    [SerializeField] bool parriable = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        parried = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Shooting shoot = coll.gameObject.GetComponent<Shooting>();
            Health heal = coll.gameObject.GetComponent<Health>();


            if (shoot != null && heal != null)
            {

                if (shoot.isParrying == true && (Mathf.Sign(coll.gameObject.transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x)) && parried == false && parriable == true)
                {
                    parried = true;
                    shoot.StartCoroutine(shoot.ParrySuccess());
                    rb.linearVelocity = -rb.linearVelocity;
                    return;

                }
                else if (parried == false)
                {
                    heal.StartCoroutine(heal.TakePDamage(contactDamage, transform.right));
                    return;
                }
            }

        }

        if (coll.gameObject.tag == "EnemyParriable" || coll.gameObject.tag == "Enemy")
        {
            if (parriable == true)
            {
                Boss boss = coll.gameObject.GetComponent<Boss>();

                if (boss != null)
                {
                    if (parried == true)
                    {
                        boss.GuaranteedDamage(contactDamage, rb.linearVelocity);
                    }


                } else
                {
                    EnemyBase enem = coll.gameObject.GetComponent<EnemyBase>();
                    if (parried == true)
                    {
                        enem.TakeDamage(contactDamage, rb.linearVelocity);
                    }
                }

                
                

            }


        }
    }
}
