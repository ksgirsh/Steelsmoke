using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public bool isPaused { get; private set; }

    [SerializeField] GameObject[] aspectRatioBars;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            foreach (GameObject bar in aspectRatioBars)
            {
                bar.SetActive(false);
            }
           
        } else
        {
            Time.timeScale = 1;
            foreach (GameObject bar in aspectRatioBars)
            {
                bar.SetActive(true);
            }
        }

        pauseMenu.SetActive(isPaused);
    }
}
