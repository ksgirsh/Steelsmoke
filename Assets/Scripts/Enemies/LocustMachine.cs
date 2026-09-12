using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocustMachine : EnemyBase
{
    [SerializeField] GameObject locust;
    Camera mc;

    [SerializeField] float spawnCooldown;
    [SerializeField] float aggroRange;

    bool spawned = false;

    private Animator anim;

    [SerializeField] float shrinkF;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        mc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        


        Vector2 camDim = new Vector2(mc.pixelWidth, mc.pixelHeight);


        float aspect = (camDim.x / camDim.y);
        float wHeight = mc.orthographicSize * 2;
        float wWidth = wHeight * aspect;


        Vector2 wDim = new Vector2(wWidth, wHeight);

        Vector2 point = player.transform.position + (Vector3)(wDim / 2);

        anim = gameObject.GetComponent<Animator>();
       // Debug.Log(mc.ScreenToWorldPoint(camDim * aspect));
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


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            
            Debug.Log(CalcSpawnPos());
        }

        if (isAggro)
        {
            StartCoroutine(Spawn());
        } else
        {
            Look();
        }
    }


    void Look()
    {
        Vector2 dist = transform.position - player.transform.position;
        if (dist.magnitude <= aggroRange)
        {
            isAggro = true;
            anim.CrossFade("LCMActivated", 0, 0);
        }
    }

    IEnumerator Spawn()
    {
        if (spawned == false)
        {
            spawned = true;
            Instantiate(locust, CalcSpawnPos(), Quaternion.identity);
            yield return new WaitForSeconds(spawnCooldown);
            spawned = false;
        }
        
    }


    Vector2 CalcSpawnPos()
    {
        Vector2 camDim = new Vector2(mc.pixelWidth, mc.pixelHeight);


        float aspect = (camDim.x / camDim.y);
        float wHeight = mc.orthographicSize * 2;
        float wWidth = wHeight * aspect;

        //Debug.Log("WIDTH: " + wWidth + "  HEIGHT: " + wHeight);

        Vector2 wDim = new Vector2(wWidth, wHeight);

        Vector2 point = player.transform.position + (Vector3)(wDim / 2);
       // Debug.Log(point);

        Vector2 spawnPos = new Vector2((point.x + Random.Range(2, 8)), Random.Range(point.y * shrinkF, (point.y - wHeight) * shrinkF));
        return spawnPos;
        //x will be point.x + some margin
        //y will be random between point.y * shrinkF and (point.y - wHeight) * shrinkF
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
