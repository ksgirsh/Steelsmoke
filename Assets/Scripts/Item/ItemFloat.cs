using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloat : MonoBehaviour
{
    [SerializeField] float floatIntensity;
    [SerializeField] float floatSpeed;
    [SerializeField] Rigidbody2D rb;

    public bool dropped;


    //GasCan
    [SerializeField] Animator anim;
    [SerializeField] float gasCooldown;
    [field:SerializeField] public bool usable { get; private set; }

    // Update is called once per frame
    void Start()
    {
        if (this.gameObject.tag == "GasCan")
        {
            usable = true;
        }
    }
    void Update()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, floatIntensity * (Mathf.Sin(Time.time * floatSpeed)));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {



        

        if (coll.gameObject.GetComponent<RunAndJump>() != null)
        {
            RunAndJump rj = coll.gameObject.GetComponent<RunAndJump>();
            if (this.tag == "GasCan")
            {
                if (anim != null && usable == true && rj.fuel < 1)
                {
                    
                    StartCoroutine("Recharge");
                    rj.AddFuel(1f);
                }
               

            } else if (this.gameObject.tag == "MedallionPickup")
            {
              //  Destroy(gameObject);
            }
            
        }
    }

    IEnumerator Recharge()
    {
      
        usable = false;
        anim.CrossFade("GasUsed", 0, 0);
        yield return new WaitForSeconds(gasCooldown);
        anim.CrossFade("GasUsable", 0, 0);
        usable = true;
    }

    void CleanUp(Respawn resp)
    {
        if (dropped && this != null)
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        Respawn.OnRespawnEvent += CleanUp;
    }

}
