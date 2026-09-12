using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatenaryRenderer : Catenary
{
    [SerializeField] LineRenderer lr;
    [SerializeField] int points = 10;

    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        lr.useWorldSpace = true;
        lr.SetPosition(0, pointA.position);
        lr.SetPosition((lr.positionCount - 1), pointB.position);

        lr.positionCount = points;
        Draw();
        //Debug.Log(ParameterizedCatenary(pointA, pointB, 1));

        /*
        for (int i = 0; i < points; i++)
        {
            float progress = (i / (points - 1f));

            Debug.Log(progress);

            float xP = Mathf.Lerp(pointA.x, pointB.x, progress);

            Debug.Log(ParameterizedCatenary(pointA, pointB, i));

            float y = ParameterizedCatenary(pointA, pointB, xP);

            Vector3 newPos = new Vector3(xP, y);

            Debug.Log(newPos);
        }*/
        //Debug.Log(pointB.x + " " + ParameterizedCatenary(pointA.position, pointB.position, (pointB.position.x)));

    }


    // Update is called once per frame
    void Update()
    {
         Draw();
    }

    void Draw()
    {
        
        for (int i = 0; i < points; i++)
        {
            float progress = (i / (points - 1f));
            
            float xP = Mathf.Lerp(pointA.position.x, pointB.position.x, progress);

            Vector3 pointI = lr.GetPosition(i);

            //Debug.Log(xP);


            float y = ParameterizedCatenary(pointA.position, pointB.position, xP);

            lr.SetPosition(i, new Vector3(xP, (y), 0));



        }
       // lr.SetPosition((points - 1), pointB);
    }
    

}
