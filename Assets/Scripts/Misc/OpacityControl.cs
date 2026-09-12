using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityControl : MonoBehaviour
{
    private SpriteRenderer rend;

    [SerializeField] bool oscillate;

    
    [SerializeField] float frequency;


    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<SpriteRenderer>();

        if (!oscillate)
        {
            rend.color = new Color(1, 1, 1, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oscillate)
        {
            float lerpAlpha = Mathf.Abs(Mathf.Sin(Time.time * frequency));
            rend.color = new Color(1, 1, 1, lerpAlpha);
        }
  
    }

    public IEnumerator Fade(float duration, int pos1orneg1)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            float lerpAlpha = Mathf.Lerp(0 - pos1orneg1, 1 + pos1orneg1, normalizedTime);
            //Debug.Log(lerpAlpha);

            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, lerpAlpha);
            yield return null;
        }
        
        if (pos1orneg1 == -1)
        {
            gameObject.SetActive(false);
        }

        
    }
}
