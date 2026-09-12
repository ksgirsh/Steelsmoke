using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballControl : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb; 
    [field:SerializeField] public bool parried { get; private set; }

    [SerializeField] float explodeRadius;
    [SerializeField] public int damage;

    [SerializeField] LayerMask enemyLayers;
    [SerializeField] LayerMask playerLayers;

    [SerializeField] SpriteRenderer rend;
    [SerializeField] GameObject exshplode;

    [SerializeField] float rotationSpeed; 
    private Vector3 rotationAxis = Vector3.forward;

    [SerializeField] AudioClip[] parriedSounds;

    // Start is called before the first frame update
    void Start()
    {
        
        SpriteRenderer rend = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        if (parried == true)
        {
            rend.color = Color.blue;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
  
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Shooting>() != null)
            {
                Shooting shoot = other.gameObject.GetComponent<Shooting>();

                if (shoot.isParrying == true && Mathf.Sign(other.gameObject.transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x))
                {
                  
                    parried = true;
                    SoundFXManager.instance.PlayRandomSoundEffectClip(parriedSounds, transform.position, 1f);
                    shoot.StartCoroutine(shoot.ParrySuccess());
                    rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
                    rotationSpeed *= -1;

                } else if (!parried) {

                    ExplodePlayer();
                    GameObject boom = Instantiate(exshplode, other.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    Destroy(boom, 2f);
                }
               


            }
                    
        }

        if (other.gameObject.tag == "Parrier" && Mathf.Sign(transform.localScale.x) != Mathf.Sign(rb.linearVelocity.x) && !parried)
        {
            Shooting shoot = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooting>();
            SoundFXManager.instance.PlayRandomSoundEffectClip(parriedSounds, transform.position, 1f);
            parried = true;
            shoot.StartCoroutine(shoot.ParrySuccess());
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
            rotationSpeed *= -1;


        }

        if (other.gameObject.tag == "Enemy")
        {
          
            if (parried == true)
            {
                GameObject boom = Instantiate(exshplode, other.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                Destroy(boom, 2f);
            }
        }
    }

    void ExplodePlayer()
    {
        
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, explodeRadius, playerLayers);
        foreach (Collider2D player in hitPlayers)
        {
            if (player.gameObject.GetComponent<Health>() != null)
            {
                player.gameObject.GetComponent<Health>().StartCoroutine(player.gameObject.GetComponent<Health>().TakePDamage(damage, transform.right));
            }
              
      
        }
        
    }

    void OnRespawn(Respawn resp)
    {
        if (this != null)
        {
            Destroy(gameObject);
        }

    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += OnRespawn;
    }


}
