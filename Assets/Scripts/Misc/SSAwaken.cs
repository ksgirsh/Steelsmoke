using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SSAwaken : MonoBehaviour
{
    [SerializeField] GameObject[] backgroundElements;
   

    [SerializeField] Light2D bgLight;
    [SerializeField] Light2D levelLight;
    [SerializeField] float fadeInTime;
    [SerializeField] float[] lightlevels;

    [SerializeField] AudioClip ssSong;

    private bool isActivated; 
    // Start is called before the first frame update
    void Start()
    {
        backgroundElements = GameObject.FindGameObjectsWithTag("BackgroundElement");
    }
    public void Awaken()
    {
        if (!isActivated)
        {
            StartCoroutine(Enlighten());
        }
      
    }

    IEnumerator Enlighten()
    {
        isActivated = true;

        StartCoroutine(LightUp(bgLight, lightlevels[0]));
        
        StartCoroutine(LightUp(levelLight, lightlevels[1]));

        yield return new WaitForSeconds(fadeInTime);

        for (int i = 0; i < backgroundElements.Length; i++)
        {
            GameObject elem = backgroundElements[i];
            if (elem.GetComponent<Animator>() != null)
            {
                Animator anim = elem.GetComponent<Animator>();
                anim.SetBool("Activate", true);
            }

            if (elem.activeSelf == false)
            {
                elem.SetActive(true);
            }
        }

       
    }

    IEnumerator LightUp(Light2D light, float level)
    {
    

        
        for (float t = 0f; t < fadeInTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeInTime;

            float lerpLight = Mathf.Lerp(0, level, normalizedTime);

            light.intensity = lerpLight;

            yield return null;
        }
        


    }
}
