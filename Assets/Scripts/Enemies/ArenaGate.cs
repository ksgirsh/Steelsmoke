using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArenaGate : MonoBehaviour
{
    [SerializeField] List<GameObject> requiredEnemies;
    [SerializeField] List<GameObject> initialEnemies;

    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;

    [SerializeField] LayerMask playerLayer;

    private bool isActivated;

    [SerializeField] Collider2D[] gates;
    [SerializeField] Animator[] anims;

    [SerializeField] int deadEnemies;
    int initListSize;

   // [SerializeField] ParticleSystem[] particles;


    // Start is called before the first frame update
    void Start()
    {
        initialEnemies = requiredEnemies.ToList();



        deadEnemies = 0;
        foreach (Collider2D gate in gates)
        {
            
            gate.enabled = false;
            //Debug.Log("TurnedOff " + gate.enabled + gate.gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActivated)
        {
            PlayerCheck();
        }

        if (deadEnemies >= initialEnemies.Count)
        {
            foreach (Animator anim in anims)
            {
                anim.SetBool("Activated", false);
            }
            

            foreach (Collider2D gate in gates)
            {
                gate.enabled = false;
            }

        } else if (isActivated)
        {

            for (int i = 0; i < (requiredEnemies.Count); i++)
            {

                if (requiredEnemies[i].activeSelf == false)
                {
                    deadEnemies++;
                    requiredEnemies.Remove(requiredEnemies[i]);
                }

            }
        }

    }

    void PlayerCheck()
    {
        Collider2D playerLook = Physics2D.OverlapArea(pointA.position, pointB.position, playerLayer);
        
        // Detects player in area
        if (playerLook != null && playerLook.gameObject.tag == "Player")
        {
            Debug.Log("Actvated" + "   " + playerLook.gameObject.name);
            Activate();
        }
    }

    public void Activate()
    {

       


        isActivated = true;
       

        //anim.SetBool("Activated", true);

        foreach (Collider2D gate in gates)
        {
            gate.enabled = true;
        }

        foreach (Animator anim in anims)
        {
            anim.SetBool("Activated", true);
        }


    }

    public void Deactivate()
    {
        isActivated = false;


        //anim.SetBool("Activated", true);

        foreach (Collider2D gate in gates)
        {
            gate.enabled = false;
        }

        foreach (Animator anim in anims)
        {
            anim.SetBool("Activated", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA.position, pointB.position);
       
    }

    void Reset(Respawn resp)
    {

        deadEnemies = 0;
        requiredEnemies = initialEnemies.ToList();
        isActivated = false;

       

        foreach (Collider2D gate in gates)
        {
            if (gate != null)
            {
                gate.enabled = false;
            }

        }
        foreach (Animator anim in anims)
        {
            anim.SetBool("Activated", false);
        }

    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += Reset;
    }



}
