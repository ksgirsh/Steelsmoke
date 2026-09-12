using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class TextBoxController : MonoBehaviour
{
    [field:SerializeField] public GameObject box { get; private set; }

    [SerializeField] Image[] images;
    [SerializeField] TextMeshProUGUI textD;
    [SerializeField] TextMeshProUGUI textI;


    [SerializeField] float fadeInTime;


    [field:SerializeField] public GameObject item { get; private set; }
    [field:SerializeField] public Image portrait;
    [field: SerializeField] public Image dialogueIcon;
    [SerializeField] GameObject dialogue;

    [SerializeField] GameObject rustburnUpgrade;
    [SerializeField] GameObject[] rustburnFadeIns;
    [SerializeField] GameObject[] rustburnSecondFadeIns;


    //
    public string writer;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;

    [field:SerializeField] public bool typing { get; private set; }
    
    //if this is set to private it doesnt work.                       what the fuck??? why??? this fucking shit makes no sense????? I can understand the rest of my convoluted dialogue system, but storing this color information is parsecs beyond whatever the fuck i think im doing. at least it works. for now
    [SerializeField] List<Color> startingColors;

    [SerializeField] TextMeshProUGUI[] ynOptions;

    private Coroutine currentCor;

    private void OnEnable()
    {
        Inspect.OnInspectEvent += ShowBox;
        Inspect.DeInspectEvent += HideBox;

        Inspect.OnMedallionUpgrade += ShowUpgrade;
        Respawn.OnRespawnEvent += OnRespawnTextControl;
        box.SetActive(false);
        HideYesNo();

        currentCor = StartCoroutine(EmptyCorout());
    }

    void OnRespawnTextControl(Respawn resp)
    {
        HideYesNo();
    }


    IEnumerator EmptyCorout()
    {
        yield return null;
    }

    public void ShowYesNo()
    {
        foreach (TextMeshProUGUI txt in ynOptions)
        {
            txt.gameObject.SetActive(true);
            
        }
    }
    public void SetIcon(Sprite icon)
    {
        dialogueIcon.sprite = icon;
    }

    public void CharacterSay(Sprite portrait, string line, AudioClip[] dialogueSounds = null)
    {
        typing = false;
        string prefix = " > ";

        //first saying, shows box
        if (!item.activeSelf)
        {
            
            textI.text = prefix + writer;
        }

        //type out
        writer = line;
        ShowImgBox(portrait, dialogueSounds);
    }


    public void HideYesNo()
    {
        foreach (TextMeshProUGUI txt in ynOptions)
        {
            txt.gameObject.SetActive(false);
        }
    }

    public void HighlightText(int index)
    {
        
        string prefix = ">";

        string textWrite = ynOptions[index].text;

        if (!textWrite.Contains(prefix))
        {

            ynOptions[index].text = prefix + textWrite;

        }

    }

    public void DeHighlightText(int index)
    {

        string prefix = ">";

        string textWrite = ynOptions[index].text;

        if (textWrite.Contains(prefix))
        {
            
            ynOptions[index].text = textWrite.Replace(prefix, "");

        }

    }
    // Start is called before the first frame update
    void Start()
    {
        //  writer = textM.text;
        dialogue = UIManager.instance.dialogueBox;
        box = UIManager.instance.textContainer;
    }

    void OnDestroy()
    {
        Inspect.OnInspectEvent -= ShowBox;
        Inspect.DeInspectEvent -= HideBox;

        Inspect.OnMedallionUpgrade -= ShowUpgrade;
        Respawn.OnRespawnEvent -= OnRespawnTextControl;

        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (typing == true && Input.GetKeyDown(KeyCode.E))
        {
            if (item.activeSelf)
            {
                StartCoroutine(StopTypingSetText(textI));
            } else
            {
                StartCoroutine(StopTypingSetText(textD));
            }
        }
    }

    void ShowBox(Inspect insp)
    {

      //  Debug.Log("AAAAAAAAAAAAHHHHHHHHHH!!!!!!!");

        //if img box isnt active, show dialogue box

        if (dialogue.activeSelf)
        {
            if (box.activeSelf == false)
            {
                box.SetActive(true);
                StartCoroutine(PlayAnimation(textD, false));


            }



            if (currentCor != null)
            {
                Debug.Log("test");
                currentCor = StartCoroutine(TypewriterTMP(textD));
            }
            

            



        }


    }

    void ShowUpgrade(Inspect insp)
    {
        rustburnUpgrade.SetActive(true);
        box.SetActive(false);
        StartCoroutine("PlayRustburnAnimation");
    }

    public void Swap(int dir)
    {
        switch (dir)
        {

            case 0:

                item.SetActive(false);
                dialogue.SetActive(true);

                break;

            case 1:

                item.SetActive(true);
                dialogue.SetActive(false);

                break;

            default:

                item.SetActive(false);
                dialogue.SetActive(true);

                break;

        }

            


    }


    public void ShowImgBox(Sprite boxImg, AudioClip[] dialogueSounds = null)
    {
        dialogue.SetActive(false);
        item.SetActive(true);

        portrait.sprite = boxImg;

        if (box.activeSelf == false)
        {
            box.SetActive(true);
            StartCoroutine(PlayAnimation(textI, true));


        }
        currentCor = StartCoroutine(TypewriterTMP(textI, dialogueSounds));

    }

    public void HideBox(Inspect insp)
    {
        box.SetActive(false);
        rustburnUpgrade.SetActive(false);
    }

    public IEnumerator PlayAnimation(TextMeshProUGUI textRef, bool portraitEnabled)
    {
        // typing = true;


        for (float t = 0f; t < fadeInTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeInTime;
         //   Debug.Log(normalizedTime);
            Vector2 scale = new Vector2(Mathf.Lerp(0.9f, 1, Ease(normalizedTime)), Mathf.Lerp(0.9f, 1, Ease(normalizedTime)));

            foreach (Image img in images)
            {
               
                img.color = new Color(1, 1, 1, normalizedTime);
                img.rectTransform.localScale = scale;
            }

            if (portraitEnabled)
            {
                portrait.color = new Color(1, 1, 1, normalizedTime);
                portrait.rectTransform.localScale = scale;
            }

            textRef.color = new Color(1, 1, 1, normalizedTime);
           // text.rectTransform.localScale = scale;

            yield return null;


          
        }

       // typing = false;
    }


    public IEnumerator PlayRustburnAnimation()
    {
        typing = true;
        float rustburnFadeIn = fadeInTime * 4;






        for (int i = 0; i < rustburnFadeIns.Length; i++)
        {
            Image img = rustburnFadeIns[i].GetComponent<Image>();

            if (img != null)
            {
                Color startColor = img.color;
                img.color = new Color(startColor.r, startColor.b, startColor.g, 0);

            }
            else
            {
                TextMeshProUGUI textRef = rustburnFadeIns[i].GetComponent<TextMeshProUGUI>();
                Color startColor = textRef.color;
                textRef.color = new Color(startColor.r, startColor.b, startColor.g, 0);
            }

        }


        for (int i = 0; i < rustburnSecondFadeIns.Length; i++)
        {
            Image img = rustburnSecondFadeIns[i].GetComponent<Image>();

            if (img != null)
            {

                Color startColor = img.color;
                img.color = new Color(startColor.r, startColor.b, startColor.g, 0);

            }
            else
            {
                TextMeshProUGUI textRef = rustburnSecondFadeIns[i].GetComponent<TextMeshProUGUI>();
                Color startColor = textRef.color;
           
                textRef.color = new Color(startColor.r, startColor.b, startColor.g, 0);
            }

        }



        for (float t = 0f; t < rustburnFadeIn; t += Time.deltaTime)
        {
            float normalizedTime = t / rustburnFadeIn;

            Vector2 scale = new Vector2(Mathf.Lerp(0.9f, 1, Ease(normalizedTime)), Mathf.Lerp(0.9f, 1, Ease(normalizedTime)));


            //looks for images first, then fades in text

            for (int i = 0; i < rustburnFadeIns.Length; i++)
            {
                Image img = rustburnFadeIns[i].GetComponent<Image>();

                if (img != null)
                {
                    Color startColor = img.color;
                    img.color = new Color(startColor.r, startColor.b, startColor.g, normalizedTime);

                } else
                {
                    TextMeshProUGUI textRef = rustburnFadeIns[i].GetComponent<TextMeshProUGUI>();
                    Color startColor = textRef.color;



                    textRef.color = new Color(startColor.r, startColor.b, startColor.g, normalizedTime);
                }
               
            }
            

           
            // text.rectTransform.localScale = scale;

            yield return null;

          

        }


        //Second fade in

        yield return new WaitForSeconds(1.8f);

        rustburnFadeIn = fadeInTime * 2;

       
        for (int i = 0; i < rustburnSecondFadeIns.Length; i++)
        {

            Image img = rustburnSecondFadeIns[i].GetComponent<Image>();
            TextMeshProUGUI textRef = rustburnSecondFadeIns[i].GetComponent<TextMeshProUGUI>();

            if (img != null)
            {
                startingColors.Add(img.color);

            } else if (textRef != null)
            {
                //TextMeshProUGUI textRef = rustburnSecondFadeIns[i].GetComponent<TextMeshProUGUI>();
                startingColors.Add(textRef.color);
            }


        }

        for (float t = 0f; t < rustburnFadeIn; t += Time.deltaTime)
        {
            float normalizedTime = t / rustburnFadeIn;
            //   Debug.Log(normalizedTime);

            for (int i = 0; i < rustburnSecondFadeIns.Length; i++)
            {
                Image img = rustburnSecondFadeIns[i].GetComponent<Image>();

                if (img != null)
                {
                   
                    img.color = new Color(startingColors[i].r, startingColors[i].b, startingColors[i].g, normalizedTime);

                }
                else
                {
                    TextMeshProUGUI textRef = rustburnSecondFadeIns[i].GetComponent<TextMeshProUGUI>();
                    textRef.color = new Color(startingColors[i].r, startingColors[i].b, startingColors[i].g, normalizedTime);
                }

            }

            // text.rectTransform.localScale = scale;

            yield return null;



        }

        typing = false;

    }


    float Ease(float progress)
    {
        //maps the progress between -pi/2 to pi/2

        progress = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, progress);

        //returns a value between -1 and 1 
        progress = Mathf.Sin(progress);

        //scale the sine between 0 & 1
        progress = (progress / 2f) + .5f;

        return progress;
    }


    IEnumerator TypewriterTMP(TextMeshProUGUI textRef, AudioClip[] dialogueSounds = null)
    {
        yield return new WaitForSeconds(0.01f);
        typing = true;

        textRef.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        int textLength = writer.Length;

        textRef.text = " >  ";
        
        foreach (char c in writer)
        {
            string currString = textRef.text;

            if (currString != "")
            {
                int currLength = currString.Length;

                if (currLength > 0)
                {
                    textRef.text = currString.Substring(0, currLength - leadingChar.Length);
                }
            }
         


            

            textRef.text += c;
            textRef.text += leadingChar;


            if (dialogueSounds != null)
            {
                SoundFXManager.instance.PlayRandomSoundEffectClip(dialogueSounds, transform.position, 1f);
            }



            yield return new WaitForSeconds(timeBtwChars);

        }

        if (leadingChar != "")
        {
            string prefix = " > ";

            textRef.text = prefix + writer.Substring(0, (textLength - leadingChar.Length) + 1);
        }

        typing = false;
    }

    IEnumerator StopTypingSetText(TextMeshProUGUI textRef)
    {
        string prefix = " > ";
        StopCoroutine(currentCor);
        yield return new WaitForSeconds(timeBtwChars);
        textRef.text = prefix + writer;
        typing = false;
    }
}
