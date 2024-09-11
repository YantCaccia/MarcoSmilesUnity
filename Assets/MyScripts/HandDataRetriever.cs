using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using System.IO;
using Unity.VisualScripting.Dependencies.NCalc;
using System.Globalization;

public class HandDataRetriever : MonoBehaviour
{
    public Boolean isPlaying = false;
    [SerializeField]
    public int playingNote = 24;
 

    public XRHand Righthand;
    public XRHand LeftHand;
    static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();
    XRHandSubsystem m_Subsystem;

    ConsoleVRScript ConsoleManager;
    public float[] ArrayDistance = new float[48];
    private float[] DoArray = new float[48];
    private Boolean emptyNote = true;
    private Boolean recordingNote = false;




    /// <summary>
    /// Path per AppData
    /// </summary>
    static string path;



    /// <summary>
    /// Nome del dataset da utilizzare
    /// </summary>
    public static string filename = "marcosmiles_dataset.csv";

    /// <summary>
    /// Nome della cartella contenente i datasets importati
    /// </summary>
    static string folderName = "MyDatasets";

    /// <summary>
    /// Nome Dataset
    /// </summary>
    public static string defaultFolder = "DefaultDataset";

    /// <summary>
    /// Dataset selezionato
    /// </summary>
    public static string selectedDataset = defaultFolder;







    private void Start()
    {
        path = Application.persistentDataPath;
        ConsoleManager = GameObject.FindGameObjectWithTag("ConsoleManager").GetComponent<ConsoleVRScript>();
      
        retrieveRecordedNotes();
    }

    Vector3 rootDistanceL = new Vector3(0, 0, 0);
    Vector3 rootDistanceR = new Vector3(0, 0, 0);
    float distance;
    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
        XRHandSubsystem.UpdateType updateType)
    {
        switch (updateType)
        {
            case XRHandSubsystem.UpdateType.Dynamic:
                // Update game logic that uses hand data
                if (m_Subsystem.rightHand.isTracked)
                {
                    Debug.Log("The right hand is tracked");
                    Righthand = m_Subsystem.rightHand;
                }
                else
                {
                    Debug.Log("The RIGHT hand NOT TRACKED");
                }

                if (m_Subsystem.leftHand.isTracked)
                {
                    Debug.Log("The LEFT hand is tracked");
                    LeftHand = m_Subsystem.leftHand;
                }
                else
                {
                    Debug.Log("The LEFT hand NOT TRACKED");
                }
                string DistanceTextR = "================DISTANCE FROM PALM=================== \n";
                string DistanceTextL = "================DISTANCE FROM PALM=================== \n";
                string textToSendR = "";
                string textToSendL = "";
                Debug.Log("CLEANED TEXT");

                int jointsCounter = 0;
                if (m_Subsystem.rightHand.isTracked && m_Subsystem.leftHand.isTracked)
                {
                    Debug.Log("HANDS TRACKED");
                    for (var i = XRHandJointID.BeginMarker.ToIndex();
                    i < XRHandJointID.EndMarker.ToIndex();
                    i++)
                    {
                        var trackingData = Righthand.GetJoint(XRHandJointIDUtility.FromIndex(i));
                        //Debug.Log(trackingData);
                        //trackingData.TryGetRadius(out float radius);
                        // Debug.Log(radius);
                        if (trackingData.TryGetPose(out Pose pose))
                        {
                            // displayTransform is some GameObject's Transform component
                            //Debug.Log("RHand "+trackingData.id +"  "+ pose.position+"  "+ pose.rotation);
                            textToSendR += trackingData.id + "  " + pose.position + "  " + pose.rotation + "\n";
                            string examinatedJoint = "" + trackingData.id;
                            if (examinatedJoint.Equals("Palm"))
                            {
                                rootDistanceR = pose.position;
                            }
                            else if (!examinatedJoint.Equals("Wrist"))
                            {
                                float dtemp = Vector3.Distance(pose.position, rootDistanceR);
                                distance = float.Parse(dtemp.ToString("F3"));

                                DistanceTextR += "" + trackingData.id + distance + "\n";
                                ArrayDistance[jointsCounter] = distance;
                                jointsCounter++;

                            }







                        }
                    }


                    for (var i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)
                    {
                        var trackingData = LeftHand.GetJoint(XRHandJointIDUtility.FromIndex(i));
                        //Debug.Log(trackingData);
                        //trackingData.TryGetRadius(out float radius);
                        //Debug.Log(radius);
                        if (trackingData.TryGetPose(out Pose pose))
                        {
                            // displayTransform is some GameObject's Transform component
                            //Debug.Log("LHand " + trackingData.id + "  " + pose.position + "  " + pose.rotation);
                            textToSendL += trackingData.id + "  " + pose.position + "  " + pose.rotation + "\n";
                            string examinatedJoint = "" + trackingData.id;
                            if (examinatedJoint.Equals("Palm"))
                            {
                                rootDistanceL = pose.position;


                            }
                            else if (!examinatedJoint.Equals("Wrist"))
                            {
                                float dtemp = Vector3.Distance(pose.position, rootDistanceL);
                                distance = float.Parse(dtemp.ToString("F3"));

                                DistanceTextL += "" + trackingData.id + distance + "\n";
                                ArrayDistance[jointsCounter] = distance;
                                jointsCounter++;
                            }



                        }



                    }
                    if (ConsoleManager != null)
                    {
                        ConsoleManager.clearDisplay("R");
                        ConsoleManager.clearDisplay("L");
                        ConsoleManager.clearDisplay("C");



                        ConsoleManager.PrintOnDisplay(textToSendL + DistanceTextL, "L");
                        ConsoleManager.PrintOnDisplay(textToSendR + DistanceTextR, "R");


                    }



                }
                break;
            case XRHandSubsystem.UpdateType.BeforeRender:
                // Update visual objects that use hand data

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (m_Subsystem != null && m_Subsystem.running)
            return;

        SubsystemManager.GetSubsystems(s_SubsystemsReuse);
        var foundRunningHandSubsystem = false;
        for (var i = 0; i < s_SubsystemsReuse.Count; ++i)
        {
            var handSubsystem = s_SubsystemsReuse[i];
            if (handSubsystem.running)
            {
                UnsubscribeHandSubsystem();
                m_Subsystem = handSubsystem;
                foundRunningHandSubsystem = true;
                break;
            }
        }
        Debug.Log("found" + foundRunningHandSubsystem);

        if (!foundRunningHandSubsystem)
            return;




        Debug.Log(m_Subsystem.running);
        m_Subsystem.updatedHands += OnUpdatedHands;


       
       






    }



    void UnsubscribeHandSubsystem()
    {
        if (m_Subsystem == null)
            return;


        m_Subsystem.updatedHands -= OnUpdatedHands;
    }


  

    




    private static void FolderChecker()
    {
        string DefaultDatasets = $"{path}/{folderName}/{defaultFolder}";
        string MyDatasets = $"{path}/{folderName}";

        if (!Directory.Exists(MyDatasets))
        {
            Directory.CreateDirectory(MyDatasets);
            Directory.CreateDirectory(DefaultDatasets);
        }

    }


    /// <summary>
    /// Restituisce il nome della cartella del dataset
    /// </summary>
    /// <returns>Il nome della cartella del dataset</returns>
    public static string GetFolderName()
    {
        return folderName;
    }

    /// <summary>
    /// Genera il path per il file da utilizzare. il path è formato da: path (La cartella in appdata dell'aplicazione); 
    /// folderName (Cartella dei datasets) e filename (nome del file)
    /// </summary>
    /// <param name="filename">Nome del file</param>
    /// <returns></returns>
    public static string GeneratePath(string filename)
    {
        return $"{path}/{folderName}/{selectedDataset}/{filename}";
    }

    /// <summary>
    /// Genera il path per il file da utilizzare da una cartella specifica.
    /// il path è formato da: path (La cartella in appdata dell'aplicazione); folderName (Cartella dei datasets);
    /// folder (Cartella passato come parametro, all'interno della quale si vuole effettuare la ricerca) e filename (nome del file)
    /// 
    /// </summary>
    /// <param name="filename">Nome del file</param>
    /// <param name="folder">Nome della cartella</param>
    /// <returns></returns>
    private static string GeneratePath(string filename, string folder)
    {
        return $"{path}/{folderName}/{folder}/{filename}";
    }

    /// <summary>
    /// Prende il path per il datasaet. Versione public di generatepath. [(è davvero necessaria???? o basta mettere generate path a public?)]
    /// </summary>
    /// <returns>Ritorna il path per il dataset</returns>
    public static string GeneratePath()
    {
        return $"{GeneratePath("")}";
    }

    /// <summary>
    /// Prende il path per il datasaet
    /// </summary>
    /// <param name="folder">Nopme della cartella</param>
    /// <returns>Ritorna il path per il dataset</returns>
    public static string PrintPathFolder(string folder)
    {
        return $"{GeneratePath("", $"{folder}")}";
    }


    private void retrieveRecordedNotes()
    {
       
        string datasetpath=GeneratePath("marcosmiles_dataset.csv");
        
        // Leggi i dati dal file CSV
        string[] lines = File.ReadAllLines(datasetpath);
        ConsoleManager.PrintOnDisplay("sono in retrieveRecordedNotes()" + " \n", "C");
       
     

        // Itera attraverso le linee del file CSV
        foreach (string line in lines)
        {
            List<float> values = new List<float>();
            GameObject poseToSend = GameObject.Find("Pose0");
            // Dividi la linea in colonne
            string[] columns = line.Split(',');
            Boolean Label = true;
            // Foreach value of a note
            foreach (string column in columns)
            {
                if (Label)
                {
                    
                    poseToSend = GameObject.Find("Pose" + column);
                    Debug.Log(poseToSend.name + "retrieved");
                    Label = false;
                }
                else
                {
                    if (float.TryParse(column, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                    {
                        values.Add(value);
                    }
                    
                    
                }
                
            }
            
            poseToSend.GetComponent<NewBehaviourScript>().PoseR = new DataToStore("Right", values[0], values[1], values[2], values[3], 
                                                                                 values[4],values[5], values[6], values[7], values[8], 
                                                                                 values[9], values[10], values[11], values[12], values[13],
                                                                                 values[14],values[15], values[16], values[17], values[18], 
                                                                                 values[19], values[20], values[21], values[22], values[23]);

            poseToSend.GetComponent<NewBehaviourScript>().PoseL = new DataToStore("Left",values[24], values[25], values[26], values[27],
                                                                                    values[28],values[29], values[30], values[31], values[32], 
                                                                                    values[33],values[34], values[35], values[36], values[37], 
                                                                                    values[38],values[39],values[40], values[41], values[42],
                                                                                    values[43],values[44], values[45], values[46], values[47]);

            
            
            poseToSend.GetComponent<NewBehaviourScript>().IsRecorded = true;
            


        }
    }

}

   

    


