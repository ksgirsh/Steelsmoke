using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxAnim : AnimController
{
    [SerializeField] protected Transform playerT;

    [SerializeField] protected int proxState;
    [SerializeField] protected string[] animationStates;

    [SerializeField] protected float[] distTriggers;

    // Update is called once per frame

    protected override void Start()
    {
        base.Start();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerT = player.transform;


    }

    protected virtual void Update()
    {
        DistCheck();
        
    }

    protected virtual void DistCheck()
    {
        Vector2 dist = transform.position - playerT.position;

        if (dist.magnitude <= distTriggers[0])
        {
            proxState = 0;
            Change();
        }
        else
        {
            for (int i = 0; i < distTriggers.Length; i++)
            {
                // 0 is closest, the higher i is, the farther you should be

                if (i != 0 && i != (distTriggers.Length - 1))
                {
                    if (dist.magnitude <= distTriggers[i] && dist.magnitude > distTriggers[(i - 1)])
                    {
                        proxState = i;
                        Change();
                    }

                }
                else if (i == (distTriggers.Length - 1))
                {
                    if (dist.magnitude > distTriggers[(i - 1)])
                    {
                        proxState = i;
                        Change();
                    }
                }


            }
        }


    }

    protected virtual void Change()
    {
        anim.CrossFade(animationStates[proxState], 0, 0);
    }
}
