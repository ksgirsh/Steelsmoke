using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInteract : MonoBehaviour
{
    Transform setParent;
    [SerializeField] Transform prevParent;
    // Start is called before the first frame update
    void Start()
    {
        prevParent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MovingPlat")
        {
            setParent = other.gameObject.transform;
            transform.parent = setParent;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "MovingPlat")
        {
            transform.parent = prevParent;
        }
    }
}
