using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightControl : MonoBehaviour
{
    [SerializeField] Light2D light2D;

    public bool glow = false;
    [SerializeField] bool deadStart = true;

    [SerializeField] float intensityRange;
    [SerializeField] float minIntensity = 0;
    [SerializeField] float glowSpeed;
    [SerializeField] float minGlowRange = 0;
    //public bool pulse = false;
    //[SerializeField] float pulseDur;

    // Start is called before the first frame update
    void Start()
    {
        if (deadStart)
        {
            light2D.intensity = 0f;
        }    

    }

    // Update is called once per frame
    void Update()
    {
        if (glow)
        {
            light2D.intensity = Mathf.Clamp(Mathf.Abs((intensityRange * (Mathf.Sin(Time.time * glowSpeed)))), minIntensity, intensityRange);   
            light2D.pointLightOuterRadius = Mathf.Clamp(Mathf.Abs((intensityRange * (Mathf.Sin(Time.time * glowSpeed)))), minGlowRange, intensityRange);
            //Debug.Log(Mathf.Clamp(Mathf.Abs((intensityRange * (Mathf.Sin(Time.time * glowSpeed)))), minGlowRange, intensityRange));
        }
    }

    public IEnumerator Pulse(float pulseDur, float pulseBrightness)
    {
        for (float t = 0f; t < pulseDur; t += Time.deltaTime)
        {
            float normalizedTime = t / pulseDur;
      
            float lerpLight = Mathf.Lerp(pulseBrightness, 0, normalizedTime);
         
            light2D.intensity = lerpLight;
          
            yield return null;
        }
    }

    public void SetColor(Color color)
    {
        light2D.color = color;
    }

    public void SetRange(float range)
    {
        light2D.pointLightOuterRadius = range;
    }

    public void SetIntensity(float intense)
    {
        light2D.intensity = intense;
    }

    
}
