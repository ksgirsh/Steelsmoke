using UnityEngine;

public class TimeObjective : Objective
{
    string prefix = "Finish In ";
    string suffix = " Minutes";
    //time is in seconds. dont be convinced by the eevil suffix. We're doing this in seconds.
    int displayTime = 0;
    int amt = 0;

    public bool timerStarted;
    public bool timerEligible;

    private float benchmarkTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        amt = objAmt;
        if (objAmt < 61)
        {

            suffix = " Seconds";
            displayTime = objAmt;

        } else
        {
            displayTime = (int) (Mathf.Round((objAmt / 60)));
        }

        objText = string.Concat(prefix, (displayTime.ToString()), suffix);
        ObjectiveSucceded();
   
    }

    // Update is called once per frame
    void Update()
    {
        if (timerStarted)
        {
            benchmarkTime -= Time.deltaTime;
            if (benchmarkTime <= 0)
            {
                benchmarkTime = 1f;
                TimerUpdate();
            }

        } else
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetAxisRaw("Horizontal") != 0) && timerEligible == true)
            {
                timerStarted = true;
            }
        }


    }

    void TimerUpdate()
    {
        amt--;

        if (suffix == " Seconds")
        {
            displayTime--;
            objText = string.Concat(prefix, (displayTime.ToString()), suffix);
            ChangeText();

        } else
        {
            if ((objAmt - amt) % 60 == 0)
            {
                displayTime--;
                objText = string.Concat(prefix, (displayTime.ToString()), suffix);
                ChangeText();

            }
        }

        if (amt < 0)
        {
            ObjectiveFailed();

        } else
        {
            ObjectiveSucceded();
        }



    }

    public void EnableTimer()
    {
        timerEligible = true;
    }
}
