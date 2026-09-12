using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalker : MonoBehaviour
{
    [SerializeField] string startingAnimation;

    [SerializeField] protected GameObject player;
    [SerializeField] protected Animator anim;

    private Vector2 dist;

    [SerializeField] float escapeRange;
    private Vector2 escaping;
    [SerializeField] float escapeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        anim = gameObject.GetComponent<Animator>();

        anim.CrossFade(startingAnimation, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        dist = transform.position - player.transform.position;

        if (dist.magnitude <= escapeRange)
        {
            Escape();
        }

        if (escaping != Vector2.zero)
        {
            transform.position += (Vector3)escaping * Time.deltaTime * escapeSpeed;
            Destroy(gameObject, 3f);
        }
    }

    void Escape()
    {
        anim.CrossFade("StalkerEscape", 0, 0);
        float xDir = -player.transform.localScale.x;

        escaping = new Vector2(xDir, 2f);
    }


}
