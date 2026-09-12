using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField] bool[] initMusic;

    public AudioSource music;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        music = GetComponent<AudioSource>();
        SceneCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneCheck();
    }

    void SceneCheck()
    {
        int s = SceneManager.GetActiveScene().buildIndex;
       
        bool play = initMusic[s];

        if (play)
        {
            music.Play();
        } else
        {
            music.Stop();
        }

    }

    public IEnumerator PulseVolume(float duration, float minVol = 0f)
    {
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            float lerpVol = Mathf.Lerp(minVol, 1, i);
            music.volume = lerpVol;

            yield return null;
        }
    }

}
