using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Health heal;
    [SerializeField] Animator anim;

    [SerializeField] GameObject healthLoss;

    [SerializeField] Transform[] points;
    [SerializeField] Transform pointParent;

    // Start is called before the first frame update
    void Start()
    {
        anim.CrossFade("5 Health", 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        int healthbarHealth = heal.currentHealth;
        anim.SetInteger("Health", heal.currentHealth);
        //StartCoroutine("AnimationControl");
        /*switch (healthbarHealth)
        {
            case 5:

                GameObject healthFX5 = Instantiate(healthLoss, points[5 - healthbarHealth].position, Quaternion.identity, pointParent);
                Destroy(healthFX5, 0.5f);
                break;
            case 4:
                GameObject healthFX4 = Instantiate(healthLoss, points[5 - healthbarHealth].position, Quaternion.identity, pointParent);
                Destroy(healthFX4, 0.5f);
                break;
            case 3:

                GameObject healthFX3 = Instantiate(healthLoss, points[5 - healthbarHealth].position, Quaternion.identity, pointParent);
                Destroy(healthFX3, 0.5f);
                break;
            case 2:

                GameObject healthFX2 = Instantiate(healthLoss, points[5 - healthbarHealth].position, Quaternion.identity, pointParent);
                Destroy(healthFX2, 0.5f);
                break;
            case 1:

                GameObject healthFX1 = Instantiate(healthLoss, points[5 - healthbarHealth].position, Quaternion.identity, pointParent);
                Destroy(healthFX1, 0.5f);
                break;
            case 0:

                GameObject healthFX = Instantiate(healthLoss, points[5 - healthbarHealth].position, Quaternion.identity, pointParent);
                Destroy(healthFX, 0.5f);
                break;
            default:
                if (healthbarHealth > 5)
                {
                    anim.CrossFade("5 Health", 0, 0);
                    break;
                }
                else
                {
                    anim.CrossFade("0 Health", 0, 0);
                    break;
                }

        }*/

    }

    IEnumerator AnimationControl()
    {
        int healthbarHealth = heal.currentHealth;

        anim.SetInteger("Health", heal.currentHealth);

        //changes health bar sprite depending on the currentHealth;
        switch (healthbarHealth)
        {
            case 5:
                anim.CrossFade("5 Health", 0, 0);
                break;
            case 4:
                anim.CrossFade("5to4", 0, 0);
                
                break;
            case 3:
                anim.CrossFade("4to3", 0, 0);
                
                break;
            case 2:
                anim.CrossFade("3to2", 0, 0);
                yield return new WaitForSeconds(1f);
                anim.CrossFade("2 Health", 0, 0);
                break;
            case 1:
                anim.CrossFade("2to1", 0, 0);
                yield return new WaitForSeconds(1f);
                anim.CrossFade("1 Health", 0, 0);
                break;
            case 0:
                anim.CrossFade("1to0", 0, 0);
                yield return new WaitForSeconds(1f);
                anim.CrossFade("0 Health", 0, 0);
                break;
            default:
                if (healthbarHealth > 5)
                {
                    anim.CrossFade("5 Health", 0, 0);
                    break;
                }
                else
                {
                    anim.CrossFade("0 Health", 0, 0);
                    break;
                }

        }
    }

}


