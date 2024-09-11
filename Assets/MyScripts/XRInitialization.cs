using System.Collections;
using System.Collections.Generic;
// Start is called before the first frame update
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;



public class XRInitialization : MonoBehaviour
{




    void Start()
    {
        //XRGeneralSettings.Instance.Manager.InitializeLoader();
       
    }

    

    void OnApplicationQuit()
    {
        // Assicurati che il loader XR attivo sia deinizializzato prima di inizializzarne uno nuovo
        if (XRGeneralSettings.Instance != null)
        {
            Debug.Log("DELETED");
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }



    }
}

