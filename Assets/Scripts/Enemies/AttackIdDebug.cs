using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackIdDebug : MonoBehaviour
{
    private TextMeshProUGUI attackIDText;
    [SerializeField] Boss attackSample;

    // Start is called before the first frame update
    void Start()
    {
        attackIDText = GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        //  string text = (attackSample.attackID.ToString() +  "Att Speed: " + attackSample.attSpeed.ToString() + " Parry?: " + attackSample.sentParrying.ToString());
        string text = (attackSample.attackID.ToString() + " " + attackSample.CoroutineDebug);
        attackIDText.text = text;
    }
}
