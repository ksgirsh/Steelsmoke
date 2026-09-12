using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DarkenLight : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] float shadowIntensity;




    // Update is called once per frame
    void Update()
    {
        light2D.intensity = -shadowIntensity;
    }
}
