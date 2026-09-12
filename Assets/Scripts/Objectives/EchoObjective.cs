using UnityEngine;

public class EchoObjective : Objective
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] string prefix = "Collect ";
    [SerializeField] string suffix = " Echoes";
    string amt = "";

    int collectedEchoes = 0;
    int remainingEchoes = 0;


    protected override void Start()
    {
        base.Start();
        amt = objAmt.ToString();
        objText = string.Concat(prefix, amt, suffix);
        //objText could be like: "Collect 5 Echoes"

        remainingEchoes = (objAmt - collectedEchoes);
        ObjectiveFailed();

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Collectible")
        {
            if (coll.gameObject.GetComponent<Collectible>().attribute == "Echo")
            {
                collectedEchoes++;
                remainingEchoes--;

                objText = string.Concat(prefix, ((remainingEchoes).ToString()), suffix);
                ChangeText();

                if (collectedEchoes >= objAmt)
                {
                    ObjectiveSucceded();
                } else
                {
                    ObjectiveFailed();
                }

                Destroy(coll.gameObject);

            }
        }
    }
}
