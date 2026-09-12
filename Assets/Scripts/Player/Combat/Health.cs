using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Health : MonoBehaviour
{
    [field: SerializeField] public int setHealth;
    [Range(0.0f, 5.0f)]
    public int CurrentInspectorHealth;

    public int currentHealth;
    public bool invincible { get; private set; }
    [SerializeField] float invincibilityTime;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] SpriteRenderer rend;

    [SerializeField] Material invincibleMaterial;
    [SerializeField] Material normalMaterial;

    [SerializeField] Respawn respawn;
    [SerializeField] Ultimate ult;

    [SerializeField] GameObject healthEffect;

    [SerializeField] Shooting shoot;
    [SerializeField] Inspect insp;
    [SerializeField] List<Transform> points;
    [SerializeField] Transform pointParent;

    [SerializeField] Image img;

    [SerializeField] float healthOffset;
    [SerializeField] GameObject heart;

    [SerializeField] GameObject[] disableObjectsOnDeath;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] RunAndJump rj;

    [SerializeField] AudioClip[] hurt;
    [SerializeField] AudioClip[] death;
    [SerializeField] AudioClip[] heal;
    [SerializeField] AudioClip healthUp;

    [SerializeField] MainMedallion mainMedal;

    [SerializeField] TextMeshProUGUI healthTxt;
    [SerializeField] Image healthRing;
    [SerializeField] Color minHealthColor;
    [SerializeField] Color maxHealthColor;

    [SerializeField] Color overhealRingColor;

    [SerializeField] GameObject totalFX;
    private FadeIn fx1;
    [SerializeField] Color normalmaxHColor;
    // Start is called before the first frame update
    void Start()
    {
        fx1 = gameObject.GetComponent<FadeIn>();
        currentHealth = setHealth;
        CurrentInspectorHealth = currentHealth;

        insp = gameObject.GetComponent<Inspect>();
        deathParticles.gameObject.SetActive(false);

        healthTxt.text = currentHealth.ToString();
        //  AddHeart();
        HealthUI();
        
    }


    void HealthUI()
    {
        
        float hRatio = ((currentHealth * 1f) / (setHealth * 1f));
        healthRing.material.SetFloat("_Value", hRatio);

        if (currentHealth <= setHealth)
        {
            healthTxt.faceColor = Color.Lerp(minHealthColor, maxHealthColor, hRatio);
            healthRing.material.SetColor("_Color2", normalmaxHColor);

        } else
        {
            healthTxt.faceColor = ult.ultFuelColor;
            healthRing.material.SetColor("_Color2", overhealRingColor);
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        

        if (currentHealth < 1 && insp.choicePresented == false)
        {
            SoundFXManager.instance.PlayRandomSoundEffectClip(death, transform.position, 1f);
            deathParticles.gameObject.SetActive(true);
            deathParticles.Emit(8);

            foreach (GameObject obj in disableObjectsOnDeath)
            {
                obj.SetActive(false);
            }
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
            foreach (Collider2D coll in colliders)
            {
                coll.enabled = false;
            }


            insp.ShowRespawnText();

        }

        if (invincible)
        {
            rend.material = invincibleMaterial;
            rend.color = new Color(1, 1, 1, 1);
        } else
        {
            rend.material = normalMaterial;
        }

        /*
        if (rj.fuel > 0.9 && Input.GetKeyDown(KeyCode.Q) && currentHealth < setHealth)
        {
            //ReplenishHealth(1);
            rj.AddFuel(-rj.fuel);
        }*/
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (!shoot.ultAttAnim)
        {
            if (coll.collider.gameObject.tag == "Enemy")
            {

               
                StartCoroutine((TakePDamage(coll.gameObject.GetComponent<EnemyBase>().contactDamage, coll.gameObject.GetComponent<Transform>().right)));

            }
            
            if (coll.collider.gameObject.GetComponent<Boss>() != null)
            {
                //uh oh. Sentinel Time!
                Boss boss = coll.collider.gameObject.GetComponent<Boss>();
                //parry dash
      
                if (boss.attackID == 1 || boss.attackID == 8)
                {
                    if (shoot.isParrying == false || (Mathf.Sign(transform.localScale.x) == Mathf.Sign(boss.GetComponent<Rigidbody2D>().linearVelocity.x)))
                    {
                        StartCoroutine((TakePDamage(coll.gameObject.GetComponent<EnemyBase>().contactDamage, coll.gameObject.GetComponent<Transform>().right)));
                    }
                }

                //contact damage; not attacking
                if (boss.attackID == 0 || boss.attackID == 5 || boss.attackID == 6 || boss.attackID == 7)
                {
                    StartCoroutine((TakePDamage(coll.gameObject.GetComponent<EnemyBase>().contactDamage, coll.gameObject.GetComponent<Transform>().right)));
                }

            }




            if (coll.collider.gameObject.tag == "EnemyParriable" && coll.collider.gameObject.GetComponent<Boss>() == null)
            {

                if (shoot.isParrying == false)
                {
                    StartCoroutine((TakePDamage(coll.gameObject.GetComponent<EnemyBase>().contactDamage, coll.gameObject.GetComponent<Transform>().right)));

                } else if (Mathf.Sign(transform.localScale.x) == Mathf.Sign(coll.gameObject.GetComponent<Rigidbody2D>().linearVelocity.x))
                {
                    
                    StartCoroutine((TakePDamage(coll.gameObject.GetComponent<EnemyBase>().contactDamage, coll.gameObject.GetComponent<Transform>().right)));
                }


            }

            if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "EnemyParriable")
            {
                StartCoroutine(rj.KnockbackEffect(coll.gameObject.transform.position, 1));

            }

        }

        if (coll.collider.gameObject.tag == "Mattress")
        {


            Vector2 d = transform.position - coll.collider.gameObject.transform.position;
            Vector2 knockV = new Vector2(d.x, (d.y * 0.25f));


            StartCoroutine(rj.ForceEffect((knockV.normalized), 1f));

            coll.gameObject.GetComponent<Rigidbody2D>().AddForce(coll.gameObject.transform.right * 5f * -coll.gameObject.transform.localScale.x, ForceMode2D.Impulse);
            

        }

    }


    public IEnumerator TakePDamage(int damageP, Vector2 knockback)
    {
        if (damageP > 0)
        {
            SoundFXManager.instance.PlayRandomSoundEffectClip(hurt, transform.position, 1f);

        } else
        {
            SoundFXManager.instance.PlayRandomSoundEffectClip(heal, transform.position, 1f);
        }

        //Debug.Log("took damage");
        int damage;

        if ((currentHealth - damageP) < 0)
        {
            damage = Mathf.Abs((currentHealth - damageP));
        } else
        {
            damage = damageP;
        }


        if (invincible == false)
        {

            // HealthEffects(damage);
            currentHealth -= damage;
            StartCoroutine(mainMedal.Shake(0.45f, (10f * damageP)));


        }

        healthTxt.text = currentHealth.ToString();
        HealthUI();


        StartCoroutine(fx1.PulseFadeSpr(0.2f, totalFX));

        invincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;

        

    }


    IEnumerator OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Health" && currentHealth < setHealth)
        {
            //Debug.Log("Picked up Health");
            ReplenishHealth(coll.gameObject.GetComponent<HealthItem>().healthGive);
            coll.gameObject.SetActive(false);
            invincible = true;
            yield return new WaitForSeconds(invincibilityTime / 2);
            invincible = false;
        }

        if (coll.gameObject.tag == "Checkpoint")
        {
            SaveValues();
        }

        //fix later, this sucks
        if (coll.gameObject.tag == "EnemyParriable")
        {
            if (shoot.isParrying == false || (Mathf.Sign(transform.localScale.x) == Mathf.Sign(coll.transform.gameObject.GetComponent<Rigidbody2D>().linearVelocity.x)))
            {
                if (coll.gameObject.GetComponent<Locust>() != null)
                {
                    StartCoroutine((TakePDamage(coll.gameObject.GetComponent<EnemyBase>().contactDamage, coll.gameObject.GetComponent<Transform>().right)));
                }

            }


        }

    }

    void HealthEffects(int amt)
    {


        for (int i = 0; i < amt; i++)
        {
            int u = (currentHealth - amt) + i;

            int final = (setHealth - u) - 1;
         //   Debug.Log(final);

            if (final >= 0)
            {
                if (final <= points.Count)
                {

                    GameObject heart = points[final].gameObject;
                    heart.GetComponent<Animator>().SetBool("Broken", true);
                    //Debug.Log(heart.name);

                    GameObject fx = Instantiate(healthEffect, points[final].position, Quaternion.identity, pointParent);
                    Destroy(fx, 1f);

                }
            }
            
           
           // Debug.Log("Curr:" + currentHealth + "  Final:" + final + " AmtLost:" + amtLost + " Set:" + setHealth);
     
        }

    }

   public void ReplenishHealth(int amt)
   {
        SoundFXManager.instance.PlayRandomSoundEffectClip(heal, transform.position, 1f);



        /*
        for (int i = 0; i < amt; i++)
        {
            int u = (currentHealth + (amt - i));

            int final = (setHealth - u);
           // Debug.Log(final + " //// " + i) ;

            if (final >= 0)
            {
                if (final <= setHealth)
                {

                    GameObject heart = points[final].gameObject;
                    heart.GetComponent<Animator>().SetBool("Broken", false);
                    //Debug.Log(heart.name);

                    //GameObject fx = Instantiate(healthEffect, points[final].position, Quaternion.identity, pointParent);
                  //  Destroy(fx, 1f);

                }
            }


            // Debug.Log("Curr:" + currentHealth + "  Final:" + final + " AmtLost:" + amtLost + " Set:" + setHealth);

        }*/

        currentHealth += amt;
        healthTxt.text = currentHealth.ToString();
        HealthUI();

    }


    public void AddHeart()
    {
        /*
        SoundFXManager.instance.PlaySoundEffectClip(healthUp, transform.position, 1f);
        GameObject heartObj = Instantiate(heart, pointParent);
        
        Vector3 heartPos = new Vector3(setHealth * healthOffset, 0f, 0f);
        heartObj.transform.localPosition = heartPos;

        points.Insert(0, heartObj.transform);
        */

        setHealth++;
        ReplenishHealth(1);

        /*
        if (currentHealth < setHealth)
        {

            heartObj.GetComponent<Animator>().SetBool("Broken", true);
        }*/
      
    }

    void RemoveHeart()
    {
        /*
        GameObject heartObj = points[0].gameObject;
 
        Destroy(heartObj);
        points.Remove(points[0]);*/
        setHealth--;

        HealthUI();
        //  currentHealth--;
    }


    public IEnumerator FadeInOut(Color start, Color end, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            img.color = Color.Lerp(start, end, normalizedTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            img.color = Color.Lerp(end, start, normalizedTime);
            yield return null;
        }

    }

    void SaveValues()
    {
        respawn.savedValues[0] = currentHealth;
        respawn.savedValues[3] = setHealth;
    }

    void LoadValues(Respawn resp)
    {

       int healthRange = Mathf.Abs(setHealth - (int)(respawn.savedValues[3]));

        //Reset max health
        if (setHealth > respawn.savedValues[3])
        {
            for (int i = 0; i < healthRange; i++)
            {
                RemoveHeart();
            }
        }

        // This shouldnt really. like ever happen. But just in case ;)
        if (setHealth < respawn.savedValues[3])
        {
            for (int i = 0; i < healthRange; i++)
            {
                AddHeart();
            }
        }

        if ((int)respawn.savedValues[0] > currentHealth)
        {
            ReplenishHealth(((int)respawn.savedValues[0] - currentHealth));

        }
        else if ((int)respawn.savedValues[0] < currentHealth)
        {
            HealthEffects((currentHealth - (int)respawn.savedValues[0]));

            currentHealth = (int)respawn.savedValues[0];
        }






        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        foreach (GameObject obj in disableObjectsOnDeath)
        {
            obj.SetActive(true);
        }

        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D coll in colliders)
        {
            coll.enabled = true;
        }

        deathParticles.gameObject.SetActive(false);



        
    }


    void OnEnable()
    {
        Respawn.OnRespawnEvent += LoadValues;
    }
    
}


