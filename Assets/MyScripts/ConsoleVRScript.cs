using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using TMPro;

public class ConsoleVRScript : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();

    public TextMeshProUGUI displayRIGHT;
    public TextMeshProUGUI displayLEFT;
    public TextMeshProUGUI displayCENTER;
    // Start is called before the first frame update

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
       
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        displayRIGHT = null;
        displayLEFT = null;
        
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : "";


            if (debugLogs.ContainsKey(debugKey))
                debugLogs[debugKey] = debugValue;
            else
                debugLogs.Add(debugKey, debugValue);
        }

        string displayTextRight = "";
        string displayTextLeft = "";
        Debug.Log("HEEEEEEREEE");

        foreach (KeyValuePair<string, string> log in debugLogs)
        {
            if (log.Value == "")
            {
                displayTextRight += log.Key + "\n";
                displayTextLeft += log.Key + "\n";
            }
            else
            {
                if (log.Key.Contains("RHand"))
                {
                    displayTextRight += log.Key + ":";
                    Debug.Log("IS RIGHTTTTTT");
                }
                //+ log.Value + "\n";

                if (log.Value.Contains("LHand"))
                    displayTextLeft += log.Key + ":";
                //+ log.Value + "\n";

            }


            //displayRIGHT.text = displayTextRight;




            //displayLEFT.text = displayTextLeft;




        }
    }


        public void PrintOnDisplay(string value,string position)
        {
            if (position.Equals("R"))
            {
                displayRIGHT.text += value;
            }


            if (position.Equals("L"))
            {
                displayLEFT.text += value;
            }

        if (position.Equals("C"))
        {
            displayCENTER.text += value;
        }

    }


    public void clearDisplay(string position)
    {
        if (position.Equals("R"))
        {
            displayRIGHT.text = "";
        }


        if (position.Equals("L"))
        {
            displayLEFT.text = "";
        }

        if (position.Equals("C"))
        {
            displayCENTER.text = "";
        }

    }



}

 

