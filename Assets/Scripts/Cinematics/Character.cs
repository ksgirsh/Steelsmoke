using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    TextBoxController tB;
    [SerializeField] Sprite charaPortrait;

    [SerializeField] Sprite[] portraitsToSet;
    [SerializeField] Sprite[] iconsToSet;
    [SerializeField] Sprite[] spritesToSet;

    [SerializeField] AudioClip[] eebydeebySounds;

    // Start is called before the first frame update
    void Start()
    {
        tB = UIManager.instance.textBox.GetComponent<TextBoxController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this portrait stuff is stupid and i hate it
    public void CharaSay(string line)
    {
        tB.CharacterSay(charaPortrait, line, eebydeebySounds);
    }

    public void SetPortrait(int iD)
    {
        //Debug.Log("SET " + tB.portrait.gameObject.name + " " + iD);
        charaPortrait = portraitsToSet[iD];

    }

    public void SetIcon(int iD)
    {
        tB.SetIcon(iconsToSet[iD]);

    }

    public void SetSprite(int spr)
    {
        StartCoroutine(Sprite(spr));
    }

    IEnumerator Sprite(int spr)
    {
        //  yield return new WaitForSeconds(del);
        yield return null;
        gameObject.GetComponent<SpriteRenderer>().sprite = spritesToSet[spr];
    }
}
