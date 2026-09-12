using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLine : MonoBehaviour
{
    [SerializeField] Transform[] startEnd;
    [SerializeField] LineRenderer lr;

    [SerializeField] Transform player;
    public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void Draw()
    {
        //clawLineEnd
        lr.SetPosition(0, startEnd[0].position);
        lr.SetPosition(1, startEnd[1].position);
    }
}
