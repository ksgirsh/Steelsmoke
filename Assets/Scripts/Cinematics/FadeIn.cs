using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeIn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PulseFadeSpr(float pulseDur, GameObject gameObj)
    {
        for (float t = 0f; t < pulseDur; t += Time.deltaTime)
        {
            float normalizedTime = t / pulseDur;

            float lerpLight = Mathf.Lerp(1, 0, normalizedTime);

            if (gameObj.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer rend = gameObj.GetComponent<SpriteRenderer>();
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, lerpLight);
            }
            else if ((gameObj.GetComponent<Image>() != null))
            {
                Image rend = gameObj.GetComponent<Image>();
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, lerpLight);

            }

            yield return null;
        }


        if (gameObj.GetComponent<Image>() != null)
        {
            Image rend = gameObj.GetComponent<Image>();
            float alpha = 0f;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);

        }

        if (gameObj.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer rend = gameObj.GetComponent<SpriteRenderer>();
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
        }
    }

    public IEnumerator PulseColorSpr(float pulseDur, GameObject gameObj, Color targetColor)
    {
        Color initColor = Color.black;
        if (gameObj.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer rend = gameObj.GetComponent<SpriteRenderer>();
            initColor = rend.color;
        }
        else if ((gameObj.GetComponent<Image>() != null))
        {
            Image rend = gameObj.GetComponent<Image>();
            initColor = rend.color;

        }

        for (float t = 0f; t < pulseDur; t += Time.deltaTime)
        {
            float normalizedTime = t / pulseDur;

            Color lerpColor = Color.Lerp(targetColor, initColor, t);

            if (gameObj.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer rend = gameObj.GetComponent<SpriteRenderer>();
                rend.color = lerpColor;
            }
            else if ((gameObj.GetComponent<Image>() != null))
            {
                Image rend = gameObj.GetComponent<Image>();
                rend.color = lerpColor;

            }

            yield return null;
        }


        if (gameObj.GetComponent<Image>() != null)
        {
            Image rend = gameObj.GetComponent<Image>();
            rend.color = initColor;

        }

        if (gameObj.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer rend = gameObj.GetComponent<SpriteRenderer>();
            rend.color = initColor;
        }
    }
}
