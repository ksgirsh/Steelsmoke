using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCard : MonoBehaviour
{
    [SerializeField] Animator anim;

    [SerializeField] GameObject[] fadeObjs;

    [SerializeField] string Area = "Wind Fortress";
    [SerializeField] bool triggered = false;

   // [SerializeField] OpacityControl opac;
    [SerializeField] float duration;


    [SerializeField] GameObject music;
    [SerializeField] GameObject ambience;




    // Start is called before the first frame update
    void Start()
    {

        music.GetComponent<AudioSource>().Stop();
        ambience.GetComponent<AudioSource>().volume = 1f;
        //Make all title card objects transparent
        foreach (GameObject obj in fadeObjs)
        {
            if (obj.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();

                rend.color = new Color(1, 1, 1, 0);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && !triggered)
        {
            StartCoroutine("Appear");
        }
    }

    IEnumerator Appear()
    {
       // titleCard.SetActive(true);
        triggered = true;
        Debug.Log("Activated Title Card");
        music.GetComponent<AudioSource>().Play();

        music.GetComponent<AudioSource>().volume = 1f;

        ambience.GetComponent<AudioSource>().volume = 0f;
        // Fade in each object
        foreach (GameObject obj in fadeObjs)
        {
            if (obj.GetComponent<OpacityControl>() != null)
            {
                OpacityControl opac = obj.GetComponent<OpacityControl>();

                StartCoroutine(opac.Fade(duration, 1));

            }

        }

        //Wait 
        yield return new WaitForSeconds((duration));

        //Activate Animations on each object
        foreach (GameObject obj in fadeObjs)
        {
            if (obj.GetComponent<Animator>() != null)
            {
                obj.GetComponent<Animator>().SetBool("Activate", true);
            }

        }
        
        // Wait some more
        yield return new WaitForSeconds(duration * 4);


        //Fade out each object
        foreach (GameObject obj in fadeObjs)
        {
            if (obj.GetComponent<OpacityControl>() != null)
            {
                OpacityControl opac = obj.GetComponent<OpacityControl>();

                StartCoroutine(opac.Fade(duration, -1));

            }

        }

       
    }


}
