using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [field: SerializeField] public int healthGive {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        healthGive = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
