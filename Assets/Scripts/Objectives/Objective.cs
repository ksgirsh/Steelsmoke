using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Objective : MonoBehaviour
{
    
    public int objAmt;
    [HideInInspector] public string objText;
    public TextMeshProUGUI objTextObject;

    protected GameObject player;



    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return null;
        ChangeText();
    }

    protected void ObjectiveFailed()
    {
        //failed obj
      //  Debug.Log("Dawwwwww :((");
        objTextObject.color = Color.red;
    }

    protected void ObjectiveSucceded()
    {
        //won obj
       // Debug.Log("Yaaaayyy!!!");
        objTextObject.color = Color.green;
    }

    public void ChangeText()
    {
        objTextObject.text = objText;
    }
}
