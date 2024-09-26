using Google.Protobuf.Reflection;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TorchSharp;
using UnityEngine;
using TMPro;
using System.Transactions;
using System.Linq;

public class Training : MonoBehaviour
{
    private List<int> labels;
    private List<float[]> features;
    private MSVenv env;
    private DQNAgent DQNagent;


    public TextMeshProUGUI console;
    public TextAsset dataset;
    public TextAsset testdataset;




    private bool started = false;
    public int batchsize = 64;
    public int epocs = 10;
    public int maxsteps = 10;
    private List<List<int>> epoch_rewards = new List<List<int>>();
    private List<int> episode_rewards= new List<int>();
    private int episodes;
    private BasicBuffer buffer;

    private Metrics metrics;

    // Start is called before the first frame update
    void Start()
    {
        (labels, features) = load_CSV_file(dataset);
        //Debug.Log("LABELS N = " + labels.Count);

       
        /*
        //Debug.Log("FEATURES N = " +features.Count); 
        for (int i = 0; i < labels.Count; i++)
        {
            //console.text = " " + i;
            
            //Debug.Log(labels[i].ToString());
        }
        for (int i = 0; i < features.Count; i++) {
            for (int j = 0; j < 48; j++)
            {
                //Debug.Log(features[i][j].ToString());
            }
        }
        */

    }
    void Update()
    {
        
        if (!started) {
            Debug.Log("Starting cioroutine");
            started = true;
            StartCoroutine(start_train_coroutine());
        }
        

    }
    IEnumerator start_train_coroutine() {
        console.text = console.text + "STARTED TRAINING \n";
            //creating new env
        env = new MSVenv(features, labels);
        console.text= console.text + "ENV created -AGENT created "+"\n";
        Debug.Log("ENV CREATED");
        DQNagent = new DQNAgent(env);
        Debug.Log("DQN AGENT CREATED");
        episodes = features.Count;
        
        epocs = 20;

        
            //starting training phase 
            for (int e = 0; e < epocs; e++)
            {
            
            DQNagent.model.fc.load("C:\\Users\\Daniele\\Desktop\\models\\model_weigths_ACC84,0055555555556%.dat");
            DQNagent.model.fc.train();
            console.text = console.text + "EPOCS " + e +"\n"; ;
                for (int episode = 0; episode < episodes; episode++)
                {
                //Debug.Log("EPISODE " + episode);
                //console.text = console.text + "EPISODE " + episode + "\n"; 
                float[] state = env.reset();
                    int episode_reward = 0;

                yield return new WaitForSeconds(0);

                for (int step = 0; step < maxsteps; step++)
                    {
                       
                        int action = DQNagent.GetAction(state);
                        
                        //Debug.Log("ACTION " + action);
                        
                        (float[] next_state, int reward, bool done, object _) = env.Step(action);
                    
                    DQNagent.replay_buffer.Push(state, action, reward, next_state, done);
                    episode_reward += reward;
                    



                    if (DQNagent.replay_buffer.Length() > batchsize)
                        {
                        //Debug.Log("UPDATING");
                            DQNagent.Update(batchsize);
                        }
                    

                    if ((done) || step == (maxsteps - 1))
                        {
                        
                        episode_rewards.Add(episode_reward);
                        //console.text = ("Episode: " + (episode + 1) + " , total reward: " + episode_reward);
                        Debug.Log("Episode: " + (episode + 1) + " , total reward: " + episode_reward);

                        break;
                    }
                    
                    state = next_state;
                    


                   
                    
                    }
                    //Debug.Log(episode_rewards);
                    epoch_rewards.Add(episode_rewards);

                }
            //console.text=""+epoch_rewards;
            //Debug.Log(epoch_rewards);

            StartTesting();
            
        }
        
        

        
        //started = false;
        yield break;
    }

    private void StartTesting()
    {
        List<int> predicted_labels = new List<int>();

        DQNagent.model.fc.eval();
        

        List<int> labels_test = new List<int>();
        List<float[]> features_test = new List<float[]>();

        (labels_test, features_test) = load_CSV_file(testdataset);

       for (int i = 0; i < labels_test.Count; i++)
        {
            var feat = torch.FloatTensor(features_test[i]).to(DQNagent.device);

            var predicted_qvals = DQNagent.model.forward(feat);

            var predicted_action= torch.argmax(predicted_qvals.cpu().detach());

            //Debug.Log("Predicted action =" + predicted_action);

            predicted_labels.Add(predicted_action.ToInt32());
        
        }
        //Debug.Log("LABEL TEST SIZE " + labels_test.Count);
        /*
        int tp = 0;         
        for (int i=0;i < labels_test.Count-1;i++)
        {
            //Debug.Log(i);
            //Debug.Log("true: "+ labels_test[i] +" -- predl:" + predicted_labels[i]);
            if (labels_test[i] == predicted_labels[i])
            {
                tp++;
            }
        }
        */
       
        
      
        

        metrics =new Metrics();
        Debug.Log("ACCURACY: "+metrics.Accuracy(labels_test, predicted_labels));
        console.text=console.text+ "ACCURACY: "+metrics.Accuracy(labels_test, predicted_labels)+"\n";
        Debug.Log("PRECISION: " + metrics.Precision(labels_test, predicted_labels));
        Debug.Log("RECALL: " + metrics.Recall(labels_test, predicted_labels));
        Debug.Log("F1SCORE: " + metrics.F1Score(labels_test, predicted_labels));


        DateTime currentDateTime = DateTime.Now;

        
        Debug.Log("Data e ora correnti: " + currentDateTime);

        // Formatta la data in un modo specifico
        string formattedDate = currentDateTime.ToString("yyyy-MM-dd-HH-mm-ss");
        

        DQNagent.model.fc.save("C:\\Users\\Daniele\\Desktop\\models\\model_"+formattedDate+".dat");


    }


    private (List<int> labels, List<float[]> features) load_CSV_file(TextAsset csv)
    {
        //RECOVER lINES
        string[] lines = csv.text.Split('\n');
        
        //cre ating List for labels (y) and features(y)
        List<int> y = new List<int>();
        List<float[]> x = new List<float[]>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            //Debug.Log("line " + i);
            //ignoring empty row
            //if (string.IsNullOrEmpty(lines[i])) continue;
          
            //Split lines by character ","
            string[] values = lines[i].Split(',');
            //Debug.Log(values[0].ToString());
            //the first item is the label
          
               
            y.Add(int.Parse(values[0].ToString()));
            
            float[] feat = new float[values.Length - 1];
            for (int j = 1; j < values.Length; j++)
            {

               
                feat[j - 1] = float.Parse(values[j].ToString(), CultureInfo.InvariantCulture);
                //console.text = console.text + feat[j - 1];
            }
            x.Add(feat);
        }
        console.text=console.text+"DATASET LOADED \n";
        return (y, x);
    }


    // Definizione della coroutine
    IEnumerator PauseCoroutine(int sec)
    {
     

        // Pausa per 3 secondi
        yield return new WaitForSeconds(sec);

        
    }
}
