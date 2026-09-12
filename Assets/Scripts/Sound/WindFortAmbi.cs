using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class WindFortAmbi : MonoBehaviour
{
    [SerializeField] AudioSource windAmbiS;
    [SerializeField] AudioSource indoorsAmbiS;
    [SerializeField] AudioClip windAmbi;
    [SerializeField] AudioClip indoorsAmbi;


    [SerializeField] LayerMask targetLayer;

    Coroutine windTransition;
    Coroutine indoorsTransition;

    //check if scene is windfortress
    void SceneQuery()
    {
        //wont work for leaving and re-entering WF, adjust later
        int s = SceneManager.GetActiveScene().buildIndex;

        gameObject.SetActive(true);
        if (s != 0)
        {
            gameObject.SetActive(false);
        } 
    }

    void Start()
    {
        SceneQuery();
        Wind(true);

        WindCheckOverlapCircle();
    }

    void WindCheckOverlapCircle()
    {
        //layermask isnt REALLY necessary, might change later
        Collider2D[] indoorsCheck = Physics2D.OverlapCircleAll(transform.position, 5f, targetLayer);
        foreach (Collider2D hit in indoorsCheck)
        {
            if (hit.gameObject.tag == "BackgroundElement")
            {
                Wind(false);
            }
        }
    }

    void Wind(bool on = true)
    {
        StopAllCoroutines();

        if (on)
        {
            windTransition = StartCoroutine(FadeVolume(windAmbiS, 0));
            indoorsTransition = StartCoroutine(FadeVolume(indoorsAmbiS, 1));
        } else
        {
            windTransition = StartCoroutine(FadeVolume(windAmbiS, 1));
            indoorsTransition = StartCoroutine(FadeVolume(indoorsAmbiS, 0));
        }

    }

    IEnumerator FadeVolume(AudioSource src, int dir = 0, float dur = 2f)
    {
        
        //1 fades out
        float initVol = src.volume;
        float lerpVol = initVol;
        float targetVol = (1 - dir);

        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            lerpVol = Mathf.Lerp(initVol, targetVol, i);
            src.volume = lerpVol;
            yield return null;

        }

        src.volume = targetVol;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
       
        if (coll.gameObject.tag == "BackgroundElement")
        {
            Wind(false);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "BackgroundElement")
        {
            Wind(true);
        }
    }
}
