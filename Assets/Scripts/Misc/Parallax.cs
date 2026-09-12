using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class Parallax : MonoBehaviour
{
    private float startPos, length;

    private float startPosY, height;

    [SerializeField] GameObject cam;
    [SerializeField] float parallaxEffect;

    [SerializeField] GameObject freecam;

    [SerializeField] float parallaxEffectY = 0;
    // Start is called before the first frame update
    void Start()
    {
        startPos = this.transform.position.x;
        if (parallaxEffectY != 0)
        {
            startPosY = this.transform.position.y;
        }


        if (GetComponent<SpriteRenderer>() != null)
        {
            length = GetComponent<SpriteRenderer>().bounds.size.x;

            if (parallaxEffectY != 0)
            {
                height = GetComponent<SpriteRenderer>().bounds.size.y;
            }

        }

        // length = 0f;

        cam = GameObject.FindWithTag("MainCamera");
        StartCoroutine(LateStart());

        float distance = cam.transform.position.x * parallaxEffect;
        float movement = cam.transform.position.x * (1 - parallaxEffect);




        transform.position = new Vector3(startPos + distance, this.transform.position.y, this.transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }

        if (parallaxEffectY != 0)
        {
            ParaY();
        }

    }

    IEnumerator LateStart()
    {
        yield return null;

        int movementa = Mathf.RoundToInt(cam.transform.position.x / length);
        transform.position = new Vector3((length * movementa), this.transform.position.y, this.transform.position.z);

    }
    // Update is called once per frame
    
    void LateUpdate()
    {
       
        if (freecam.activeSelf == false)
        {
            float distance = cam.transform.position.x * parallaxEffect;
            float movement = cam.transform.position.x * (1 - parallaxEffect);




            transform.position = new Vector3(startPos + distance, this.transform.position.y, this.transform.position.z);

            if (movement > startPos + length)
            {
                startPos += length;
            }
            else if (movement < startPos - length)
            {
                startPos -= length;
            }

            if (parallaxEffectY != 0)
            {
                ParaY();
            }

        }

    }
    
    void FixedUpdate()
    {
        
        if (freecam.activeSelf == true)
        {

            float distance = cam.transform.position.x * parallaxEffect;
            float movement = cam.transform.position.x * (1 - parallaxEffect);

            
           

            transform.position = new Vector3(startPos + distance, this.transform.position.y, this.transform.position.z);

            if (movement > startPos + length)
            {
                startPos += length;
            }
            else if (movement < startPos - length)
            {
                startPos -= length;
            }

            if (parallaxEffectY != 0)
            {
                ParaY();
            }

        }
    }

    void ParaY()
    {
        float distanceY = cam.transform.position.y * parallaxEffectY;
        float movementY = cam.transform.position.y * (1 - parallaxEffectY);

        transform.position = new Vector3(transform.position.x, startPosY + distanceY, this.transform.position.z);

        if (movementY > startPosY + length)
        {
            startPosY += length;
        }
        else if (movementY < startPosY - length)
        {
            startPosY -= length;
        }
    }
}
