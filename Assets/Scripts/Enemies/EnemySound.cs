using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySound : MonoBehaviour
{
    private AudioSource audio;
    private Transform player;

    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy != null)
        {
            if (enemy.activeSelf)
            {
                audio.volume = Mathf.InverseLerp(20, 10, (player.position - transform.position).magnitude);
            }
            else
            {
                audio.volume = 0f;
            }

        }

    }
}
