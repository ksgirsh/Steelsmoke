using UnityEngine;

public class Mimic : MonoBehaviour
{
    [SerializeField] SpriteRenderer mimicSprite;
    private SpriteRenderer thisSprite;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        thisSprite.sprite = mimicSprite.sprite;
    }
}
