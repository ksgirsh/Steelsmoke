using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class Cutscene : MonoBehaviour
{
    //might change later
    [SerializeField] bool CallOnInit;
    [SerializeField] UnityEvent[] cutsceneActions;

    [SerializeField] MonoBehaviour[] disableScripts;
    [SerializeField] Sprite[] spritesToSet;

    [SerializeField] CStatic cStatic;
    [SerializeField] GameObject fullScreenFlash;
    

    TextBoxController tB;

    public bool start = false;
    public int currentAction = 0;
    bool delay = false;






    // Start is called before the first frame update
    void Start()
    {
        tB = UIManager.instance.textBox.GetComponent<TextBoxController>();
        if (CallOnInit)
        {
            PlayCutscene();
        }

    }




    public void MovePlayerTo(Transform position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerSceneController playerS = player.GetComponent<PlayerSceneController>();
        playerS.SceneMovePlayerTo(position);
    }

    public void FadeToScene(int sceneANumber)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerSceneController playerS = player.GetComponent<PlayerSceneController>();

        if (playerS != null)
        {
            playerS.FadeSceneTransition(sceneANumber);
        }
        

    }

    void SetCDur(float set)
    {
        cStatic.talkTime = set;
    }

    public void PulseColor()
    {

        GameObject canv = GameObject.FindGameObjectWithTag("MainCanvas");

        GameObject flash = Instantiate(fullScreenFlash, canv.transform);
        Image img = flash.GetComponent<Image>();
        img.color = Color.black;

        StartCoroutine(FadeInImg(1.2f, img, 1));

    }

    public void CSay(string text)
    {
        StartCoroutine(cStatic.CWrite(" ", 0f));
        StartCoroutine(cStatic.CWrite(text, 4f));
        Delay(cStatic.talkTime);
    }

    public void SetPlayerUIActive(bool enable)
    {
        //toggles things
        GameObject playUI = UIManager.instance.playerUI;
        playUI.SetActive(enable);
    }

    /*
    public void SetPlayerUITransparent()
    {
        GameObject playUI = UIManager.instance.playerUI;
        SetTransparent(playUI);
    }
    */
    public void FadeInPlayerUI()
    {
        GameObject playUI = UIManager.instance.playerUI;
        FadeInObj(playUI);

        foreach (GameObject effect in UIManager.instance.medallionEffects)
        {
            effect.SetActive(true);
        }
    }

    public void PlaySong(AudioClip clip)
    {
        SoundFXManager.instance.musicCont.music.clip = clip;
        SoundFXManager.instance.musicCont.music.Play();
    }
    /*
    public void SetTransparent(GameObject obj)
    {
        List<Transform> objectsToFade = new List<Transform>();
        objectsToFade.Add(obj.transform);
        AddDescendants(obj.transform, objectsToFade);
        //get all children with an image

        List<Image> spritesToFade = new List<Image>();
        foreach (Transform rendCheck in objectsToFade)
        {
            if (rendCheck.gameObject.activeSelf == true)
            {
                if (rendCheck.gameObject.GetComponent<Image>() != null)
                {
                    Image imgElem = rendCheck.GetComponent<Image>();
                    spritesToFade.Add(imgElem);
                }

                if (rendCheck.gameObject.GetComponent<TextMeshProUGUI>() != null)
                {
                    TextMeshProUGUI txt = rendCheck.gameObject.GetComponent<TextMeshProUGUI>();
                    Color colorToSet = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
                    txt.color = colorToSet;
                }
            }

        }

        //for each image, fade 

        foreach (Image img in spritesToFade)
        {
            //set opacity

            Color colorToSet = new Color(img.color.r, img.color.g, img.color.b, 0f);
            img.color = colorToSet;

            //cache initial opacities to save for later?
        }
    }
    */
    public void FadeInObj(GameObject obj)
    {
        List<Transform> objectsToFade = new List<Transform>();
        objectsToFade.Add(obj.transform);
        AddDescendants(obj.transform, objectsToFade);
        //get all children with an image

        List<Image> spritesToFade = new List<Image>();
        foreach (Transform imageCheck in objectsToFade)
        {
            if (imageCheck.gameObject.activeSelf == true)
            {
                if (imageCheck.gameObject.GetComponent<Image>() != null)
                {
                    Image imgElem = imageCheck.GetComponent<Image>();
                    spritesToFade.Add(imgElem);
                }
            }

            if (imageCheck.gameObject.GetComponent<TextMeshProUGUI>() != null)
            {
                TextMeshProUGUI txt = imageCheck.gameObject.GetComponent<TextMeshProUGUI>();
                StartCoroutine(FadeInText(1f, txt));
            }

        }

        //for each image, fade 

        foreach (Image img in spritesToFade)
        {
            //find way to fix these magic numbers and add more customizablity
            
            StartCoroutine(FadeInImg(1f, img));
        }
       
    }

    IEnumerator FadeInImg(float dur, Image img, int direction = 0, float fadeTo = 1f)
    {
        //Debug.Log((0f + direction * fadeTo) + " " + ((1f * fadeTo) - (direction * fadeTo)));
        Color initColor = img.color;


        float target = (1f * fadeTo) - (direction * fadeTo);

        if (fadeTo == 1 && direction == 0)
        {
            //fades to initial alpha instead of 1.
            target = initColor.a;
           // Debug.Log("Set Target Alpha: " + target);
        }

        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            //if direction is 1 then it fades out
            float alpha = Mathf.Lerp(0f + direction * fadeTo, target, i);

            Color newImgColor = new Color(initColor.r, initColor.g, initColor.b, alpha);

            img.color = newImgColor;

            //Debug.Log("Fading");
            yield return null;
        }


    }

    IEnumerator FadeInText(float dur, TextMeshProUGUI text, int direction = 0)
    {
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f + direction, 1f - direction, i);

            Color newImgColor = new Color(text.color.r, text.color.g, text.color.b, alpha);

            text.color = newImgColor;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (start && !delay)
        {
            DoCutscene();
        }


    }


    public void PlayCutscene()
    {
        start = true;
        Debug.Log("Started Cutscene");

    }

    void DoCutscene()
    {
        if (currentAction < cutsceneActions.Length)
        {
            if (tB.box.activeSelf == false)
            {
                cutsceneActions[currentAction].Invoke();
                currentAction++;

            }
            else if (Input.GetKeyDown(KeyCode.E) && tB.typing == false)
            {
                cutsceneActions[currentAction].Invoke();
                currentAction++;
            }
        }


    }

    void EndCutscene()
    {
        delay = false;
    }

    public void Delay(float time)
    {
        delay = true;
        StartCoroutine(EndDelay(time));
    }

    IEnumerator EndDelay(float del)
    {
        yield return new WaitForSeconds(del);
        delay = false;
    }
 
    public void Move(float duration, Transform startPos, Vector3 targetPos)
    {
        Vector3 startPosition = startPos.position;
        StartCoroutine(MoveOverTime(duration, startPosition, targetPos));
    }

    IEnumerator MoveOverTime(float dur, Vector3 startPos, Vector3 targetPos)
    {
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float t = i / dur;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    public void ToggleScripts()
    {

        for (int i = 0; i < disableScripts.Length; i++)
        {
            disableScripts[i].enabled = !disableScripts[i].enabled;
        }
    }

    public void PlayAnimation(GameObject obj, string animationState)
    {
        obj.GetComponent<Animator>().CrossFade(animationState, 0, 0);
    }

    public void SetSprite(GameObject obj, int sprite, float delay)
    {
        StartCoroutine(Sprite(obj, sprite, delay));
    }


    IEnumerator Sprite(GameObject obj, int sprite, float del)
    {
        yield return new WaitForSeconds(del);
        obj.GetComponent<SpriteRenderer>().sprite = spritesToSet[sprite];
    }

    private void AddDescendants(Transform parent, List<Transform> list)
    {
        foreach (Transform child in parent)
        {
            list.Add(child);
            AddDescendants(child, list);
        }
    }
}
