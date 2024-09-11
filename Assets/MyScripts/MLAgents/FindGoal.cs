
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using TMPro;
using System.Globalization;
using System;
using System.IO;
using UnityEngine.InputSystem;

public class FindGoal : Agent
{
    private float[] normalizedAndScaledValues;
    private int LabelNote;
    public GameObject leftWall;
    public GameObject rightWall;
    public Material green;
    public Material red;
    [SerializeField]  private GameObject finder;

    private int episodecounter = 0;
    [SerializeField] List<float> dArray=new List<float>();
    //public GameObject DatasetLoader;
    private int _note;
    public GameObject cube;
    public TextMeshPro text;
    
    public List<float> distanceArray = new List<float>();
    
    static string path;

    public static string filename = "marcosmiles_dataset.csv";
    static string folderName = "MyDatasets";
    public static string defaultFolder = "DefaultDataset";
    public static string selectedDataset = defaultFolder;
    string datasetpath = "";
    string Adatasetpath = "";

    [SerializeField] private Transform targetTransform;
    

    private void Start()
    {
        path = Application.persistentDataPath;
        datasetpath = $"{path}/{folderName}/{selectedDataset}/marcosmiles_dataset.csv";
        retrieveRecordedNotes();
        setWalls();

        

    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(1.65f, -1.13f, 0.63f);
        


        episodecounter++;
        Debug.Log("EPISODE " + episodecounter);
        
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 1f;
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionEnter(Collider other)
    {
        Debug.Log("TRIGGEREEEED");
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            Debug.Log("goaaaaaal");
            SetReward(1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            Debug.Log("WALLLLLLLLLLLLL");
            SetReward(-1f);
            EndEpisode();
        }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");


    }




    [SerializeField]
    public int note
    {
        get { return _note; }
        set
        {
            if (_note != value)
            {
                _note = value;
                OnMyIntChanged();
            }
        }
    }

    // Metodo chiamato quando note viene modificato
    int counter=0;
    private void OnMyIntChanged()
    {
        //Debug.Log("onMyIntChanged");
        if (note == 4)
        {
            Debug.Log("GOT REWARD");
            AddReward(100f);
            cube.GetComponent<MeshRenderer>().material=green;
            EndEpisode();
        }
        else 
        {
            cube.GetComponent<MeshRenderer>().material = red;
            AddReward(-100f);
            counter++;
            EndEpisode();

           
            

        }

        // Puoi inserire qui il codice che desideri eseguire quando la variabile viene modificata.
    }



    private void retrieveRecordedNotes()
    {



        // Leggi i dati dal file CSV
        string[] lines = File.ReadAllLines(datasetpath);




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

                        LabelNote = int.Parse(column);
                        Label = false;
                    }
                    else
                    {
                        if (float.TryParse(column, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            distanceArray.Add(value);
                        }


                    }

                



                
            }
        }
        normalizedAndScaledValues = NormalizeAndScaleValues(distanceArray.ToArray());


    }
    /*private void setWalls()
    {
        counter = 0;
        foreach (Transform child in leftWall.transform)
        {
            Vector3 newScale = child.localScale;
            newScale.y = 2 * normalizedAndScaledValues[counter];
            child.localScale = newScale;
            counter++;
        }

        foreach (Transform child in rightWall.transform)
        {
            Vector3 newScale = child.localScale;
            newScale.y = 2 * normalizedAndScaledValues[counter];
            child.localScale = newScale;
            counter++;
        }
    }
    */

    private void setWalls()
    {
        counter = 0;
        int upcount = 0;
        float[] downValue= new float[24];
        int downcount = 0;
        foreach (Transform child in leftWall.transform)
        {
            if (upcount < 24)
            {
                Vector3 newScale = child.localScale;
                newScale.y = 2 * normalizedAndScaledValues[counter];
                child.localScale = newScale;
                downValue[upcount] = 0.8f - normalizedAndScaledValues[counter];
                counter++;
                upcount++;
            }
            else
            {
                Vector3 newScale = child.localScale;
                newScale.y = 2 * downValue[downcount];
                child.localScale = newScale;
                downcount++;
                
                
            }
        }
        downcount = 0;
        upcount = 0;

        foreach (Transform child in rightWall.transform)
        {
            if (upcount < 24)
            {
                Vector3 newScale = child.localScale;
                newScale.y = 2 * normalizedAndScaledValues[counter];
                child.localScale = newScale;
                downValue[upcount] = 0.8f - normalizedAndScaledValues[counter];
                counter++;
                upcount++;
            }
            else
            {
                Vector3 newScale = child.localScale;
                newScale.y = 2 * downValue[downcount];
                child.localScale = newScale;
                downcount++;
                      }
        }
    }


    float[] NormalizeAndScaleValues(float[] values)
    {
        // Trova il minimo e il massimo tra i valori
        float minValue = Mathf.Min(values);
        float maxValue = Mathf.Max(values);

        // Normalizza ogni valore nell'intervallo da 0 a 1
        float[] normalizedValues = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            normalizedValues[i] = (values[i] - minValue) / (maxValue - minValue);
        }

        // Scala i valori nell'intervallo da 0 a 1.6
        for (int i = 0; i < normalizedValues.Length; i++)
        {
            normalizedValues[i] *= 0.8f;
        }

        return normalizedValues;
    }

}
