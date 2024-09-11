using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Boolean visualizeFeatures = false;
    public GameObject Synth;
    public Boolean IsRecorded=false;
    public GameObject GridDisplay;
    [SerializeField]
    public DataToStore PoseL;
    [SerializeField]
    public DataToStore PoseR;
    public GameObject actualPose;
   


    private List<float> poseLtoList;
    private List<float> poseRtoList;

    ConsoleVRScript ConsoleManager;
    private int errorL = 0;
    private int errorR = 0;
    private int totalerror = 0;
    private float[] refPose;
    private GameObject PoseDisplay;


    string posenote;
    // Start is called before the first frame update

    /// </summary>
    // Update is called once per frame
 

    private void Start()
    {
       
        Synth = GameObject.FindGameObjectWithTag("Synth");
        ConsoleManager = GameObject.FindGameObjectWithTag("ConsoleManager").GetComponent<ConsoleVRScript>();
        posenote = gameObject.name.Substring(4);
        PoseDisplay = GridDisplay.transform.Find(gameObject.name+"v").gameObject;

        FindTextMeshPro(PoseDisplay.transform.Find("BoxName")).text=gameObject.name;
        if (IsRecorded)
        {
            FindTextMeshPro(PoseDisplay.transform.Find("BoxName")).color = Color.cyan;
        }


    }

    void Update()
    {
        if (IsRecorded)
        {
            //ConsoleManager.clearDisplay("C");
            refPose = actualPose.GetComponent<HandDataRetriever>().ArrayDistance;
            /*foreach (var pose in refPose) 
            {
                //Debug.Log(pose.ToString());
               ConsoleManager.PrintOnDisplay(pose.ToString()+"\n", "C");
            }
            */


            


            NoteChecker();
            
        }
        


    }


    private void NoteChecker()
    {
        //ConsoleManager.clearDisplay("C");
        poseLtoList = PoseL.FeatureLists();
        poseRtoList = PoseR.FeatureLists();

        
        foreach (var pose in poseLtoList)
        {
           // ConsoleManager.PrintOnDisplay(pose.ToString()+"\n","C");
        }
        float T = 0.01f;
        float ThumbTh = 0.01f;
        errorL = JointDistanceCheker(1, poseLtoList, refPose, T,ThumbTh);
        errorR= JointDistanceCheker(0, poseRtoList, refPose, T,ThumbTh);




        totalerror = errorL + errorR;  

        if ((totalerror == 0) && (actualPose.GetComponent<HandDataRetriever>().isPlaying == false))
        {
            Debug.Log("Note " + posenote);
            FindTextMeshPro(PoseDisplay.transform.Find("BoxName")).color=Color.green;
            ConsoleManager.PrintOnDisplay("nota"+ posenote + "\n", "C");
            Synth.GetComponent<ProceduralAudioOscillator>().changeNote(int.Parse(posenote));
            actualPose.GetComponent<HandDataRetriever>().isPlaying = true;
            actualPose.GetComponent <HandDataRetriever>().playingNote = int.Parse(posenote);
            
        }
        else if((actualPose.GetComponent<HandDataRetriever>().playingNote == int.Parse(posenote))&& totalerror != 0)
        {
            Debug.Log("ehi stoppo " + posenote);
            FindTextMeshPro(PoseDisplay.transform.Find("BoxName")).color = Color.cyan;
            ConsoleManager.PrintOnDisplay("ERRORI "+totalerror+ "\n", "C");
            actualPose.GetComponent<HandDataRetriever>().isPlaying = false;
            actualPose.GetComponent<HandDataRetriever>().playingNote = 24;
            Synth.GetComponent<ProceduralAudioOscillator>().changeNote(24);

        }

    }
    //actually saving label/right/left
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Hand">The hand 0=right, 1=left</param>
    /// <param name="features">The features of the note to check </param>
    /// <param name="valueToCheck">The actual features</param>
    /// <param name="threshold">the threshold</param>
    /// <param name=""></param>
    private int JointDistanceCheker(int Hand,List<float> features, float[] valueToCheck,float threshold,float ThumbT)
    {
        //ConsoleManager.PrintOnDisplay("In note checker ---", "C");
        int errorcounter = 0;
        if(Hand == 0)
        {
            for(int i=0;i<24; i++)
            {
                float localT = threshold;
                if (i < 4)
                {
                    localT = ThumbT;
                }
                   
                if (visualizeFeatures) //right
                {
                    //ConsoleManager.PrintOnDisplay(valueToCheck[i].ToString() + "---", "C");
                    //ConsoleManager.PrintOnDisplay(features[i].ToString() + "\n", "C");
                    FindTextMeshPro(PoseDisplay.transform.Find("Box (" + i + ")")).text = "" + valueToCheck[i];
                    //Debug.Log(features[i]);
                    FindTextMeshPro(PoseDisplay.transform.Find("Box (" + i + ")")).color = Color.green;
                    //if ((valueToCheck[i] < (1 - threshold) * features[i]) || (valueToCheck[i] > (1 + threshold) * features[i]))
                }
                if (valueToCheck[i] >  (features[i] + localT) || (valueToCheck[i] < ( features[i]- localT)))
                {
                    if (visualizeFeatures)
                        FindTextMeshPro(PoseDisplay.transform.Find("Box (" + i + ")")).color = Color.red;
                    
                    errorcounter++;
                }
            }
        }
        else if(Hand == 1) //left
        {


            for (int i=24; i<=47; i++)
            {

                float localT = threshold;
                if (i >= 24 && i < 28 )
                {
                    localT = ThumbT;
                }
                // ConsoleManager.PrintOnDisplay(valueToCheck[i].ToString() + "---", "C");

                //ConsoleManager.PrintOnDisplay(features[i - 24].ToString() + "\n", "C");
                if (visualizeFeatures)
                {
                    //if ((valueToCheck[i] < (1 - threshold) * features[i-24]) || (valueToCheck[i] > (1+threshold) * features[i-24]))
                    FindTextMeshPro(PoseDisplay.transform.Find("Box (" + (i) + ")")).text = "" + valueToCheck[i];
                    FindTextMeshPro(PoseDisplay.transform.Find("Box (" + (i) + ")")).color = Color.green;
                }
                if (valueToCheck[i] >  (features[i-24] + localT) || (valueToCheck[i-24] < (features[i-24]- localT )))
                {
                    if (visualizeFeatures)
                        FindTextMeshPro(PoseDisplay.transform.Find("Box (" + (i)+ ")")).color = Color.red;
                    errorcounter++;
                }

            }
        }
        
        return errorcounter;
    }


    TextMeshPro FindTextMeshPro(Transform currentTransform)
    {
        
        TextMeshPro textMeshPro = currentTransform.GetComponent<TextMeshPro>();
        if (textMeshPro != null)
        {
           
            return textMeshPro;
        }

       
        foreach (Transform childTransform in currentTransform)
        {
            TextMeshPro foundTextMeshPro = FindTextMeshPro(childTransform);
            if (foundTextMeshPro != null)
            {
                
                return foundTextMeshPro;
            }
        }

       
        return null;
    }

    private void OnDestroy()
    {
          GameObject Synth;
     Boolean IsRecorded = false;
    
     DataToStore PoseL = null;
    
     DataToStore PoseR=null;
     GameObject actualPose = null;


     List<float> poseLtoList=null;
     List<float> poseRtoList= null;

    ConsoleVRScript ConsoleManager=null;
     int errorL = 0;
     int errorR = 0;
     int totalerror = 0;
     float[] refPose=null;
}
}
