using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class PlayerSceneController : MonoBehaviour
{
    
    [SerializeField] List<GameObject> playersQuery;
    [SerializeField] List<GameObject> mainCam;
    [SerializeField] List<GameObject> mainUI;

    [SerializeField] GameObject fullScreenColor;
    private Vector3 playerTransportLoc;
    private Color fadeToColor = Color.black;
    //[SerializeField] GameObject WFAmbienceCheck;

    RunAndJump rj;
    // Start is called before the first frame update
    void Awake()
    {
        Query();
    }

    void Start()
    {
        rj = GetComponent<RunAndJump>();
        playerTransportLoc = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void Query()
    {
        playersQuery = (GameObject.FindGameObjectsWithTag("Player")).ToList();
        mainCam = (GameObject.FindGameObjectsWithTag("MainCamera")).ToList();
        mainUI = (GameObject.FindGameObjectsWithTag("MainCanvas")).ToList();

        if (playersQuery.Count > 1)
        {
            for (int i = 0; i < playersQuery.Count; i++)
            {
                if (playersQuery.Count != 1)
                {
                    Destroy(playersQuery[(playersQuery.Count - 1)].transform.parent.gameObject);
                    playersQuery.Remove(playersQuery[(playersQuery.Count - 1)].transform.parent.gameObject);
                }
            }

            for (int i = 0; i < mainCam.Count; i++)
            {
                if (mainCam.Count != 1)
                {
                    Destroy(mainCam[(mainCam.Count - 1)]);
                    mainCam.Remove(mainCam[(mainCam.Count - 1)]);
                }
            }

            for (int i = 0; i < mainUI.Count; i++)
            {
                if (mainUI.Count != 1)
                {
                    Destroy(mainUI[(mainUI.Count - 1)]);
                    mainCam.Remove(mainUI[(mainUI.Count - 1)]);
                }
            }

        }
    }

    public void SceneMovePlayerTo(Transform position)
    {
        //set future post-transition position
        playerTransportLoc = position.localPosition;

    }

    public void FadeSceneTransition(int sceneANumber)
    {
        StartCoroutine(Trans(1f, sceneANumber));
       
        
    }

    public void ScaleToTransitionColor(Transform trans)
    {
        //this is really bad. like really bad i dont have any reasonable excuses for this. what have I done
        Color scale2color = new Color(trans.localScale.x, trans.localScale.y, trans.localScale.z);
        fadeToColor = scale2color;
        
    }
    

    IEnumerator Trans(float dur, int scene)
    {
        //fade in black, load scene, fade out. Simple?
        GameObject canv = GameObject.FindGameObjectWithTag("MainCanvas");
        GameObject fullCol = Instantiate(fullScreenColor, canv.transform);
        Image fColor = fullCol.GetComponent<Image>();
        fColor.color = fadeToColor;

        StartCoroutine(FadeImg(dur, fColor));

        yield return new WaitForSeconds(dur);

     
        SceneManager.LoadScene(scene);
        transform.position = playerTransportLoc;

        yield return new WaitForSeconds(dur);
        StartCoroutine(FadeImg(dur, fColor, 1));
        Destroy(fullCol, 1.3f);
    }

    IEnumerator FadeImg(float dur, Image img, int direction = 0, float fadeTo = 1f)
    {
        Debug.Log((0f + direction * fadeTo) + " " + ((1f * fadeTo) - (direction * fadeTo)));
        for (float i = 0; i < dur; i += Time.deltaTime)
        {
            //if direction is 1 then it fades out
            float alpha = Mathf.Lerp(0f + direction * fadeTo, (1f * fadeTo) - (direction * fadeTo), i);

            Color newImgColor = new Color(img.color.r, img.color.g, img.color.b, alpha);

            img.color = newImgColor;
            yield return null;
        }


    }



}
