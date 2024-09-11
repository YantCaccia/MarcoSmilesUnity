using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Processing;

public class HandPositionSaver : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    static string path;



    /// <summary>
    /// Nome del dataset da utilizzare
    /// </summary>
    public string filename = "HandsPositions.txt";

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


    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void saveHandsPosition()
    {


        SaveOnFile(leftHand.name + "," + leftHand.transform.localPosition+"\n");
       
        TraverseHierarchy(leftHand.transform);


        SaveOnFile(rightHand.name + "," + rightHand.transform.localPosition + "\n");

        TraverseHierarchy(rightHand.transform);


    }
    void TraverseHierarchy(Transform parent)
    {
        // Scorrere tutti i figli del GameObject corrente
        foreach (Transform child in parent)
        {
            // Ora puoi fare qualcosa con ciascun GameObject, ad esempio accedere a componenti, ecc.
            Vector3 temp = child.localPosition;
            SaveOnFile(child.name + "," + temp+ "\n");

            // Chiamata ricorsiva per esplorare i figli di questo GameObject
            TraverseHierarchy(child);
        }
    }


    void SaveOnFile(string texttosave)
    {

        // Percorso del file (puoi modificarlo secondo le tue esigenze)
        string filepath = $"{path}/{folderName}/{selectedDataset}/{filename}";
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
}
