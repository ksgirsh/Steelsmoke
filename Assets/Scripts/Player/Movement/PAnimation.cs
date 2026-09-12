 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAnimation : MonoBehaviour
{
    [SerializeField] RunAndJump rj;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] Shooting shoot;
    [SerializeField] Inspect insp;
    [SerializeField] Ultimate ulti;

    SpriteRenderer rend;

    // this is the most fucvked up,, evil script ever. BEWARE! abandon all hope ye who enter here this might be the worst code ever written in the history of ever. 




    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        //Running Animation
        if (rj.grabbed)
        {
            rend.color = Color.blue;
        } else
        {
            rend.color = Color.white;
        }

        if (!rj.enabled && !shoot.enabled && insp.inspecting && !rj.sitting && !rj.riding && !rj.stunned)
        {
            if (!ulti.ultimateState)
            {
                anim.CrossFade("Inspect", 0, 0);
            }
            else if (shoot.ultAttAnim == false)
            {
                anim.CrossFade("TInspect", 0, 0);
            }
         
        }

        
        if (shoot.parrySuccess && !rj.fireBoost && !ulti.ultimateState && !rj.sitting && !rj.riding && !rj.stunned)
        {
            
              anim.CrossFade("ParrySuccess", 0, 0);
            
          
        }
        if (shoot.isParrying && !shoot.parrySuccess && rb.linearVelocity.y < 0.1 && rb.linearVelocity.y > -0.1 && !rj.fireBoost)
        {
            if (!ulti.ultimateState)
            {
                anim.CrossFade("ParryA", 0, 0);
            } else
            {
                anim.CrossFade("TPlayerParry", 0, 0);
            }
            
        }


        if (rj.enabled)
        {
            if (rj.riding)
            {
                if (rj.moving == 0)
                {
                    anim.CrossFade("ZipNormal", 0, 0);
                }
            }


            if (rj.moving != 0 && rb.linearVelocity.y == 0 && !Input.GetKeyDown(KeyCode.LeftShift) && shoot.isAttacking == false && !shoot.isParrying && !shoot.parrySuccess && !rj.fireBoost && !rj.sitting && !rj.riding && !rj.stunned)
            {
                
                if (!ulti.ultimateState)
                {
                    anim.CrossFade("Running", 0, 0);
                }
                else if (shoot.ultAttAnim == false)
                {
                    anim.CrossFade("TPlayerRunning", 0, 0);
                }
            }

            if (rj.wallSliding && shoot.isParrying == false && !shoot.parrySuccess && !rj.fireBoost && !rj.sitting && !rj.riding && !rj.stunned)
            {
                if (!ulti.ultimateState)
                {
                    anim.CrossFade("WallSlide", 0, 0);
                }
                else if (shoot.ultAttAnim == false)
                {
                    anim.CrossFade("TWallSlide", 0, 0);
                }
                
            }

        }




        //Jumping 

        //Falling
        if (rb.linearVelocity.y != 0 && rb.linearVelocity.y < -0.7 && !Input.GetKey(KeyCode.LeftShift) && shoot.isAttacking == false && rj.wallSliding == false && !shoot.parrySuccess && !rj.fireBoost && !rj.sitting && !rj.riding && !rj.stunned)
         {
            if (!ulti.ultimateState)
            {
                anim.CrossFade("Falling", 0, 0);
            }
            else if (shoot.ultAttAnim == false)
            {
                anim.CrossFade("TFalling", 0, 0);
            }

         }
        

        if (rb.linearVelocity.y > 0.7 && !Input.GetKeyDown(KeyCode.LeftShift) && shoot.isAttacking == false && rj.grounded == false && rj.wallSliding == false && !shoot.parrySuccess && shoot.isParrying == false && !rj.fireBoost && !rj.sitting && !rj.riding && !rj.stunned) 
        {
            if (!ulti.ultimateState)
            {
                anim.CrossFade("Jumping", 0, 0);
            }
            else if (shoot.ultAttAnim == false)
            {
                anim.CrossFade("TJumping", 0, 0);
            }
            

        }

        //Attacking
        if (shoot.isAttacking && shoot.isParrying == false && !shoot.parrySuccess && !rj.fireBoost && !rj.wallSliding && !rj.sitting && !rj.riding && !rj.stunned)
        {
            //attack animation logic handled in attack script -- AnimationCheck()


        }

        if (rj.enabled && shoot.enabled)
        {
            if (shoot.isAttacking == false && rj.grounded == true && rj.moving == 0 && shoot.isParrying == false && !shoot.parrySuccess && !rj.fireBoost && rb.linearVelocity.y > -0.1 && !rj.sitting && !rj.riding && !rj.stunned)
            {
                if (!ulti.ultimateState)
                {
                    anim.CrossFade("Idle", 0, 0);
                } else if (shoot.ultAttAnim == false)
                {
                    anim.CrossFade("TIdle", 0, 0);
                    
                }
         
            }
        }
        //Idle
        

    }
}
