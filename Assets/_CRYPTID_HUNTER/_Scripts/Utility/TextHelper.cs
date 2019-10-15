using System;

public class TextHelper : Singleton<TextHelper>
{

    public string FormatTime(int hours, int minutes, bool isMilitaryTime)
    {
        string formattedTime;

        if(isMilitaryTime)
        {
            formattedTime = String.Format("{0:00}", hours) + ":" + String.Format("{0:00}", minutes);

            return formattedTime;
        }

        string amOrPM = (hours > 11) ? "PM" : "AM";
        if(hours > 12)
        {
            hours = hours - 12;
        }
        else if(hours == 0)
        {
            hours = 12;
        }
        formattedTime = String.Format("{0:00}", hours) + ":" + String.Format("{0:00}", minutes);
        formattedTime += " " + amOrPM;

        return formattedTime;
    }
}
