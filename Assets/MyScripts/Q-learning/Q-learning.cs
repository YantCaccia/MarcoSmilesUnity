using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;
using System.Text;
using System.Threading;


public class QLearningAgent : MonoBehaviour
{
    private int label_number=0;
    public List<List<float>> features = new List<List<float>>();
    public List<float> distanceArray = new List<float>();
    public List<int> labelArray = new List<int>();
    public float[] actualFeatures;
    private int note_label;


    private string path;
    public static string filename = "marcosmiles_dataset.csv";
    static string folderName = "MyDatasets";
    public static string defaultFolder = "DefaultDataset";
    public static string selectedDataset = defaultFolder;
    [SerializeField]
    private int epochs = 2000;
    string Adatasetpath = "";


    private void Start()
    {
        Debug.Log("onstart");
        //read the dataset to start the train
        path = Application.persistentDataPath;
        Adatasetpath = $"{path}/{folderName}/{selectedDataset}/augmented_dataset.csv";
        RetrieveDataset();
        Debug.Log("label number:" + label_number);


      
        


    }

    public void StartTraining()
    {
        Debug.Log("clickedddddd");
        for (int i = 0; i < epochs; i++)
        {
            Debug.Log("EPOCHS " + i);
            
            for (int j = 0; j < label_number; j++)
            {
                Debug.Log("we are training");
                note_label = labelArray[j];
                Debug.Log(note_label);
                Debug.Log(features[j].ToString());
                Train(createTrainState(features[j]));
                System.Threading.Thread.Sleep(1);


            }
            

        }
        
    }

    public State createTrainState(List<float> distanceArray)
    {
        //save first left and then right so we need to retrieve like this

        DataToStore LHand = new DataToStore("left", distanceArray[0], distanceArray[1], distanceArray[2], distanceArray[3], distanceArray[4],
                                           distanceArray[5], distanceArray[6], distanceArray[7], distanceArray[8], distanceArray[9],
                                           distanceArray[10], distanceArray[11], distanceArray[12], distanceArray[13], distanceArray[14],
                                           distanceArray[15], distanceArray[16], distanceArray[17], distanceArray[18], distanceArray[19],
                                           distanceArray[20], distanceArray[21], distanceArray[22], distanceArray[23]);

        DataToStore RHand = new DataToStore("right", distanceArray[24], distanceArray[25], distanceArray[26], distanceArray[27], distanceArray[28],
                                    distanceArray[29], distanceArray[30], distanceArray[31], distanceArray[32], distanceArray[33],
                                    distanceArray[34], distanceArray[35], distanceArray[36], distanceArray[37], distanceArray[38],
                                    distanceArray[39], distanceArray[40], distanceArray[41], distanceArray[42], distanceArray[43],
                                    distanceArray[44], distanceArray[45], distanceArray[46], distanceArray[47]);

        int note = -1;
        return new State(LHand,RHand,note);
    }






    // Define Q-table
    Dictionary<State, Dictionary<Action, float>> QTable = new Dictionary<State, Dictionary<Action, float>>();

    // Q-learning parameters
    float alpha = 0.1f; // learning rate
    float gamma = 0.9f; // discount factor 
    //the probability of the agent to chose a new action or to take the best action 
    float epsilon = 0.1f; // exploration-exploitation trade-off

    // Training loop of a single state
    private void Train(State state)
    {
        State currentState = state;
        Debug.Log(currentState.ToString());
        while (!IsTerminalState(currentState))
        {
            // Choose action using epsilon-greedy policy
            Action chosenAction = EpsilonGreedyPolicy(currentState);

            // Take action and observe new state and reward
            (State newState, float reward) = TakeAction(chosenAction,currentState);

            // Update Q-value
            UpdateQValue(currentState, chosenAction, newState, reward);

            currentState = newState;
        }
        
    }

    // Epsilon-greedy policy
    Action EpsilonGreedyPolicy(State currentState)
    {
        // Choose random action with probability epsilon, otherwise choose the best action
        if (UnityEngine.Random.value < epsilon)
        {
            return new Action(RandomNote());
        }
        else
        {
            return GetBestAction(currentState);
        }
    }

    // Update Q-value
    void UpdateQValue(State currentState, Action chosenAction, State newState, float reward)
    {
        if (!QTable.ContainsKey(currentState))
        {
            QTable[currentState] = new Dictionary<Action, float>();
        }

        float currentQValue = QTable[currentState].ContainsKey(chosenAction) ? QTable[currentState][chosenAction] : 0f;
        float maxFutureQValue = GetMaxQValue(newState);

        float newQValue = (1 - alpha) * currentQValue + alpha * (reward + gamma * maxFutureQValue);

        QTable[currentState][chosenAction] = newQValue;
    }

    // Take action and observe new state and reward 
    private (State, float) TakeAction(Action action,State state)
    {
        // Simulate the environment dynamics (simplified for illustration purposes)
        int new_note = action.chosednote;

        State newState = state;
        newState.set_note(new_note);
        float reward=0;
        // Calculate reward (simplified for illustration purposes)
        if (IsTerminalState(newState))
        {
            reward = 2f;
        }
        


        return (newState, reward);
    }

    // Get best action for a given state
    private Action GetBestAction(State state)
    {
        if (QTable.ContainsKey(state))
        {
            float maxQValue = float.MinValue;
            Action bestAction = null;

            foreach (var kvp in QTable[state])
            {
                if (kvp.Value > maxQValue)
                {
                    maxQValue = kvp.Value;
                    bestAction = kvp.Key;
                }
            }

            return bestAction;
        }

        // If state is not in QTable, explore by choosing a random action
        return new Action(RandomNote());
    }

    // Get maximum Q-value for a given state
    private float GetMaxQValue(State state)
    {
        if (QTable.ContainsKey(state))
        {
            float maxQValue = float.MinValue;

            foreach (var kvp in QTable[state])
            {
                if (kvp.Value > maxQValue)
                {
                    maxQValue = kvp.Value;
                }
            }

            return maxQValue;
        }

        return 0f;
    }

    // Check if a state is terminal (simplified for illustration purposes)
    private bool IsTerminalState(State state)
    {
        if (state.note == note_label)
        {
            Debug.Log("reached terminal state " + note_label);
            return true;
        }
        return false;
        
    }

    // Utility function to get a random note 
    private int RandomNote()
    {
        return UnityEngine.Random.Range(0,25);
    }


 



    private void RetrieveDataset()
    {



        // Read data from dataset 
        string[] lines = File.ReadAllLines(Adatasetpath);

        // Iterate the csv FILE 
        foreach (string line in lines)
        {
            // split columns 
            string[] columns = line.Split(',');
            Boolean Label = true;
            distanceArray = new List<float>();
            foreach (string column in columns)
            {
                if (Label)
                {
                    label_number++;
                    int note = int.Parse(column);
                    labelArray.Add(note);
                    Debug.Log("retrieved " + note);
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
            features.Add(distanceArray);
        }

    }



    private void SaveQTableToCSV(string filePath)
    {
        StringBuilder csvContent = new StringBuilder();

        // Header
        csvContent.AppendLine("State,Action,QValue");

        // Data
        foreach (var state in QTable.Keys)
        {
            foreach (var action in QTable[state].Keys)
            {
                float qValue = QTable[state][action];
                csvContent.AppendLine($"{state},{action},{qValue}");
            }
        }

        // Write to file
        File.WriteAllText(filePath, csvContent.ToString());

        Debug.Log("QTable saved to CSV: " + filePath);
    }

    // Chiamare questo metodo quando si desidera salvare la QTable (ad esempio, al termine delle iterazioni)
    private void SaveQTable()
    {
        string filePath = $"{path}/{folderName}/{selectedDataset}/QTable.csv";
        if (File.Exists(filePath)){
            File.Delete(filePath);
        }
        
        SaveQTableToCSV(filePath);
    }
}