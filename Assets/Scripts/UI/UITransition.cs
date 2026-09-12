using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UITransition : MonoBehaviour
{
    [SerializeField] Vector3 targetPos;
    public List<float> cachedOpacities;

    [SerializeField] Vector3 secondaryPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator Slide(float duration)
    {
        Vector3 initialPos = transform.position;
        Vector3 lerpPos = initialPos;



        for (float i = 0; i < Mathf.Abs(duration); i += Time.deltaTime)
        {
            lerpPos = Vector3.Lerp(initialPos, targetPos, i);
            transform.position = lerpPos;
            yield return null;
        }
        transform.position = targetPos;
    }

    //WHICH WAY BRUH :sob:
    public void MatryoshkaTransition()
    {
        StartCoroutine(MatryCoroutine());
        
    }

    IEnumerator MatryCoroutine()
    {
        //Debug.Log("Doing the transition");
        //slide into frame
        StartCoroutine(Slide(1f));

        //fade in
        CacheOpacities();
        
        StartCoroutine(MatryFadeIn(0.1f, 1.5f));

        yield return null;
        //fade out and reappear at bottom of screen
        //StartCoroutine(MatryFadeOut(1.7f + 2f, 1.5f));

        //yield return new WaitForSeconds(2f);
        //move to bottom of screen
        //transform.position = secondaryPos;

        //fade back in :)
        //StartCoroutine(MatryFadeIn(3.7f + 1.5f + 1f, 1.5f));
    }

    void CacheOpacities()
    {

        List<Transform> objectsToFade = new List<Transform>();
        objectsToFade.Add(transform);
        AddDescendants(transform, objectsToFade);

        //make new list w/ all children with an image or text component
        List<Transform> spritesToFade = new List<Transform>();
        foreach (Transform imageCheck in objectsToFade)
        {
            if (imageCheck.gameObject.activeSelf == true)
            {
                if (imageCheck.gameObject.GetComponent<Image>() != null)
                {
                    Image imgElem = imageCheck.GetComponent<Image>();
                    spritesToFade.Add(imgElem.gameObject.transform);
                }
            }

            if (imageCheck.gameObject.GetComponent<TextMeshProUGUI>() != null)
            {
                TextMeshProUGUI txt = imageCheck.gameObject.GetComponent<TextMeshProUGUI>();
                spritesToFade.Add(txt.gameObject.transform);
            }

        }

        //iterate thru target list and do the opacity thing (trademark)
        for (int i = 0; i < spritesToFade.Count; i++)
        {
            if (spritesToFade[i].GetComponent<Image>() != null)
            {
                //cache image opacity and set transparent
                Image img = spritesToFade[i].GetComponent<Image>();
                cachedOpacities.Add(img.color.a);
                Color transColor = new Color(img.color.r, img.color.g, img.color.b, 0f);
                img.color = transColor;

            }
            else if (spritesToFade[i].GetComponent<TextMeshProUGUI>() != null)
            {
                //cache text opacity and set transparent
                TextMeshProUGUI txt = spritesToFade[i].GetComponent<TextMeshProUGUI>();
                cachedOpacities.Add(txt.color.a);
                Color transColor = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
                txt.color = transColor;
            }

            //indexes should correspond to components and their respective cached opacities now.
        }

    }


    IEnumerator MatryFadeIn(float initialDelay, float duration)
    {
       // Debug.Log("Did the fade in");
        yield return new WaitForSeconds(initialDelay);
       List<Transform> objectsToFade = new List<Transform>();
       objectsToFade.Add(transform);
       AddDescendants(transform, objectsToFade);

       //make new list w/ all children with an image or text component
       List<Transform> spritesToFade = new List<Transform>();
       foreach (Transform imageCheck in objectsToFade)
       {
           if (imageCheck.gameObject.activeSelf == true)
           {
               if (imageCheck.gameObject.GetComponent<Image>() != null)
               {
                   Image imgElem = imageCheck.GetComponent<Image>();
                   spritesToFade.Add(imgElem.gameObject.transform);
               }
           }

           if (imageCheck.gameObject.GetComponent<TextMeshProUGUI>() != null)
           {
               TextMeshProUGUI txt = imageCheck.gameObject.GetComponent<TextMeshProUGUI>();
               spritesToFade.Add(txt.gameObject.transform);
           }

       }

       for (int i = 0; i < spritesToFade.Count; i++)
       {
           //self. this is fucking unreadable
           if (spritesToFade[i].GetComponent<Image>() != null)
           {
               //if it finds an image it fades in the image
               Image IMAGEYAY = spritesToFade[i].GetComponent<Image>();
               StartCoroutine(Fade(duration, IMAGEYAY, null, 0, cachedOpacities[i]));

           } else if (spritesToFade[i].GetComponent<TextMeshProUGUI>() != null)
           {
               //if it finds text it fades in the text
               TextMeshProUGUI TEXTWOO = spritesToFade[i].GetComponent<TextMeshProUGUI>();
               StartCoroutine(Fade(duration, null, TEXTWOO, 0, cachedOpacities[i]));
           }
       }

    }

    IEnumerator MatryFadeOut(float initDelay, float duration)
    {
       // Debug.Log("Did the fade out");
        yield return new WaitForSeconds(initDelay);
        //this shouldnt be bad, just call a fade out for each relevant obj 

        List<Transform> objectsToFade = new List<Transform>();
        objectsToFade.Add(transform);
        AddDescendants(transform, objectsToFade);

        //make new list w/ all children with an image or text component
        List<Transform> spritesToFade = new List<Transform>();
        foreach (Transform imageCheck in objectsToFade)
        {
            if (imageCheck.gameObject.activeSelf == true)
            {
                if (imageCheck.gameObject.GetComponent<Image>() != null)
                {
                    Image imgElem = imageCheck.GetComponent<Image>();
                    spritesToFade.Add(imgElem.gameObject.transform);
                }
            }

            if (imageCheck.gameObject.GetComponent<TextMeshProUGUI>() != null)
            {
                TextMeshProUGUI txt = imageCheck.gameObject.GetComponent<TextMeshProUGUI>();
                spritesToFade.Add(txt.gameObject.transform);
            }

        }

        for (int i = 0; i < spritesToFade.Count; i++)
        {
            if (spritesToFade[i].GetComponent<Image>() != null)
            {
                //if it finds an image it fades out the image
                Image IMAGEYAY = spritesToFade[i].GetComponent<Image>();
                StartCoroutine(Fade(duration, IMAGEYAY, null, 1));

            }
            else if (spritesToFade[i].GetComponent<TextMeshProUGUI>() != null)
            {
                //if it finds text it fades out the text
                TextMeshProUGUI TEXTWOO = spritesToFade[i].GetComponent<TextMeshProUGUI>();
                StartCoroutine(Fade(duration, null, TEXTWOO, 1));
            }
        }

    }

    IEnumerator Fade(float dur, Image img = null, TextMeshProUGUI txt = null, int direction = 0, float fadeTo = 1f)
    {

        
        Color initColor = Color.blue;
        //this block pisses me off but its fine for now i guess
        if (img != null)
        {
            initColor = img.color;

        } else if (txt != null)
        {
            initColor = txt.color; 
        }

        float target = (1f * fadeTo) - (direction * fadeTo);

        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            //if direction is 1 then it fades out, fadeTo the target opacity for fadeIns (direction = 0)
            float alpha = Mathf.Lerp(0f + direction * fadeTo, target, i);

            Color newImgColor = new Color(initColor.r, initColor.g, initColor.b, alpha);

            if (img != null)
            {
                img.color = newImgColor;

            }
            else if (txt != null)
            {
                txt.color = newImgColor;
            }

            yield return null;
        }


    }


    private void AddDescendants(Transform parent, List<Transform> list)
    {
        foreach (Transform child in parent)
        {
            list.Add(child);
            AddDescendants(child, list);
        }
    }
}
