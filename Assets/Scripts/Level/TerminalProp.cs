using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalProp : ProxAnim
{
    [SerializeField] GameObject light;
    [SerializeField] bool isC = false;
    [SerializeField] bool cRandom = true;
    [SerializeField] int cOnSpr = 3;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        proxState = 500;
    }

    // Update is called once per frame
    void Update()
    {
        DistCheck();
        if (proxState == 0)
        {
            light.SetActive(true);
        }
        else
        {
            light.SetActive(false);
        }
    }

    protected override void Change()
    {
        int termState = 0;
        //FOR TERMSTATE:
        //1-2 is on
        //3-4 is cOn
        //0 is off

        if (isC && proxState == 0)
        {
            if (cRandom == true)
            {
                termState = Random.Range(3, 5);
                anim.CrossFade(animationStates[termState], 0, 0);

            } else if (cRandom == false)
            {
                anim.CrossFade(animationStates[cOnSpr], 0, 0);
            }


        } else if (proxState == 0)
        {
            termState = Random.Range(1, 3);
            anim.CrossFade(animationStates[termState], 0, 0);

        } else
        {
            anim.CrossFade(animationStates[0], 0, 0);
        }
       
        
    }

    protected virtual void DistCheck()
    {
        Vector2 dist = transform.position - playerT.position;

        if (dist.magnitude <= distTriggers[0] && proxState != 0)
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
                    if (dist.magnitude <= distTriggers[i] && dist.magnitude > distTriggers[(i - 1)] && proxState != i)
                    {
                        proxState = i;
                        Change();
                    }

                }
                else if (i == (distTriggers.Length - 1))
                {
                    if (dist.magnitude > distTriggers[(i - 1)] && proxState != i)
                    {
                        proxState = i;
                        Change();
                    }
                }


            }
        }


    }
}
