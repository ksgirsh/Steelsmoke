using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    protected Animator anim;
    [SerializeField] string startAnim;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.CrossFade(startAnim, 0, 0);
    }

}
