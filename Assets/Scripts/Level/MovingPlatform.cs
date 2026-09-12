using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] BezierRenderer bezR;
    [SerializeField] Transform[] controlPoints;
    [SerializeField] float yOffset;

    private Animator anim;
    private bool isTrigger;

    public bool Triggered;

    //rate at which t increases
    [SerializeField] float moveSpeed;
    float currentMoveSpeed;

    [SerializeField] float accelTime;
    float tV;

    bool goRight = true;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        if (Triggered != true && anim != null)
        {
            isTrigger = true;
            anim.CrossFade("triggerPlatInactive", 0, 0);
        }



        currentMoveSpeed = 0;
        //control points
        if (bezR != null)
        {
            controlPoints = bezR.controlPoints;
        }
        StartCoroutine(Accel());
    }

    public void TriggerPlatform()
    {
        Triggered = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Triggered)
        {
            //curve should be c1 continous (tangent points mirrored)
            Move();
        }

        if (isTrigger == true && Triggered == true && anim != null)
        {
            anim.CrossFade("triggerPlat", 0, 0);

            //i dont like constantly loading an animation every frame
            isTrigger = false;
        }

    }

    void Move()
    {
        //how shoul t increase? t should increase with time but reset upon reaching the end of the curve
        if (goRight)
        {
            tV += (Time.deltaTime * currentMoveSpeed);
        } else
        {
            tV -= (Time.deltaTime * currentMoveSpeed);
        }

        Vector3 offsetV = new Vector3(0f, yOffset, 0f);

        if (transform.position == (controlPoints[controlPoints.Length - 1].position + offsetV) && goRight == true)
        {
            StopAllCoroutines();
            tV = 1;
            currentMoveSpeed = 0;

            StartCoroutine(Accel());
            goRight = false;


        }

        if (transform.position == (controlPoints[0].position + offsetV) && goRight == false)
        {
            StopAllCoroutines();
            tV = 0;
            currentMoveSpeed = 0;
            
            StartCoroutine(Accel());
            goRight = true;

        }

        Vector2 nextPos = Bezier(tV);
        nextPos.y += yOffset;

        transform.position = nextPos;
        
    }
    
    IEnumerator Accel()
    {
        
            for (float i = 0; i < accelTime; i += (Time.deltaTime))
            {
                currentMoveSpeed = Mathf.Lerp(0f, moveSpeed, (i));

                yield return null;
            }

    }

    Vector2 Bezier(float t)
    {
        Vector2 finalVec = Vector2.zero;
        float lerpDegrees = (controlPoints.Length);
        List<List<Vector2>> lerpList = new List<List<Vector2>>();

        for (int i = 1; i < controlPoints.Length + 1; i++)
        {
            int requiredLerps = controlPoints.Length - (i - 1);






            List<Vector2> lerpListDegreeI = new List<Vector2>();

            //fill list i with stuff
            if (i != 1)
            {
                //Debug.Log(i);
                //elements in individual list, k always < length of last list
                for (int k = 0; k < requiredLerps; k++)
                {
                    float kx = lerpList[i - 2][k].x;
                    float ky = lerpList[i - 2][k].y;

                    float kNextx = lerpList[i - 2][k + 1].x;
                    float kNexty = lerpList[i - 2][k + 1].y;

                    Vector2 kVector = new Vector2(kx, ky);
                    Vector2 kNextVector = new Vector2(kNextx, kNexty);

                    Vector2 lerpVector = Vector2.Lerp(kVector, kNextVector, t);

                    lerpListDegreeI.Add(lerpVector);
                }

            }
            else
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


            if (requiredLerps == 1)
            {

                int c = lerpList.Count;

                finalVec = new Vector2((lerpList[c - 1][0].x), (lerpList[c - 1][0].y));

            }
        }



        return finalVec;

    }
}
