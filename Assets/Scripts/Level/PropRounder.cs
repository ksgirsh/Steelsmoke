using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRounder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = RoundToNearestMultipleOf16(transform.position);
    }

    public Vector2 RoundToNearestMultipleOf16(Vector2 v)
    {
        float multiple = (1/16.0f);

        float roundedX = (float)Mathf.Round(v.x / multiple) * multiple;
        float roundedY = (float)Mathf.Round(v.y / multiple) * multiple;

        return new Vector2(roundedX, roundedY);
    }
}
