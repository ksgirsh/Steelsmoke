using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierRenderer : MonoBehaviour
{
    public Transform[] controlPoints;
    [SerializeField] LineRenderer lr;

    [SerializeField] int renderedPoints;

    [SerializeField] Sprite turnOffSprite;

    // Start is called before the first frame update
    void Start()
    {
        
        foreach (Transform trans in controlPoints)
        {
            if (trans.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                if (trans.gameObject.GetComponent<SpriteRenderer>().sprite == turnOffSprite)
                {
                    trans.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
            }


        }

        lr.useWorldSpace = true;
        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }


    void Draw()
    {
        lr.positionCount = renderedPoints;
        for (int i = 0; i < renderedPoints; i++)
        {
            float progress = (i / (renderedPoints - 1f));

            Vector2 bez = Bezier(progress);

            Vector3 iPos = new Vector3(bez.x, bez.y, 0f);

            lr.SetPosition(i, iPos);
        
        }
    }



    Vector2 Bezier(float t)
    {
        Vector2 finalVec = Vector2.zero;
       
        List<List<Vector2>> lerpList = new List<List<Vector2>>();

        for (int i = 1; i < controlPoints.Length + 1; i++)
        {
            int requiredLerps = controlPoints.Length - (i - 1);



            


            List<Vector2> lerpListDegreeI = new List<Vector2>();

            //fill list i with stuff
            if (i != 1)
            {
                
                //elements in individual list, k always < length of last list
                for (int k = 0; k < requiredLerps; k++)
                {
                    float kx = lerpList[i - 2][k].x;
                    float ky = lerpList[i - 2][k].y;

                    float kNextx = lerpList[i - 2][k + 1].x;
                    float kNexty = lerpList[i - 2][k + 1].y;

                    Vector2 kVector = new Vector2(kx, ky);
                    Vector2 kNextVector = new Vector2(kNextx, kNexty);

                    Vector2 lerpVector = LerpVector(kVector, kNextVector, t);

                    lerpListDegreeI.Add(lerpVector);
                }

            } else
            {
                //fill initial list
                //elements in individual list
                for (int k = 0; k < requiredLerps; k++)
                {
                    Vector2 kVector = new Vector2(controlPoints[k].position.x, controlPoints[k].position.y);
                    lerpListDegreeI.Add(kVector);
                }

            }




            lerpList.Add(lerpListDegreeI);

            //last iteration; 
            if (requiredLerps == 1)
            {

                int c = lerpList.Count;

                finalVec = new Vector2((lerpList[c - 1][0].x), (lerpList[c - 1][0].y));

            }
        }



        return finalVec;

    }





    Vector2 LerpVector(Vector2 vecA, Vector2 vecB, float t)
    {
        float vecCX = Mathf.Lerp(vecA.x, vecB.x, t);
        float vecCY = Mathf.Lerp(vecA.y, vecB.y, t);

        return new Vector2(vecCX, vecCY);
    }
}
