using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraProp : ProxAnim
{
    float playerY;

    [SerializeField] GameObject[] lights;
    [SerializeField] int currentLight;

    [SerializeField] Color farC;
    [SerializeField] Color closeC;
    [SerializeField] float colorThreshold;

    [SerializeField] Transform target;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (target == null)
        {
            target = playerT.transform;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        playerY = target.position.y;
        float pX = target.position.x;
        //currentLight is negative, then camera is broken
        if (pX * transform.localScale.x < transform.position.x * transform.localScale.x && currentLight != -1)
        {
            if (playerY < transform.position.y + 1f)
            {
                DistCheck();

                switch (proxState)
                {
                    case 0:
                        lights[0].SetActive(true);
                        lights[1].SetActive(false);
                        lights[2].SetActive(false);
                        currentLight = 0;
                        break;

                    case 1:
                        lights[0].SetActive(false);
                        lights[1].SetActive(true);
                        lights[2].SetActive(false);
                        currentLight = 1;
                        break;

                }

            }
            else
            {
                anim.CrossFade("cameraUp", 0, 0);

                lights[0].SetActive(false);
                lights[1].SetActive(false);
                lights[2].SetActive(true);
                currentLight = 2;
            }

            ColorCheck();

        } else if (currentLight != -1)
        {
            anim.CrossFade("cameraClose", 0, 0);

            lights[0].SetActive(true);
            lights[1].SetActive(false);
            lights[2].SetActive(false);
            currentLight = 0;
            lights[0].GetComponent<LightControl>().SetColor(farC);
        }

        

    }

    void ColorCheck()
    {
        float distM = Mathf.Abs(target.position.x - transform.position.x);

        if (distM < colorThreshold)
        {
            lights[currentLight].GetComponent<LightControl>().SetColor(closeC);

        } else
        {
            lights[currentLight].GetComponent<LightControl>().SetColor(farC);
        }
    }

    public void Break()
    {
        lights[0].SetActive(false);
        lights[1].SetActive(false);
        lights[2].SetActive(false);
        currentLight = -1;

        anim.CrossFade("cameraBreak", 0, 0);
    }
}
