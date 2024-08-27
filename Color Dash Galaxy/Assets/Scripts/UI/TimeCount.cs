using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCount : MonoBehaviour
{
    Text timeText;
    float timeSinceLevelStart;

    private void Start()
    {
        timeText = GetComponent<Text>();
        timeSinceLevelStart = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        timeText.text = FormatDisplayedTime(timeSinceLevelStart);
        timeSinceLevelStart += Time.deltaTime;
    }

    private string FormatDisplayedTime(float timeInSeconds)
    {
        int intTime = (int)timeInSeconds;
        int hours = intTime / 3600;
        int minutes = (intTime % 3600) / 60;
        int seconds = intTime % 60;

        if (hours < 1)
        {
            return "Time: " + $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            return "Time: " + $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}
