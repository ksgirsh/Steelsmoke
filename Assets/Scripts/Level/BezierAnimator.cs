using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BezierAnimator : MonoBehaviour
{
    LineRenderer lr;

    [SerializeField] int[] movePoints;
    [SerializeField] float amp;
    [SerializeField] float freq;


    [SerializeField]
    [Range(0.0f, 1.0f)]
    float xFac;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float yFac;

    [SerializeField] BezierRenderer bR;


    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (lr != null && Time.timeScale != 0)
        {

            BlowPoints();

        }

    }



    void BlowPoints()
    {
        //blow chosen points in a circle
        float cOffset = Mathf.Cos(Time.time * freq) * amp;
        float sOffset = Mathf.Sin(Time.time * freq) * amp;

    

       // Vector3 offset = new Vector3(0, sOffset, 0);

       // transform.position += offset;

        
        for (int i = 0; i < movePoints.Length; i++)
        {
            Vector3 currentPos = bR.controlPoints[movePoints[i]].position;

            Vector3 offset = new Vector3(cOffset * xFac, sOffset * yFac, 0);

            bR.controlPoints[movePoints[i]].position += offset;

        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, amp);
    }
}
