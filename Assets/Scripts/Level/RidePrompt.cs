using UnityEngine;

public class RidePrompt : ItemInspectable
{
    private GameObject ziplineHead;
    [SerializeField] LineRenderer zipline;
    private RideLine rL;
    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ziplineHead = player.GetComponent<RunAndJump>().zipline;
        rL = ziplineHead.GetComponent<RideLine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PromptGo()
    {
        ziplineHead.SetActive(true);
        rL = ziplineHead.GetComponent<RideLine>();
        rL.enabled = true;
        rL.AssignLine(zipline);
        rL.TriggerRide();
    }
}
