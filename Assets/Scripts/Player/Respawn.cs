using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Respawn : MonoBehaviour
{

    public List<GameObject> deadEnemies;
    private List<GameObject> takenItems;
    [SerializeField] Vector3 savedPos;

    public float[] savedValues;


    public static event Action<Respawn> OnRespawnEvent;

    [SerializeField] Inspect insp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RespawnControl();
        }
    
    }
    public void RespawnControl()
    {
        Time.timeScale = 1f;
        Debug.Log("Called");
        this.transform.position = savedPos;
        //  this.transform.position = savedPos.position;
        foreach (GameObject enm in deadEnemies)
        {
            enm.SetActive(true);
            //enm.GetComponent<EnemyBase>().RespawnEnemy();
         
        }
      

        deadEnemies.Clear();
        OnRespawnEvent(this);

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Health")
        {
            if (!coll.gameObject.GetComponent<ItemFloat>().dropped)
            {
            //    takenItems.Add(coll.gameObject);
            }
        
        }

        if (coll.gameObject.tag == "Checkpoint")
        {
            savedPos = coll.gameObject.transform.position;

            //StartCoroutine(coll.gameObject.GetComponent<LightControl>().Pulse(0.6f, 2f));
            
            StartCoroutine(CheckpointEff(coll.gameObject));
        }
    }

    IEnumerator CheckpointEff(GameObject obj)
    {
        if (obj.GetComponent<LightControl>() != null)
        {
            StartCoroutine(obj.GetComponent<LightControl>().Pulse(0.6f, 2f));
        }

        yield return new WaitForSeconds(1.8f);

        obj.SetActive(false);
    }

    public void ResetEnemies()
    {
        Debug.Log("cALLED");
        foreach(GameObject enemyObj in deadEnemies)
        {
            EnemyBase enem = enemyObj.GetComponent<EnemyBase>();
            enemyObj.SetActive(true);
            enem.RespawnEnemy(this);
            enem.ResetEnemy(this);
           
        }
    }
}
