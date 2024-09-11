using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem.Switch;
using UnityEngine.XR.Hands;




public class HandDataRecorder : MonoBehaviour
{
    public GameObject HandPositionSaver;
    private int numberOfDecimal = 3;
    private Boolean[] RecordedNote = new Boolean[24];
    public Boolean doRecorder = false;
    public XRHand Righthand;
    public XRHand LeftHand;
    static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();
    XRHandSubsystem m_Subsystem;
    public int number_of_instances = 500;
    public float sampling_time = 0f;

    ConsoleVRScript ConsoleManager;
    private float[] ArrayDistance=new float[48];
    
    private Boolean emptyNote=true;
    private Boolean recordingNote = false;
    private DataToStore RHandStore;
    private DataToStore LHandStore;

    /// <summary>
    /// Path per AppData
    /// </summary>
    static string path;
    public Boolean RecordMoreInstance = false;



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
        FolderChecker();
        ConsoleManager =GameObject.FindGameObjectWithTag("ConsoleManager").GetComponent<ConsoleVRScript>();

        Debug.Log(GeneratePath());
        recoverNotes();
        UpdateColorNotes();
    }

    Vector3 rootDistanceL=new Vector3 (0, 0, 0);
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
                            else if(!examinatedJoint.Equals("Wrist"))
                            {
                                float dtemp = Vector3.Distance(pose.position, rootDistanceR);
                                distance = float.Parse(dtemp.ToString("F3"));


                                Debug.DrawLine(pose.position, rootDistanceR, Color.red);

                                DistanceTextR += "" + trackingData.id + distance + "\n";
                                ArrayDistance[jointsCounter] = distance;
                                jointsCounter++;
                                
                            }

                            





                        }
                    }


                    for (var i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex();i++)
                    {
                        var trackingData = LeftHand.GetJoint(XRHandJointIDUtility.FromIndex(i));
                        //Debug.Log(trackingData);
                        //trackingData.TryGetRadius(out float radius);
                        //Debug.Log(radius);
                        if (trackingData.TryGetPose(out Pose pose))
                        {
                            
                            // displayTransform is some GameObject's Transform component
                            //Debug.Log("LHand " + trackingData.id + "  " + pose.position + "  " + pose.rotation);
                            textToSendL += trackingData.id + "  " + pose.position + "  " + pose.rotation +"\n";
                            string examinatedJoint = ""+trackingData.id;
                            if (examinatedJoint.Equals("Palm"))
                            {
                                rootDistanceL = pose.position;
                            
                                
                            }
                            else if (!examinatedJoint.Equals("Wrist"))
                            {
                                float dtemp = Vector3.Distance(pose.position, rootDistanceL);
                                distance = float.Parse(dtemp.ToString("F3"));


                                Debug.DrawLine(pose.position, rootDistanceL, Color.red);

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
                        


                        ConsoleManager.PrintOnDisplay(textToSendL+DistanceTextL, "L");
                        ConsoleManager.PrintOnDisplay(textToSendR+DistanceTextR, "R");
                        

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


        ConsoleManager.PrintOnDisplay("VARIABLE EMPTY NOTE == " + emptyNote, "C");
        if (!emptyNote)
        {
           
            ConsoleManager.PrintOnDisplay("A NOTE EXIST \n", "C");
            

        }
       





    }


    IEnumerator StartRecording(int sec,int note)
    {
        for (int i = 0; i < 24; i++)
        {
            if (i != note)
            {
                GameObject button = GameObject.Find(i.ToString());
                button.GetComponent<Button>().interactable = false;
            }
        }

        int countdownTime = sec;

        while (countdownTime > 0)
        {
            Debug.Log(countdownTime.ToString());
            ConsoleManager.PrintOnDisplay(countdownTime.ToString()+"\n", "C");
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        Debug.Log("FINISHED");

        int repeat = 1;
        if (RecordMoreInstance == true)
        {
            
            repeat = number_of_instances;
        }

        for (int i = 0; i < repeat; i++)
        {
            RecordKey(note);
            ConsoleManager.clearDisplay("C");
            ConsoleManager.PrintOnDisplay(" "+i+"----------" + note, "C");
            yield return new WaitForSeconds(sampling_time);
        }
        

        for (int i = 0; i < 24; i++)
        {
            if (i != note)
            {
                GameObject button = GameObject.Find(i.ToString());
                button.GetComponent<Button>().interactable=true;
            }
        }
        recordingNote = false;

        SaveOnFile(""+note + "\n", "NoteRecorder.txt");
        recoverNotes();
        UpdateColorNotes();

    }

    void UnsubscribeHandSubsystem()
    {
        if (m_Subsystem == null)
            return;

      
        m_Subsystem.updatedHands -= OnUpdatedHands;
    }


    void RecordKey(int note) 
    {
        
        {
            /*foreach (float d in ArrayDistance)
            {
                ConsoleManager.PrintOnDisplay(d.ToString() + "\n", "C");
            }
            */


            RHandStore = new DataToStore("Right", ArrayDistance[0], ArrayDistance[1], ArrayDistance[2], ArrayDistance[3], ArrayDistance[4],
                                        ArrayDistance[5], ArrayDistance[6], ArrayDistance[7], ArrayDistance[8], ArrayDistance[9],
                                        ArrayDistance[10], ArrayDistance[11], ArrayDistance[12], ArrayDistance[13], ArrayDistance[14],
                                        ArrayDistance[15], ArrayDistance[16], ArrayDistance[17], ArrayDistance[18], ArrayDistance[19],
                                        ArrayDistance[20], ArrayDistance[21], ArrayDistance[22], ArrayDistance[23]);

            LHandStore = new DataToStore("Left",ArrayDistance[24], ArrayDistance[25], ArrayDistance[26], ArrayDistance[27], ArrayDistance[28],
                                        ArrayDistance[29], ArrayDistance[30], ArrayDistance[31], ArrayDistance[32], ArrayDistance[33],
                                        ArrayDistance[34], ArrayDistance[35], ArrayDistance[36], ArrayDistance[37], ArrayDistance[38],
                                        ArrayDistance[39], ArrayDistance[40], ArrayDistance[41], ArrayDistance[42], ArrayDistance[43],
                                        ArrayDistance[44], ArrayDistance[45], ArrayDistance[46], ArrayDistance[47]);



            if (!RecordedNote[note])
                saveData(note, RHandStore, LHandStore);
            //HandPositionSaver.GetComponent<HandPositionSaver>().filename=note+".txt";
            //HandPositionSaver.GetComponent<HandPositionSaver>().saveHandsPosition();
                emptyNote = false;








        }

        
    }

    // here there is a problem with data need to change left with right
    //actually saving label/right/left
    private void saveData(int note,DataToStore left, DataToStore right)
    {

        //  Imposta il separatore dei numeri decimali a "." (nel caso si avesse il pc in lingua che usa "," come separatore si potrebbero avere problemi, quindi lo imposta manualmente)
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        Debug.Log("HERE");
        //  Controlla se il file nel path esiste
        if (File.Exists(GeneratePath(filename)))
        {
            //  Se il file è presente nel path, crea una stringa contenente tutte le features e id della nota, per salvare sul file nel path


            Debug.Log("ITd == " +left.ITd);
            
            var str = $"{note},{left.TMd}, {left.TPd}, {left.TDd}, {left.TTd}," +
               $" {left.IMd}, {left.IPd}, {left.IId}, {left.IDd}, {left.ITd}," +
               $" {left.MMd}, {left.MPd}, {left.MId}, {left.MDd}, {left.MTd}," +
               $" {left.RMd}, {left.RPd}, {left.RId}, {left.RDd}, {left.RTd}," +
               $" {left.LMd}, {left.LPd}, {left.LId}, {left.LDd}, {left.LTd},"+
               $" {right.TMd}, {right.TPd}, {right.TDd}, {right.TTd}," +
               $" {right.IMd}, {right.IPd}, {right.IId}, {right.IDd}, {right.ITd}," +
               $" {right.MMd}, {right.MPd}, {right.MId}, {right.MDd}, {right.MTd}," +
               $" {right.RMd}, {right.RPd}, {right.RId}, {right.RDd}, {right.RTd}," +
               $" {right.LMd}, {right.LPd}, {right.LId}, {right.LDd}, {right.LTd}"
               .ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

            //  Va a capo per la prossima feature da salvare
            str += Environment.NewLine;

            //  Stampa la stringa str sul file, aggiungendo quindi tutte le posizioni registrate per la nota selezionata
            File.AppendAllText(GeneratePath(filename), str);



            //  stampa il messaggio nella console di debug
            Debug.Log("DATASET SAVED");
        }




        else
        {
            //  IL FILE NON ESISTE
            //  crea il file nel path appropriato
            File.Create(GeneratePath(filename)).Dispose();

            //  Effettua una chiamata ricorsiva per salvare sul file appena creato
            saveData(note,left,right);

            //  stampa il messaggio nella console di debug
            Debug.Log("FILE CREATO");
        }
    }
        


    

    public void OnButtonPressed(GameObject sender)
    {
        if (recordingNote == false)
        {
            recordingNote = true;
            ConsoleManager.PrintOnDisplay("+\n BUTTON PRESSED " + sender.name, "C");
            StartCoroutine(StartRecording(10, int.Parse(sender.name)));
     

            
        }
              




    }



    /*if (!recordingNote)
    {
        Debug.Log("STARTING COOLDOWN");
        recordingNote = true;
        StartCoroutine(CountDown(5));


    }
    */

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


    void SaveOnFile(string texttosave,string filename)
    {
        // Percorso del file (puoi modificarlo secondo le tue esigenze)
        string filepath = GeneratePath(filename);

        // Controlla se il file esiste già
        if (!File.Exists(filepath))
        {
            // Crea il file se non esiste
            File.WriteAllText(filepath, texttosave);
          
        }
        else
        {
            // Se il file esiste già, aggiungi il testo al file
            File.AppendAllText(filepath, texttosave);
            
        }
    }


    private static void FolderChecker()
    {
        string notesDirectory = $"{path}/{folderName}/{defaultFolder}/notesDirectory";
        string DefaultDatasets= $"{path}/{folderName}/{defaultFolder}";
        string MyDatasets= $"{path}/{folderName}";

        if (!Directory.Exists(MyDatasets))
        {
            Directory.CreateDirectory(MyDatasets);
            Directory.CreateDirectory(DefaultDatasets);
            Directory.CreateDirectory(notesDirectory);

        }
        
    }


    private void recoverNotes()
    {
        string path = GeneratePath("NoteRecorder.txt");
        if (File.Exists(path))
        {

            // Read all the lines
            string[] righe = File.ReadAllLines(path);

            // Iterate on all the rows and set true the value of the array RecorderNote
            foreach (string riga in righe)
            {
                RecordedNote[int.Parse(riga)] = true; 
            }
        }

    }

    
    private void UpdateColorNotes()
    {
        
       
        for (int i = 0; i < 24; i++) 
        {
            GameObject button = GameObject.Find(i.ToString());
            Button b = button.GetComponent<Button>();
            if (RecordedNote[i])
            {
                b.image.color = Color.green; 
            }
            else
            {
                b.image.color = Color.white;
            }
            
           
            






        }
    }



    
}








