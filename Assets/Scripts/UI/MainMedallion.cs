using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainMedallion : MonoBehaviour
{
    private float shakeIntensity = 1;
    [SerializeField] GameObject medal;
    [SerializeField] float angleInt = 30;
    [SerializeField] float worldSpaceRadius = 1.5f;
    

    private Camera mc;
    [SerializeField] int maxMedals = 5;
    [SerializeField] float currentMedals = 0;

    [SerializeField] Transform heartOffset;

    [SerializeField] List<GameObject> hearts;
    [SerializeField] Health heal;
    [SerializeField] RunAndJump rj;
    [SerializeField] Ultimate ult;

    [SerializeField] GameObject needle;
    [SerializeField] Image fuelBG;

    [SerializeField] Color minFuelColor;
    [SerializeField] Color maxFuelColor;

    // Start is called before the first frame update
    void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        rj = player.GetComponent<RunAndJump>();
        heal = player.GetComponent<Health>();
        ult = player.GetComponent<Ultimate>();
        mc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Needle();
        //SpawnMedals(maxMedals);
    }

    // Update is called once per frame
    void Update()
    {
        Needle();
    }

    public IEnumerator Shake(float dur, float intensity)
    {
        while (dur > 0)
        {
            Vector3 move = Random.insideUnitCircle * 10;
            Vector3 dir = move.normalized;

            transform.parent.position += (dir / 10f) * intensity;
            yield return new WaitForSeconds(Time.deltaTime);
            transform.parent.position -= (dir / 10f) * intensity;

            dur -= Time.deltaTime;

        }
       
    }

    public void SpawnMedals(int amt)
    {
        //polar coordinates
        for (int i = 0; i < amt; i++)
        {

            float angle = Mathf.Deg2Rad * (angleInt * i);
            float dist = worldSpaceRadius;

            Vector2 change = new Vector2((dist * Mathf.Cos(angle)) - (dist/2), (dist * Mathf.Sin(angle)));
            //Debug.Log(change);

            Vector2 initial = mc.WorldToScreenPoint(this.transform.position + (Vector3.right * (dist/2)));
            //Debug.Log(initial);
            // change + currentPos

            Vector3 finalPos = new Vector3(change.x + mc.ScreenToWorldPoint(initial).x, change.y + mc.ScreenToWorldPoint(initial).y, 0);

            GameObject newMedal = Instantiate(medal, finalPos, Quaternion.identity, this.transform);
            hearts.Add(newMedal);

            float scalar = Mathf.Sqrt((1f / (i + 1)));
            Vector3 Scaler = new Vector3(scalar, scalar, 1f);
            Vector3 finalScale = scalar * newMedal.transform.localScale;
            Debug.Log(scalar);

            newMedal.transform.localScale = finalScale;

            Quaternion rotation = Quaternion.identity;

            rotation.eulerAngles = new Vector3(0, 0, ((90 + angleInt*i)));
           // Debug.Log(angleInt*i);

            newMedal.transform.rotation = rotation;
            //Mathf.Deg2Rad();

        }
    }

    void Needle()
    {
        //calc angle based off of max medals
        float angle = ((rj.fuel * 2f) * 135f);
       // Debug.Log(135 - angle);

        //rotate needle by that angle
        Quaternion rotation = Quaternion.identity;

        rotation.eulerAngles = new Vector3(0, 0, ((135-angle)));
        needle.transform.rotation = rotation;

        if (rj.fuel < 0.99)
        {
            Color lerpColor = Color.Lerp(minFuelColor, maxFuelColor, rj.fuel);
           
            needle.GetComponent<Image>().color = lerpColor;
            lerpColor.a = (20f / 255f);
            fuelBG.color = lerpColor;

        } else
        {
            needle.GetComponent<Image>().color = ult.ultFuelColor;
            Color bgColor = ult.ultFuelColor;
            bgColor.a = (20f / 255f);
            fuelBG.color = bgColor;
        }



    }
}
