using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.XR.CoreUtils;
using Unity.VisualScripting;
using TMPro;
using System;
using JetBrains.Annotations;

public class CheckCorrectNote : Agent
{
    public int note;
    public Boolean PlayModeOn = false;
    public Material green;
    public Material red;
    public Material orange;

    private int episodecounter = 0;
    [SerializeField] List<float> dArray=new List<float>();
    private List<List<float>> features = new List<List<float>>();
    public GameObject DatasetLoader;
    private float[] RefPose;
    private int _note;
    public GameObject cube;
    public TextMeshPro text;

    private List<int> labelArray= new List<int>();
    private int labelRef;
    private int featurescounter=0;
    
    //private ConsoleVRScript Console;

    private void Start()
    {
       
        features = DatasetLoader.GetComponent<DatasetLoader>().features;
        labelArray = DatasetLoader.GetComponent<DatasetLoader>().labelArray;

    }
    public override void OnEpisodeBegin()
    {
        if (!PlayModeOn)
        {
            
            labelArray = DatasetLoader.GetComponent<DatasetLoader>().labelArray;

            features = DatasetLoader.GetComponent<DatasetLoader>().features;
            dArray = features[featurescounter];
            labelRef = labelArray[featurescounter];
            featurescounter++;
            if (featurescounter > 1499)
            {
                featurescounter = 0;
                
            }


            episodecounter++;
            //Debug.Log("EPISODE " + episodecounter);
           
        }
        if (PlayModeOn)
        {
            RefPose = DatasetLoader.GetComponent<DatasetLoader>().actualFeatures;
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (!PlayModeOn)
        {
            dArray = DatasetLoader.GetComponent<DatasetLoader>().distanceArray;
            foreach (var d in dArray)
            {
                sensor.AddObservation(d);
            }
        }
        else
        {
            Debug.Log(RefPose.Length);
            for(int i = 0; i < RefPose.Length; i++)
            {
                sensor.AddObservation(RefPose[i]);
            }
        }
        //sensor.AddObservation(note);
    }
    int counter = 0;
    public override void OnActionReceived(ActionBuffers actions)
    {
        /*int step = 0;
        int direction = 1;
        if (actions.DiscreteActions[0] == 0)
                    step = 1; //semitone
        else
                    step = 2; //TONE


        if (actions.DiscreteActions[1] == 0)
            direction = 1;
        else
            direction = 1;
            

        note += step * direction;
        if (note < 0)
        {
            note += 25;
        }
        note = note % 25;
        */
        note = actions.DiscreteActions[0];
        text.text = note.ToString();
        Debug.Log("note "+note);

        //Debug.Log("onMyIntChanged");
        if (note == labelRef)
        {
            Debug.Log("GOT REWARD");
            AddReward(100f);
            cube.GetComponent<MeshRenderer>().material = green;
            EndEpisode();
        }

        else
        {
            cube.GetComponent<MeshRenderer>().material = red;
            AddReward(-100f);
            counter++;
            EndEpisode();





        }




    }



    /*
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
        if (note == labelRef)
        {
            Debug.Log("GOT REWARD");
            AddReward(100f);
            
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
    */

    
}
