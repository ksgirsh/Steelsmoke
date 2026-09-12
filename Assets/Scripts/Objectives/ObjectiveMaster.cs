using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

//Include modifiers in this script as well.
public class ObjectiveMaster : MonoBehaviour
{
    [SerializeField] float lineDistance = 19.5f;

    //the text object 
    [SerializeField] GameObject objectiveTextObject;

    //Where in the hierarchy the text is being instantiated
    [SerializeField] Transform location;


    //list of all objectives
    [SerializeField] List<Objective> objectiveList;

    //list of objectives to display; why does this have to be public i doont know but it BREAKS if its not pblic.
    public List<GameObject> uiObjects;

    //ref to player
    private GameObject player;

    //title of the level - eg "THE SCRIBE" or "THE SWIFT" idk. might not include in finished product
    [SerializeField] string Title;

    
    void Start()
    {
        GameObject mainUI = GameObject.FindGameObjectWithTag("MainCanvas");

        this.transform.parent = mainUI.transform;

        //add each objective to player
        player = GameObject.FindGameObjectWithTag("Player");

       // Debug.Log(objectiveList.Count);
        for (int i = 0; i < objectiveList.Count; i++)
        {
            //instantiate UI text & add to list
            Vector3 parentPos = location.position;
            Vector3 textPos = new Vector3(parentPos.x, parentPos.y - (uiObjects.Count * lineDistance), parentPos.z);
            GameObject objectiveChild = Instantiate(objectiveTextObject, textPos, Quaternion.identity, location.transform);
            uiObjects.Add(objectiveChild);

            //assign reference
            objectiveList[i].objTextObject = objectiveChild.GetComponent<TextMeshProUGUI>();

            Vector3 cachedLocalPos = objectiveList[i].gameObject.transform.localPosition;

            objectiveList[i].gameObject.transform.parent = player.transform;
            objectiveList[i].gameObject.transform.localPosition = cachedLocalPos;

            objectiveList[i].ChangeText();
        }

 

    }

    void Update()
    {
        
    }
    public void GoToPauseMenu()
    {
        this.transform.parent = UIManager.instance.pauseContainer.transform;
    }

}
