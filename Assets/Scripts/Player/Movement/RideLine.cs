using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RideLine : MonoBehaviour
{
    [SerializeField] float yOffset;
    public LineRenderer line;

    private Rigidbody2D rb;
    [SerializeField] float rideSpeed;

    //its cool to observe how these values change
    [SerializeField] float currentRideSpeed;
    [SerializeField] int currentIndex;

    [SerializeField] List<Vector3> points;
    private Vector3 currentPoint;

    

    bool startRide = false;

    [SerializeField] float accelTime;

    [SerializeField] bool goRight;
    [SerializeField] RunAndJump rj;

    [SerializeField] float thisYOffset;

 //   [SerializeField] GameObject ziplineObj;
//    Rigidbody2D zipRB;
    Transform prevParent;
    [SerializeField] Transform player;

    [SerializeField] ClawLine clawLine;

    // Start is called before the first frame update
    void Start()
    {
        //  ziplineObj.SetActive(false);
        //  zipRB = ziplineObj.GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (player.gameObject.GetComponent<RunAndJump>() != null)
        {
            rj = player.gameObject.GetComponent<RunAndJump>();
        }


        points.Clear();

        if (transform.localScale.x > 0)
        {
            goRight = true;
        } else
        {
            goRight = false;
        }

        rb = gameObject.GetComponent<Rigidbody2D>();
        PointSet();
    }

    // Update is called once per frame
    void Update()
    {
        if (goRight)
        {
            if (currentIndex < (points.Count - 1))
            {
                PointCheck();

            }

        } else if (currentIndex > 0)
        {
            PointCheck();
        }


        if (Input.GetKeyDown(KeyCode.E) && startRide == true)
        {
            RideOver(-Vector2.up);
        }

        if (startRide == true)
        {

            SetVelocity();
            player.transform.position = new Vector3(transform.position.x, transform.position.y + yOffset);
            clawLine.Draw();
        }


    }

    public void AssignLine(LineRenderer lr)
    {
        line = lr;
    }

    public void TriggerRide()
    {

        prevParent = player.transform.parent;
        player.transform.parent = gameObject.transform;


        if (rj != null)
        {
            rj.riding = true;
        }

        

        PointSet();
        RightLogic();
        PointSet();
        transform.position = new Vector3(currentPoint.x, currentPoint.y + thisYOffset);
        

        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player.GetComponent<Rigidbody2D>().gravityScale = 0f;

        startRide = true;
        StartCoroutine(Accelerate());
    }

    void RightLogic()
    {
        int middleIndex = ((line.positionCount / 2) - 1);

        Debug.Log(middleIndex);

        Vector3 middlePoint = points[middleIndex];
        Vector3 dist = player.position - middlePoint;


        if (dist.x > 0)
        {
            goRight = false;

        } else
        {
            goRight = true;
        }
        
    }



    void PointSet()
    {
        if (line != null)
        {
            for (int i = 0; i < line.positionCount; i++)
            {
                if (points.Count > i)
                {
                    points[i] = line.GetPosition(i);

                } else
                {

                    points.Add(line.GetPosition(i));
                }


            }

            if (goRight)
            {
                //start from left-most side
                currentIndex = 0;
            }
            else
            {
                //start from right-most side
                currentIndex = (points.Count - 1);
            }

            currentPoint = points[currentIndex];

        } else
        {
            Debug.LogWarning("No Line Renderer");
        }

      
    }

    void PointCheck()
    {
        if (goRight)
        {
            //iterate right
            if (currentIndex < (points.Count - 1))
            {
                if (transform.position.x > points[currentIndex].x)
                {
                    currentIndex++;
                    currentPoint = points[currentIndex];
                }
            }
        } else
        {
            //iterate left
            if (currentIndex > 0)
            {
                if (transform.position.x < points[currentIndex].x)
                {
                    currentIndex--;
                    currentPoint = points[currentIndex];
                }
            }
        }


    }

    void SetVelocity()
    {
        if (goRight)
        {
            int nextIndex = (int)(currentIndex + 1);

            if (currentIndex < (points.Count - 1))
            {

                Vector3 dist = points[nextIndex] - points[currentIndex];

                Vector3 dir = dist.normalized;


                rb.linearVelocity = dir * currentRideSpeed;
               

            } else if (currentIndex == (points.Count - 1) && transform.position.x < points[(points.Count - 1)].x)
            {
                nextIndex = currentIndex;
                currentIndex--;

                Vector3 dist = points[nextIndex] - points[currentIndex];

                Vector3 dir = dist.normalized;


                rb.linearVelocity = dir * currentRideSpeed;


            } else
            {
                int lastIndex = (points.Count - 1);
                int penumIndex = lastIndex - 1;

                Vector3 dist = points[lastIndex] - points[penumIndex];
                Vector3 dir = dist.normalized;
 
                RideOver(dir);
                //ride over
            }


        } else
        {
            int nextIndex = (int)(currentIndex - 1);

            if (currentIndex > 0)
            {
                

                Vector3 dist = points[nextIndex] - points[currentIndex];

                Vector3 dir = dist.normalized;


                rb.linearVelocity = dir * currentRideSpeed;


            } else if (currentIndex == 0 && transform.position.x > points[0].x)
            {
                nextIndex = currentIndex;
                currentIndex++;

                Vector3 dist = points[nextIndex] - points[currentIndex];

                Vector3 dir = dist.normalized;


                rb.linearVelocity = dir * currentRideSpeed;
     
            } else
            {

                Vector3 dist = points[0] - points[1];
                Vector3 dir = dist.normalized;

                RideOver(dir);
                //ride over
            }
        }
        /*
        //unpleasant way of writing this
        if (goRight && currentIndex <= (points.Count - 1))
        {
            

            int nextIndex = (int)(currentIndex + 1);

            // if current index is last point
            if (currentIndex == (points.Count - 1) && transform.position.x != points[currentIndex].x)
            {
                nextIndex = currentIndex;
                currentIndex--;


            } else if (currentIndex == (points.Count - 1))
            {
                //ride over
                /*
                int ci = 0;

                if (goRight)
                {
                    ci = (points.Count - 2);
                }
                else
                {
                    ci = 1;
                }


                nextIndex = (int)(ci + (1 * Mathf.Sign(transform.localScale.x)));
                Vector3 dist = points[nextIndex] - points[ci];

                //  Debug.Log(points[nextIndex] + " " + points[ci] + " " + dist);
                
               // Vector3 dir = dist.normalized;

                RideOver(Vector3.zero);

            }

            

            Vector3 dist = points[nextIndex] - points[currentIndex];

            Vector3 dir = dist.normalized;


            rb.velocity = dir * currentRideSpeed;

        } else if (goRight == false && currentIndex >= (1))
        {
            int nextIndex = (int)(currentIndex - 1);

            Vector3 dist = points[nextIndex] - points[currentIndex];

            Vector3 dir = dist.normalized;


            rb.velocity = dir * currentRideSpeed;

        }*/



    }

    IEnumerator Accelerate()
    {
        for (float i = 0; i < accelTime; i += Time.deltaTime)
        {
            currentRideSpeed = Mathf.Lerp(0, rideSpeed, (i/accelTime));
            yield return null;
        }
    }

    void RideOver(Vector3 finalPoint)
    {


        player.GetComponent<Rigidbody2D>().gravityScale = 1f;
        startRide = false;
        player.transform.parent = prevParent;
        rb.linearVelocity = Vector2.zero;


        if (rj != null)
        {
            rj.riding = false;
          //  rb.AddForce(transform.up * 30f, ForceMode2D.Impulse);
            StartCoroutine(rj.ForceEffect((finalPoint + (transform.up * 0.25f)), 1.5f));



        }


        gameObject.SetActive(false);
    }
}
