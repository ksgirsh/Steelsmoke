using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class Freecam : MonoBehaviour
{
    //[SerializeField] AttachtoObject attach;
    [SerializeField] GameObject freecam;
    // Start is called before the first frame update
    void Start()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        //attach = cam.GetComponent<AttachtoObject>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        freecam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
       // freecam.GetComponent<CinemachineCamera>().Target.TrackingTarget = player.transform;
        //attach.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Player")
        {
            freecam.SetActive(true);

        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (freecam != null)
            {
                freecam.SetActive(false);
            }
            
        }
    }

}
