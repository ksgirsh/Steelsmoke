using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public class Inspect : MonoBehaviour
{
    [SerializeField] float textAppear;
    [SerializeField] float textDetect;

    [SerializeField] LayerMask textLayer;

    [SerializeField] bool inspectable;
    [field:SerializeField] public bool inspecting { get; private set; }


    [SerializeField] GameObject textObj = null;
    [SerializeField] GameObject respawnController;
    bool respawnEnabled;



    [SerializeField] MonoBehaviour[] scripts;

    public static event Action<Inspect> OnInspectEvent;
    public static event Action<Inspect> OnInspectDoor;
    public static event Action<Inspect> DeInspectEvent;

    public static event Action<Inspect> OnMedallionUpgrade;

    [SerializeField] TextBoxController textBox;
    [SerializeField] Sprite dialogueIcon;

    [SerializeField] Health heal;
    [SerializeField] RunAndJump rj;

    [SerializeField] GameObject[] dontDestroyOnLoad;

    [field:SerializeField] public bool choicePresented { get; private set; }

    [SerializeField] bool yesHighlighted = true;

    // Start is called before the first frame update
    void Start()
    {
        if (dontDestroyOnLoad.Length > 0)
        {
            foreach (GameObject obj in dontDestroyOnLoad)
            {
                DontDestroyOnLoad(obj);
            }
        }


        textBox = UIManager.instance.textBox.GetComponent<TextBoxController>();

        //GameObject textObj = CheckForText(textDetect);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        textBox = UIManager.instance.textBox.GetComponent<TextBoxController>();
    }

    public void ShowRespawnText()
    {
   

        respawnEnabled = true;

        ItemInspectable item = respawnController.transform.parent.GetComponent<ItemInspectable>();

        inspecting = true;

        if (item != null)
        {
            item.beingInspected = true;
        }

        if (OnInspectEvent != null)
        {
            // Debug.Log("Called Event");
            OnInspectEvent(this);

            // iterate through choiceLoc[] to check if text line matches

            for (int i = 0; i < item.choiceLoc.Length; i++)
            {
                if (item.interactions[item.currentInteraction].textIndex[item.textLine] == item.choiceLoc[i])
                {
                    choicePresented = true;

                }
                else if (choicePresented)
                {
                    choicePresented = false;
                    textBox.HideYesNo();
                }
            }

            if (Time.timeScale != 0)
            {
                Time.timeScale = 0.5f;
            }

            textObj = respawnController;

        }

    }


    // Update is called once per frame
    void Update()
    {
       
        if (textObj != null && textObj != respawnController)
        {
           // GameObject textObj = CheckForText(textDetect);
            TextOpacity(textObj);

        }


        if (Input.GetKeyDown(KeyCode.E) && inspecting == true && textBox.typing == false)
        {

            ItemInspectable item = textObj.transform.parent.GetComponent<ItemInspectable>();

        
            if (choicePresented && yesHighlighted == true)
            {
            
                item.YesChoice();
                if (respawnEnabled)
                {
                    //Deinspecting process

                    

                    DeInspectEvent(this);
                    inspecting = false;
                    choicePresented = false;

                    if (item != null)
                    {
                        item.beingInspected = false;
                    }
                }

            }
            else if (choicePresented)
            {
        
                item.NoChoice();
                if (respawnEnabled)
                {
                    QuitGame();
                }
                //Deinspecting process

                inspecting = false;
                ToggleScripts();

                DeInspectEvent(this);
                choicePresented = false;

                if (item != null)
                {
                    item.beingInspected = false;
                }
            }

            if (textBox.typing == false && heal.currentHealth != 0 && !respawnEnabled)
            {
                if (item.textLine >= ((item.interactions[item.currentInteraction].textIndex.Length)) || item.UpgradeClaimed == true || item.gameObject.tag == "MedallionPickup")
                {
                   
                    //Deinspecting process
                    inspecting = false;
                    ToggleScripts();

                    
                    if ((item.interactions.Length - 1) > (item.currentInteraction))
                    {
                        item.currentInteraction++;
                    }

                    //sets to negative 1 so that when inspected, starts at 0
                    item.textLine = 0;

                    textBox.HideYesNo();
                    DeInspectEvent(this);

                    if (item != null)
                    {
                        item.beingInspected = false;
                    }

                    //Deinspecting medallion -- activates upgrade
                    if (item.gameObject.tag == "MedallionPickup")
                    {
                        item.gameObject.SetActive(false);
                        rj.MedallionUpgrade();
                    }
                }
                else if (!respawnEnabled)
                {
                    //Advancing to next text line
                    OnInspectEvent(this);
                    for (int i = 0; i < item.choiceLoc.Length; i++)
                    {
                        if (item.interactions[item.currentInteraction].textIndex[item.textLine] == item.choiceLoc[i])
                        {
                            choicePresented = true;
                        }
                        else if (choicePresented)
                        {
                            choicePresented = false;
                            textBox.HideYesNo();
                        }

                    }

                    item.textLine++;

                    // iterate through choiceLoc[] to check if text line matches
                   
                }


                //If its the last line of dialogue, and the inspectable is a max health upgrade, increase max life.
                if (item.textLine >= (item.text.Length) && item.maxHealthUp == true && !item.UpgradeClaimed)
                {
                    heal.AddHeart();
                    item.UpgradeClaimed = true;
                }
          

                return;
            }
            respawnEnabled = false;
        }



        //Inspect
        if (Input.GetKeyDown(KeyCode.E) && inspectable == true && !inspecting && heal.currentHealth != 0 && !rj.sitting)
        {
            TriggerInspection(true);

        }

        //Text Box Control -- Choice Control
        if (choicePresented == true)
        {
            textBox.ShowYesNo();

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                yesHighlighted = true;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                yesHighlighted = false;
            }

            //Player presses Y

            if (Input.GetKeyDown(KeyCode.Y))
            {
                yesHighlighted = true;

                ItemInspectable item = textObj.transform.parent.GetComponent<ItemInspectable>();
                item.YesChoice();
                if (respawnEnabled)
                {

                    inspecting = false;

                    

                    DeInspectEvent(this);
                    if (item != null)
                    {
                        item.beingInspected = false;
                    }
                    choicePresented = false;
                    respawnEnabled = false;

                    textBox.HideYesNo();
                    return;
                }

                
                if (item.textLine >= (item.text.Length))
                {
                    //Deinspecting process
                    inspecting = false;
                    ToggleScripts();

                    DeInspectEvent(this);
                    if (item != null)
                    {
                        item.beingInspected = false;
                    }

                    
                    choicePresented = false;
                    textBox.HideYesNo();

                }
                else
                {
                    //Advancing to next text line
                    OnInspectEvent(this);
                    for (int i = 0; i < item.choiceLoc.Length; i++)
                    {
                        if (item.interactions[item.currentInteraction].textIndex[item.textLine] == item.choiceLoc[i])
                        {
                            choicePresented = true;
                        }
                        else if (choicePresented)
                        {
                            choicePresented = false;
                            textBox.HideYesNo();
                        }

                    }
                    item.textLine++;

                    // iterate through choiceLoc[] to check if text line matches



                }

                choicePresented = false;


            }

            // Player pressers N
            if (Input.GetKeyDown(KeyCode.N))
            {
                ItemInspectable item = textObj.transform.parent.GetComponent<ItemInspectable>();
                item.NoChoice();
                if (respawnEnabled)
                {

                    inspecting = false;

                    

                    DeInspectEvent(this);
                    if (item != null)
                    {
                        item.beingInspected = false;
                    }

                    choicePresented = false;
                    respawnEnabled = false;

                    textBox.HideYesNo();

                    QuitGame();

                    return;
                }



                if (item.textLine >= (item.text.Length))
                {
                    //Deinspecting process
                    inspecting = false;
                    ToggleScripts();

                    
                    DeInspectEvent(this);

                    if (item != null)
                    {
                        item.beingInspected = false;
                    }

                    choicePresented = false;

                }
                else
                {
                    //Advancing to next text line
                    OnInspectEvent(this);
                    for (int i = 0; i < item.choiceLoc.Length; i++)
                    {
                        if (item.interactions[item.currentInteraction].textIndex[item.textLine] == item.choiceLoc[i])
                        {
                            choicePresented = true;
                        }
                        else if (choicePresented)
                        {
                            choicePresented = false;
                            textBox.HideYesNo();
                        }

                    }
                    item.textLine++;

                    // iterate through choiceLoc[] to check if text line matches



                }

                choicePresented = false;

                return;

            }

            if (yesHighlighted)
            {
                textBox.HighlightText(1);
                textBox.DeHighlightText(0);

            }
            else
            {
                textBox.HighlightText(0);
                textBox.DeHighlightText(1);
            }
        }

  


    }

    public void TriggerInspection(bool checkLocked)
    {
            InteractionCheck();

            inspecting = true;
            ToggleScripts();

            ItemInspectable item = textObj.transform.parent.GetComponent<ItemInspectable>();

            if (item.immediateYes == true)
            {
                item.YesChoice();

                inspecting = false;
                ToggleScripts();
                return;
            }


            if (item != null)
            {
                item.beingInspected = true;
            }

            textBox.SetIcon(dialogueIcon);

            if (item.gameObject.tag == "MedallionPickup")
            {
                if (OnMedallionUpgrade != null)
                {            
                    OnMedallionUpgrade(this);
                }

            } else if (item != null && item.door == true)
            {

                if (OnInspectDoor != null)
                {
                    this.transform.position = item.travelPos;
                    OnInspectDoor(this);

                    inspecting = false;
                    ToggleScripts();

                    DeInspectEvent(this);

                    if (item != null)
                    {
                        item.beingInspected = false;
                    }
                }

            } else
            {
                
                if (item.textLine >= (item.text.Length - 1) && item.maxHealthUp == true && item.UpgradeClaimed == true)
                {
                    item.textLine = (item.text.Length - 2);
                }

                if (OnInspectEvent != null)
                {
                    // Debug.Log("Called Event");
                    OnInspectEvent(this);


                    // if the last line of a inspectable is a choice, then reenable that choice.
                    if (item.choiceLoc.Length != 0)
                    {
                        if (item.choiceLoc[(item.choiceLoc.Length - 1)] == (item.text.Length - 1) && item.textLine >= item.text.Length)
                        {
                            item.textLine = item.choiceLoc[(item.choiceLoc.Length - 1)];
                        }
                    }


                    if (item.interactions[item.currentInteraction].textIndex.Length > item.textLine)
                    {
                        // iterate through choiceLoc[] to check if text line matches
                        for (int i = 0; i < item.choiceLoc.Length; i++)
                        {   
                        

                            if (item.interactions[item.currentInteraction].textIndex[item.textLine] == item.choiceLoc[i])
                            {
                                choicePresented = true;

                            }
                            else if (choicePresented)
                            {
                                choicePresented = false;
                                textBox.HideYesNo();
                            }
                        }

                    }
                    
                    item.textLine++;
                    
  

                }
            }
           

            return;
    }

    void InteractionCheck()
    {
        ItemInspectable item = textObj.transform.parent.GetComponent<ItemInspectable>();

        for (int i = 0; i < item.lockedInteractions.Count; i++)
        {
            if (item.currentInteraction == item.lockedInteractions[i])
            {
                Debug.Log("Locked Interaction detected, skipping");

                if (item.currentInteraction != item.interactions.Length)
                {
                    item.currentInteraction++;

                } else
                {
                    //last interaction in the list is locked
                    item.currentInteraction--;
                }
           
            }
        }

    }

    public void TriggerDeinspection()
    {
        ItemInspectable item = textObj.transform.parent.GetComponent<ItemInspectable>();
        inspecting = false;
        ToggleScripts();

        
        if ((item.interactions.Length - 1) > (item.currentInteraction))
        {
            item.currentInteraction++;
        }

        //sets to negative 1 so that when inspected, starts at 0
        item.textLine = 0;

        textBox.HideYesNo();
        DeInspectEvent(this);

        if (item != null)
        {
            item.beingInspected = false;
        }
    }

    void CheckForText(float distance)
    {

        //Debug.Log("looking...");
        Collider2D textColl = Physics2D.OverlapCircle(this.transform.position, distance, textLayer);
        
        if (textColl != null)
        {
            textObj = textColl.gameObject;
            //Debug.Log("found text");
        }
    }

    void TextOpacity(GameObject textP)
    {
        SpriteRenderer textRend = textP.GetComponent<SpriteRenderer>();

        Vector2 dist = textP.transform.parent.transform.position - this.transform.position;
        //Debug.Log(distX);

        float opac = Mathf.InverseLerp(textAppear, 0, dist.magnitude);
        //Debug.Log(opac);
        textRend.color = new Color(1, 1, 1, opac);

    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 10 && heal.currentHealth != 0)
        {
            inspectable = true;

            textObj = coll.gameObject.transform.GetChild(0).gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.layer == 10 && heal.currentHealth != 0)
        {
            inspectable = false;
           
        }
    }


    public void ToggleScripts()
    {
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = !inspecting;
        }
    }

    void OnGizmosDrawSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, textAppear);
    }

    public void QuitGame()
    {
        Time.timeScale = 0f;
        Application.Quit();
     
    }


    
}
