using UnityEngine;

public class KillObjective : Objective
{

    string prefix = "Kill ";
    string suffix = " Enemies";
    [SerializeField] string amt = "All";

    int collectedKills = 0;
    int remainingKills = 0;

    int killBenchmark;

    private Shooting shoot;

    protected override void Start()
    {
        base.Start();

        if (objAmt > 0)
        {
            amt = "All";
        } else
        {
            amt = "Zero";
        }

        objText = string.Concat(prefix, amt, suffix);
        //objText is like: "Kill All Enemies"

        shoot = player.GetComponent<Shooting>();
        remainingKills = (objAmt - collectedKills);

        killBenchmark = shoot.kills;
        ObjectiveFailed();
    }

    void Update()
    {
        //weird way of updating kills but i dont want to debase myself to weird reference shit
        if (shoot.kills > killBenchmark)
        {
            KillUpdate();
        }
    }

    void KillUpdate()
    {
        collectedKills++;
        remainingKills--;
        killBenchmark = shoot.kills;
        objText = string.Concat(prefix, (remainingKills.ToString()), suffix);
        ChangeText();

        if (objAmt! > 0 && collectedKills > 0)
        {
            ObjectiveFailed();
        }

        if (collectedKills >= objAmt && objAmt > 0)
        {
            ObjectiveSucceded();
        }

    }
}
