using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachtoObject : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform trans;
    public float zOffset = -24.0f;
    [SerializeField] float xOffset;

    [SerializeField] bool attachToX;

    void Start()
    {
        /*
        Transform trans = GetComponent<Transform>();
        Vector3 pos = trans.position;
        pos.z += zOffset;
        pos.x += xOffset * trans.localScale.x;
        this.transform.position = pos;
        */
    }

    // Update is called once per frame
    void Update()
    {
       if (!attachToX)
       {
           Vector3 pos = trans.position;
           pos.z += zOffset;
           pos.x += xOffset * trans.localScale.x;
           this.transform.position = pos;

       } else
       {
            transform.position = new Vector3(trans.position.x, transform.position.y, transform.position.z);
       }

        
        
        
    }

    
    
}
