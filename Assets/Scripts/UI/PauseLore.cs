using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseLore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subjectText;
    [SerializeField] TextMeshProUGUI descText;

    [SerializeField] string[] subjects;
    [SerializeField] string[] descriptions;

    private int scene;

    [SerializeField] int debugText;
    public bool bossAggro;

    // Start is called before the first frame update
    void Start()
    {
            scene = SceneManager.GetActiveScene().buildIndex;
            LoadText(scene);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            

            if (debugText >= subjects.Length)
            {
                debugText = 0;

            } else
            {
                debugText++;
            }
            LoadText(debugText);

        }
    }

    public void LoadText(int Index)
    {
        if (Index < subjects.Length)
        {
            subjectText.text = subjects[Index];
            descText.text = descriptions[Index];
        }
        debugText = Index;

    }


}
