using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class HandStandScript : MonoBehaviour
{

    public GameObject lefthand;
    public GameObject righthand;
    public static string path;
    /// <summary>
    /// Nome del dataset da utilizzare
    /// </summary>
    public string filename = "4.txt";

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

    // Dizionario per mappare i nomi agli oggetti Vector3
    private Dictionary<string, Vector3> objectPositions = new Dictionary<string, Vector3>();

    void Start()
    {
        setposition();
    }
    public void  setposition()
    {
         path = Application.persistentDataPath;
        // Percorso del file contenente le informazioni
        string filepath = $"{path}/{folderName}/{selectedDataset}/{filename}";
        float x=0f, y=0f, z=0f;
        if (File.Exists(filepath))
        {
            // Leggi tutte le linee dal file
            string[] lines = File.ReadAllLines(filepath);

            // Analizza e assegna i valori al dizionario
            foreach (string line in lines)
            {
                // Splitta la riga in base alle virgole
                string[] values = line.Split(',');

                // Estrai il nome e i valori X, Y, Z
                string name = values[0];
                string temp=values[1].Replace("(", "");


                if (float.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out float resultx))
                {
                     x = resultx;
                }
                if (float.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out float resulty))
                {
                     y = resulty;
                }
                temp = values[3].Replace(")", "");
                if (float.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out float resultz))
                {
                     z = resultz;
                }
                
                
                

                // Aggiungi al dizionario
                objectPositions[name] = new Vector3(x, y, z);
                Debug.Log(objectPositions[name]);
            }

            // Ora puoi accedere ai valori utilizzando i nomi
            //lefthand.transform.position = new Vector3(0, 0, 0);
           // righthand.transform.position =new Vector3(0, 0, 0);
            TraverseHierarchy(lefthand.transform);
            TraverseHierarchy(righthand.transform);
        }

    }

    void TraverseHierarchy(Transform parent)
    {
        // Scorrere tutti i figli del GameObject corrente
        foreach (Transform child in parent)
        {

            child.localPosition = objectPositions[child.name];

            // Chiamata ricorsiva per esplorare i figli di questo GameObject
            TraverseHierarchy(child);
        }
    }
}

