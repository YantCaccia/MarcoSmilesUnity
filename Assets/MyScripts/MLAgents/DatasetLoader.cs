using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;


public class DatasetLoader : MonoBehaviour
{
    public GameObject actualPose;
    // Variabile to understand if we are in play or not 
    public Boolean PlayModeOn = false;
    // Start is called before the first frame update
    public Boolean charged = false;
    public List<List<float>> features = new List<List<float>>();
    public List<float> distanceArray = new List<float>();
    public List<int> labelArray = new List<int>();
    public float[] actualFeatures;
    public int note;
    /// <summary>
    /// Path per AppData
    /// </summary>
    static string path;

    private float noiseFactor = 0.01f;

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
    string datasetpath = "";
    string Adatasetpath = "";


    private void Start()
    {
        if (!PlayModeOn) { 
            path = Application.persistentDataPath;
            datasetpath = $"{path}/{folderName}/{selectedDataset}/marcosmiles_dataset.csv";
            Adatasetpath = $"{path}/{folderName}/{selectedDataset}/augmented_dataset.csv";

            //if (File.Exists(Adatasetpath))
            //{ File.Delete(Adatasetpath); }
            //createAugmentedDataset();
            RetrieveRecordedNotes();
        }
      

        
    }


    private void Update()
    {
        if (PlayModeOn)
        {
            actualFeatures = actualPose.GetComponent<HandDataRetriever>().ArrayDistance;
        }
    }
    private void createAugmentedDataset()
    {

        

        // Leggi i dati dal file CSV
        string[] lines = File.ReadAllLines(datasetpath);




        // Itera attraverso le linee del file CSV
        foreach (string line in lines)
        {


            // Dividi la linea in colonne
            string[] columns = line.Split(',');
           
            for (int i = 0; i < 500; i++)
            {
                Boolean Label = true;
                // Foreach value of a note
                distanceArray = new List<float>();
                foreach (string column in columns)
                {
                    if (Label)
                    {

                        note = int.Parse(column);
                        labelArray.Add(note);
                        Label = false;
                    }
                    else
                    {
                        if (float.TryParse(column, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            float temp = ApplyJittering(value);
                            Debug.Log(temp);
                            distanceArray.Add(temp);
                        }


                    }

                }



                DataToStore RHandStore = new DataToStore("Right", distanceArray[0], distanceArray[1], distanceArray[2], distanceArray[3], distanceArray[4],
                                           distanceArray[5], distanceArray[6], distanceArray[7], distanceArray[8], distanceArray[9],
                                           distanceArray[10], distanceArray[11], distanceArray[12], distanceArray[13], distanceArray[14],
                                           distanceArray[15], distanceArray[16], distanceArray[17], distanceArray[18], distanceArray[19],
                                           distanceArray[20], distanceArray[21], distanceArray[22], distanceArray[23]);

                DataToStore LHandStore = new DataToStore("Left", distanceArray[24], distanceArray[25], distanceArray[26], distanceArray[27], distanceArray[28],
                                            distanceArray[29], distanceArray[30], distanceArray[31], distanceArray[32], distanceArray[33],
                                            distanceArray[34], distanceArray[35], distanceArray[36], distanceArray[37], distanceArray[38],
                                            distanceArray[39], distanceArray[40], distanceArray[41], distanceArray[42], distanceArray[43],
                                            distanceArray[44], distanceArray[45], distanceArray[46], distanceArray[47]);



                saveData(note, LHandStore, RHandStore);

            }
        }

        
    }
    float ApplyJittering(float value)
    {
        int number = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
        if (number == 0)
        {
            value += noiseFactor;
            //Debug.Log("questo è value augmented"+value);
        }
        else
        {
            value -= noiseFactor;
            //Debug.Log("questo è value decremented"+value);
        }
        return value;
    }


    
    private void saveData(int note, DataToStore left, DataToStore right)
    {

        //  Imposta il separatore dei numeri decimali a "." (nel caso si avesse il pc in lingua che usa "," come separatore si potrebbero avere problemi, quindi lo imposta manualmente)
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        Debug.Log("HERE");
        //  Controlla se il file nel path esiste
        if (File.Exists(Adatasetpath))
        {
            
            //  Se il file è presente nel path, crea una stringa contenente tutte le features e id della nota, per salvare sul file nel path

            
                Debug.Log("ITd == " + left.ITd);

                var str = $"{note},{left.TMd}, {left.TPd}, {left.TDd}, {left.TTd}," +
                   $" {left.IMd}, {left.IPd}, {left.IId}, {left.IDd}, {left.ITd}," +
                   $" {left.MMd}, {left.MPd}, {left.MId}, {left.MDd}, {left.MTd}," +
                   $" {left.RMd}, {left.RPd}, {left.RId}, {left.RDd}, {left.RTd}," +
                   $" {left.LMd}, {left.LPd}, {left.LId}, {left.LDd}, {left.LTd}," +
                   $" {right.TMd}, {right.TPd}, {right.TDd}, {right.TTd}," +
                   $" {right.IMd}, {right.IPd}, {right.IId}, {right.IDd}, {right.ITd}," +
                   $" {right.MMd}, {right.MPd}, {right.MId}, {right.MDd}, {right.MTd}," +
                   $" {right.RMd}, {right.RPd}, {right.RId}, {right.RDd}, {right.RTd}," +
                   $" {right.LMd}, {right.LPd}, {right.LId}, {right.LDd}, {right.LTd}"
                   .ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

                //  Va a capo per la prossima feature da salvare
                str += Environment.NewLine;

                //  Stampa la stringa str sul file, aggiungendo quindi tutte le posizioni registrate per la nota selezionata
                File.AppendAllText(Adatasetpath, str);



                //  stampa il messaggio nella console di debug
                Debug.Log("DATASET SAVED");
            
        }




        else
        {
            //  IL FILE NON ESISTE
            //  crea il file nel path appropriato
            File.Create(Adatasetpath).Dispose();

            //  Effettua una chiamata ricorsiva per salvare sul file appena creato
            saveData(note, left, right);

            //  stampa il messaggio nella console di debug
            Debug.Log("FILE CREATO");
        }
    }


    private void RetrieveRecordedNotes()
    {



        // Leggi i dati dal file CSV
        string[] lines = File.ReadAllLines(Adatasetpath);




        // Itera attraverso le linee del file CSV
        foreach (string line in lines)
        {


            // Dividi la linea in colonne
            string[] columns = line.Split(',');

            
            Boolean Label = true;
            // Foreach value of a note
            distanceArray = new List<float>();
            foreach (string column in columns)
            {
                if (Label)
                {

                    note = int.Parse(column);
                    labelArray.Add(note);
                    Label = false;
                }
                else
                {
                    if (float.TryParse(column, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                    {
                        float temp = ManipulateValue(value);
                        //Debug.Log(temp);
                        distanceArray.Add(temp);
                    }


                }




                features.Add(distanceArray);
               
            }
        }

        charged = true;


    }


    float ManipulateValue(float originalValue, float maxPercentageChange = 20f)
    {
        // Genera una percentuale casuale compresa tra -maxPercentageChange e maxPercentageChange
        float percentageChange = UnityEngine.Random.Range(-maxPercentageChange, maxPercentageChange);

        

        // Calcola il cambiamento percentuale
        float change = originalValue * (percentageChange / 100f);

        //Debug.Log("CHANGE" + change);

        // Applica il cambiamento al valore originale
        float manipulatedValue = originalValue + change;

        

        return manipulatedValue;
    }

    // Esempio di utilizzo del metodo
    
       

}



   

