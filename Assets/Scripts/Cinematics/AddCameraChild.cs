using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AddCameraChild : MonoBehaviour
{
    [SerializeField] List<Transform> children;
    [SerializeField] List<Vector3> localPositions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddDescendants(this.transform);

        foreach(Transform child in children)
        {
            //cache localPositions
            localPositions.Add(child.localPosition);
        }



        GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        AssignDescendants(mainCam.transform);


    }

    private void AddDescendants(Transform parent)
    {
        foreach (Transform child in parent)
        {
            children.Add(child);
            AddDescendants(child);
        }
    }

    public void AssignDescendants(Transform target)
    {
        for (int i = 0; i < children.Count; i++)
        {
            Transform child = children[i];
            child.parent = target;
            child.localPosition = localPositions[i];
        }
    }
}
