using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] Cutscene[] cut;
    bool triggered = false;
    [SerializeField] bool resetOnDeath;

    [SerializeField] AudioSource musicM;
    [SerializeField] AudioClip initMusic;




    int deathTracker = 0;

    // Start is called before the first frame update
    void Start()
    {
        initMusic = musicM.clip;
        deathTracker = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && triggered == false)
        {
            if (deathTracker > 0)
            {
                //plays alt cutscene on retrigger/respawn
                cut[1].currentAction = 0;
                cut[1].PlayCutscene();

            } else
            {
                cut[0].currentAction = 0;
                cut[0].PlayCutscene();
            }
            
            triggered = true;
        }
    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += ResetCutscene;
    }

    void ResetCutscene(Respawn resp)
    {
        deathTracker++;
        musicM.Stop();
        musicM.clip = initMusic;
        musicM.Play();

        if (resetOnDeath)
        {
            triggered = false;

        }
    }
}
