using UnityEngine;

public class CloudAnim : MonoBehaviour
{
    [SerializeField] float moveTime;
    float length;

    float initialTime;

    private Vector3 toPosition;
    private Vector3 initialPos;

    [SerializeField] GameObject cam;
    [SerializeField] float xOffset;

    [SerializeField] GameObject freecam;
    [SerializeField] int cloudGroupCount = 3;

    float movement = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Vector3 setV = new Vector3(xOffset, transform.localPosition.y, 0f);
        transform.position = setV;

        cam = GameObject.FindWithTag("MainCamera");
        initialTime = 0;

        length = GetComponent<SpriteRenderer>().bounds.size.x;

        
        //move a distance length over duration moveTime

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (freecam.activeSelf == false)
        {
            Move();

           // Debug.Log("Moving with Fixed: " + gameObject.name);
        }

    }

    void LateUpdate()
    {
        if (freecam.activeSelf)
        {
            Move();
           // Debug.Log("Moving with Late: " + gameObject.name);
        }
    }

    void Move()
    {
        //Debug.Log("Mooooving");

        float deltaTime = Time.time - initialTime;


        float j = Mathf.InverseLerp(initialTime, (initialTime + moveTime), deltaTime);

        toPosition = new Vector3(movement + length, 0, 0);
        initialPos = new Vector3(movement, 0, 0);

        if (deltaTime >= (initialTime + moveTime))
        {
            initialTime += (moveTime / 2);
        }

        transform.localPosition = Vector3.Lerp(initialPos, toPosition, j);

        SetNearPlayer();
    }

    void SetNearPlayer()
    {
        Transform cloudGroup = transform.parent;
        

    
        Transform cam = GameObject.FindWithTag("MainCamera").transform;

        float dist = cam.position.x - cloudGroup.position.x;
        float totalWidth = (length + Mathf.Abs(xOffset));

        //Debug.Log(dist + " width: " + totalWidth);

        if (dist > totalWidth)
        {
            

            Vector3 move = new Vector3(length * cloudGroupCount, 0f, 0f);
           
            cloudGroup.localPosition += move;
           // Debug.Log("Repositioned object " + cloudGroup.gameObject.name + ", by: " + move);

        }

        if (dist < -totalWidth)
        {
            Vector3 move = new Vector3(-length * cloudGroupCount, 0f, 0f);

            cloudGroup.localPosition += move;
          //  Debug.Log("Repositioned object " + cloudGroup.gameObject.name + ", by: " + move);
        }

    }

}
