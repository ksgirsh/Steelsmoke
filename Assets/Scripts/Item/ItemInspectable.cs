using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Linq;

public class ItemInspectable : MonoBehaviour
{
    //[SerializeField] bool isTextBox;
    
    [System.Serializable]

    public class Interactions
    {
        public int[] textIndex;
    }

    [field:SerializeField] public Interactions[] interactions { get; private set; }
    [field: SerializeField] public List<int> lockedInteractions;
    [field:SerializeField] public string[] text { get; private set; }

    [field:SerializeField] public bool maxHealthUp { get; private set; }
    public bool UpgradeClaimed;


    [field: SerializeField] public bool door { get; private set; }
    [field: SerializeField] public Vector3 travelPos { get; private set; }
    [SerializeField] int area;
    // [SerializeField] Inspect insp;

    public bool beingInspected;

    public int currentInteraction = 0;
    public int textLine = 0;

    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;

    [SerializeField] TextBoxController textBox;

    [SerializeField] Sprite boxPortrait;

    [field: SerializeField] public int[] choiceLoc { get; private set; }

    [SerializeField] UnityEvent[] yesChoiceActions;
    private int ActionLine = 0;
    [SerializeField] UnityEvent[] noChoiceActions;

    [SerializeField] UnityEvent[] otherInspectActions;
    [SerializeField] int[] actionLoc;

    [field: SerializeField] public bool immediateYes { get; private set; }


    void Start()
    {
        
        dialogueText = UIManager.instance.dialogueText.GetComponent<TextMeshProUGUI>();
        dialogueBox = UIManager.instance.dialogueBox;
        textBox = UIManager.instance.textBox.GetComponent<TextBoxController>();


    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dialogueText = UIManager.instance.dialogueText.GetComponent<TextMeshProUGUI>();
        dialogueBox = UIManager.instance.dialogueBox;
        textBox = UIManager.instance.textBox.GetComponent<TextBoxController>();

    }


    public void DeinspectActions(Inspect insp)
    {
        //way of identifying as specific object being inspected
        if (beingInspected)
        {
            for (int i = 0; i < actionLoc.Length; i++)
            {
                int cLoc = actionLoc[i];
                if (cLoc > text.Length)
                {
                    otherInspectActions[i].Invoke();
                }
            }

        }


    }

    public void YesChoice()
    {
        yesChoiceActions[ActionLine].Invoke();
        if (yesChoiceActions.Length > 1)
        {
            ActionLine++;
        }

    }

    public void NoChoice()
    {
        noChoiceActions[ActionLine].Invoke();
        if (noChoiceActions.Length > 1)
        {
            ActionLine++;
        }

    }



    // Update is called once per frame
    public void SetText(Inspect insp)
    {

        if (beingInspected)
        {
            string prefix = " > ";

            //If this line is 2nd to last line && max health up
            if (maxHealthUp == true && (text.Length - textLine) == 1)
            {
                textBox.writer = text[textLine];
                textBox.ShowImgBox(boxPortrait);
            }


            if (textLine <= ((interactions[currentInteraction].textIndex.Length) - 1))
            {
                textBox.writer = text[interactions[currentInteraction].textIndex[textLine]];

            }
           
            //First activation
            if (!dialogueBox.activeSelf)
            {  
                dialogueText.text = prefix + textBox.writer;
                
            }

            
        }

     
    }

    public void SetInteraction(int interactIndex)
    {
        currentInteraction = interactIndex;
    }
    
    public void LoadInteraction(int loadInt)
    {
        for (int i = 0; i < lockedInteractions.Count; i++)
        {
            if (loadInt == lockedInteractions[i])
            {
                UnockInteraction(loadInt);
            }
        }



        int savedInteract = currentInteraction;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inspect pI = player.GetComponent<Inspect>();

        pI.TriggerDeinspection();
        currentInteraction = loadInt;
        pI.TriggerInspection(false);

        currentInteraction = savedInteract;
    }

    public void LockInteraction(int interact)
    {

        lockedInteractions.Add(interact);
        lockedInteractions = lockedInteractions.Distinct().ToList();
    }

    public void UnockInteraction(int interact)
    {
        lockedInteractions.Remove(interact);
        lockedInteractions = lockedInteractions.Distinct().ToList();

        //multiple of the same variable in a list, delete

    }
    void Door(Inspect insp)
    {
        if (beingInspected && area != SceneManager.GetActiveScene().buildIndex)
        {
            SceneManager.LoadScene(area);
        }

    }

    void HideText(Inspect insp)
    {
       if (maxHealthUp)
        {
            textBox.Swap(0);
        }
    }

    private void OnEnable()
    {
        Inspect.OnInspectEvent += SetText;
        Inspect.DeInspectEvent += HideText;
        Inspect.DeInspectEvent += DeinspectActions;

        SceneManager.sceneLoaded += OnSceneLoaded;

        Inspect.OnInspectDoor += Door;
        Respawn.OnRespawnEvent += ResetUpgrades;
        Respawn.OnRespawnEvent += ResetText;

    }

    void ResetText(Respawn resp)
    {
        textLine = 0;
        currentInteraction = 0;
    }

    void ResetUpgrades(Respawn resp)
    {
        if (UpgradeClaimed == true)
        {
            UpgradeClaimed = false;
        }
    }

}
