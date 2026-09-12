using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catenary : MonoBehaviour
{
    //length of catenary
    [field:SerializeField] protected float l = 5f;

    public float ParameterizedCatenary(Vector2 pA, Vector2 pB, float xV)
    {

        float v = Mathf.Abs(pB.y - pA.y);
        float h = Mathf.Abs(pB.x - pA.x);

        //Debug.Log(v + " " + h);



        //failsafe in case im a dumbass. if arc length is less than v or h then unity crashes. because it makes an infinite while loop (among other bad things)
        if (l < v || l < h)
        {
            l = Mathf.Sqrt(Mathf.Pow(v, 2) + Mathf.Pow(h, 2)) + 40;
            Debug.LogError("Arclength too small!");
        }

        float a = BisectionMethod(1f, 0.001f, h, v);


       //Debug.Log("h: " + h + ", a: " + a + ", v: " + v);
        //default base for Mathf.Log = e
        double pDouble = (pA.x + pB.x - (a * Mathf.Log((l + v) / (l - v)))) / 2f;
        
      //  Debug.Log("pA.y + pB.y = " + (pA.y + pB.y) + ", l / tanh() = " + (l / System.Math.Tanh(h / (2f * a))) + ". . . . () = " + (h / (2f * a)));
        double qDouble = ((pA.y + pB.y) - (l / System.Math.Tanh(h / (2f * a)))) / 2f;
       // Debug.Log("q: " + qDouble);

        float p = (float)pDouble;
        float q = (float)qDouble;

        return StandardCatenary(a, p, q, xV);
    }


    //uses bisection method / binary search algorithm to approximate a better, ideally precision is something like 0.0001 (low value)
    float BisectionMethod(float intStep, float precision, float h, float v)
    {

        float a = NumericalIntegration(intStep, h, v);
       // Debug.Log(a);

        float aPrev = a - intStep;
        float aNext = a;

        while ((aNext - aPrev) > precision)
        {
            //Debug.Log("aPrev: " + aPrev + " aNext: " + aNext);
            //Debug.Log("thing: " + Mathf.Sqrt(Mathf.Pow(l, 2) - Mathf.Pow(v, 2)) + "condition: " + (2 * a * System.Math.Sinh(h / (2 * a))));
            //avg
            a = (aPrev + aNext) / 2f;

            //numerical integration method, gives rough estimate of a that is refined thru iterations
            if (Mathf.Sqrt(Mathf.Pow(l, 2) - Mathf.Pow(v, 2)) < (2 * a * System.Math.Sinh(h / (2 * a))))
            {
                aPrev = a;

            } else
            {
                aNext = a;
            }

            

        }

        return a;
    }

    float NumericalIntegration(float intStep, float h, float v)
    {
        float val = 0.1f;

        //one side of the transcendental equation
        float thing = Mathf.Sqrt(Mathf.Pow(l, 2) - Mathf.Pow(v, 2));
       // Debug.Log("thing: " + thing + "condition: " + (val * 2 * System.Math.Sinh(h / (2 * val))));

        while (thing < (val * 2 * System.Math.Sinh(h/(2*val))))
        {
            val += intStep;
            //Debug.Log(val);

            if (thing >= (val * 2 * System.Math.Sinh(h / (2 * val))))
            {
                break;
            }
        }

        return val;

    }


    public float StandardCatenary(float a, float p, float q, float x)
    {
        double val = System.Math.Cosh((x - p) / a) + q;

        return (float)val;
    }
}
