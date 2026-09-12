using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [field:SerializeField] public GameObject textBox { get; private set; }
    [field: SerializeField] public GameObject textContainer { get; private set; }
    [field: SerializeField] public GameObject dialogueBox { get; private set; }
    [field: SerializeField] public GameObject dialogueText { get; private set; }
    [field: SerializeField] public GameObject playerUI { get; private set; }
    [field: SerializeField] public GameObject pauseUI { get; private set; }
    [field: SerializeField] public GameObject pauseContainer { get; private set; }
    [field: SerializeField] public GameObject bossUI { get; private set; }
    [field: SerializeField] public List<GameObject> medallionEffects { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
