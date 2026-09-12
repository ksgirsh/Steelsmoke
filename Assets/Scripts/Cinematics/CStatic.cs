using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class CStatic : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cText;
    public float talkTime = 2.5f;

    public Image[] thingsToFade;
    [SerializeField] GameObject fullScreenFlash;

    [SerializeField] AudioClip cSaySFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.N))
        {
            CTrigger(3f);
            SetCDur(5f);
            CSay("ABATE.");

        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            EndStatic();
        }
    }

    public void LoadScene(int sceneANumber)
    {
        SceneManager.LoadScene(sceneANumber);
    }

    public IEnumerator CWrite(string text, float initDelay)
    {
        //time per character is totalWritedur / total amt of characters in line
        string prefix = "";
        float timePerChar = ((talkTime) / text.Length);
        string write = prefix;

        char[] characters = text.ToCharArray();

        yield return new WaitForSeconds(initDelay);

        SoundFXManager.instance.PlaySoundEffectClip(cSaySFX, transform.position, 1f);

        for (int i = 0; i < text.Length; i++)
        {
            write += characters[i];
            cText.text = write;
            yield return new WaitForSeconds(timePerChar);
        }


    }

    public void SetCDur(float set)
    {
        talkTime = set;
    }

    IEnumerator FadeInImg(float dur, Image img, int direction = 0, float fadeTo = 1f)
    {
        float targetOpac = ((1f * fadeTo) - (direction * fadeTo));
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            //if direction is 1 then it fades out
            float alpha = Mathf.Lerp(0f + direction * fadeTo, targetOpac, i);

            Color newImgColor = new Color(img.color.r, img.color.g, img.color.b, alpha);

            img.color = newImgColor;
            yield return null;
        }
        Color finalColor = new Color(img.color.r, img.color.g, img.color.b, targetOpac);
        img.color = finalColor;

    }

    IEnumerator FadeInText(float dur, TextMeshProUGUI text, int direction = 0)
    {
        float targetOpac = (1f - direction);
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f + direction, targetOpac, i);

            Color newImgColor = new Color(text.color.r, text.color.g, text.color.b, alpha);

            text.color = newImgColor;
            yield return null;
        }
        Color finalColor = new Color(text.color.r, text.color.g, text.color.b, targetOpac);
        text.color = finalColor;
    }

    public void FlashColor(Color col)
    {
        GameObject canv = GameObject.FindGameObjectWithTag("MainCanvas");

        GameObject flash = Instantiate(fullScreenFlash, canv.transform);
        Image img = flash.GetComponent<Image>();
        img.color = col;

        StartCoroutine(FadeInImg(1f, img, 1));


    }

    public void CSay(string text)
    {
        StartCoroutine(CWrite(" ", 0f));
        StartCoroutine(CWrite(text, 4f));
       

    }


    public void EndStatic()
    {
        SoundFXManager.instance.PlaySoundEffectClip(cSaySFX, transform.position, 1f);
        FlashColor(Color.black);
        LoadScene(2);
        MovePlayer(new Vector3(13.42f, -5.6f, 0f));        
        Image[] statChildren = gameObject.GetComponentsInChildren<Image>();
        foreach (Image stc in statChildren)
        {
            StartCoroutine(FadeInImg(1f, stc, 1));
        }


        Image stat = gameObject.GetComponent<Image>();
        TextMeshProUGUI txt = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(FadeInImg(1f, stat, 1));
        StartCoroutine(FadeInText(1f, gameObject.GetComponentInChildren<TextMeshProUGUI>(), 1));
    }

    public void CTrigger(float dur)
    {


        FlashColor(Color.white);
        StartCoroutine(FadeInImg(dur, gameObject.GetComponent<Image>()));
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            StartCoroutine(FadeInImg(dur, img, 0, 0.75f));
        }


        StartCoroutine(FadeInText(dur, gameObject.GetComponentInChildren<TextMeshProUGUI>()));

        //slow down player
        //play spooky noise




    }

    void MovePlayer(Vector3 loc)
    {
        GameObject playr = GameObject.FindGameObjectWithTag("Player");
        Transform playerT = playr.GetComponent<Transform>();
        playerT.position = loc;
    }

}