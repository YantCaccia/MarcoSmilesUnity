using System;
using System.Collections.Generic;
using System.IO;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.optim;
using static TorchSharp.torch.nn;
using TorchSharp.Modules;
using SkiaSharp;
using UnityEngine;
using static TorchSharp.TensorExtensionMethods;
using System.Linq;

// Definizione del modello DQN
public class DQN
{

    public Sequential fc;
    
    public DQN(int inputDim, int outputDim)
    {
        fc = Sequential(
            (Linear(inputDim, 128)),
            ReLU(),
            Linear(128, 256),
            ReLU(),
            Linear(256, 128),
            ReLU(),
            Linear(128, outputDim)
        );
    }

    public Tensor forward(Tensor input)
    {
        Tensor qvals = fc.forward(input);
        return qvals;
    }
}

// Definizione dell'agente DQN
public class DQNAgent
{   
    
    public DQN model;
    private DQN targetModel;
    private Optimizer optimizer;
    private double gamma;
    private double tau;
    //private BasicBuffer replayBuffer;
    public Device device;
    private MSVenv env;
    public BasicBuffer replay_buffer;
   
    public DQNAgent(MSVenv env, double learningRate = 3e-4, double gamma = 0.99, int bufferSize = 10000, double tau = 0.005)
    {
        this.env = env;
        this.device = torch.CPU;
        this.replay_buffer = new BasicBuffer(bufferSize);   
        //creating online network
        this.model = new DQN(48, env.action_space.Length);
        //creating target network
        this.targetModel = new DQN(48, env.action_space.Length);

        this.gamma = gamma;
        this.tau = tau;

        this.optimizer = new Adam(this.model.fc.parameters(), learningRate);

        // Copy of the neural network 
        targetModel.fc=model.fc;
       
    }
   
    public int GetAction(Tensor state, double eps = 0.20)
    {
        var qvals = model.forward(state);
        var action = qvals.argmax().detach();
        //Debug.Log("THAT IS THE ACTION "+ action.ToInt16());

        double randomFloat = UnityEngine.Random.Range(0f, 1f);

        //Debug.Log("RANDOM EPS: " + randomFloat);
        if (randomFloat < eps)
        {
            //Debug.Log("chosed randomly");
            return UnityEngine.Random.Range(0, 12); ;
        }

        //Debug.Log("chosed from experience");

        return action.ToInt16();
    }

    public Tensor ComputeLoss(Tensor states, Tensor actions, Tensor rewards, Tensor nextStates, Tensor dones)
    {
        states = torch.FloatTensor(states).to(device);
        actions = torch.LongTensor(actions).to(device);
        rewards = torch.FloatTensor(rewards).to(device);
        nextStates = torch.FloatTensor(nextStates).to(device);
        dones = torch.FloatTensor(dones).to(device);
        /*
        // Ottieni la forma del tensore e stampala per ciascun tensore
        var stateShape = states.shape;
        Debug.Log("Forma di 'states': [" + string.Join(", ", stateShape) + "]");

        var actionShape = actions.shape;
        Debug.Log("Forma di 'actions': [" + string.Join(", ", actionShape) + "]");

        var rewardShape = rewards.shape;
        Debug.Log("Forma di 'rewards': [" + string.Join(", ", rewardShape) + "]");

        var nextStateShape = nextStates.shape;
        Debug.Log("Forma di 'nextStates': [" + string.Join(", ", nextStateShape) + "]");

        var doneShape = dones.shape;
        Debug.Log("Forma di 'dones': [" + string.Join(", ", doneShape) + "]");
        */

        // Ridimensiona i tensori
        actions = actions.view(actions.size());
        dones = dones.view(dones.size());
        
        /*
        string vector="[";
        // Stampa gli elementi di actions
        Debug.Log("________ACTIONS______:");
        for (int i = 0; i < actions.size(0); i++)
        {
            
            vector = vector + "  " + (actions[i].item<long>());
        }
        Debug.Log("Tensore :"+ vector + "]");

        */


        // Calcolo della perdita
        //var currQ = model.forward(states);
        var currQ = model.forward(states).gather(1, actions.view(actions.size(0), 1));
        var nextQ = targetModel.forward(nextStates);
        //the var _ is ignored
        var (maxNextQ, _) = nextQ.max(1); // Get the max values along dimension 1
        maxNextQ = maxNextQ.view(maxNextQ.size(0), 1); // Ensure correct shape
        

        var expectedQ = rewards + (1 - dones) * gamma * maxNextQ;

        var mseLoss = new MSELoss();
        var loss = mseLoss.forward(currQ, expectedQ);
        //Debug.Log("LOSS :  " +loss);
        return loss;
    }
    
    public void Update(int batchSize)
    {
        var batch = replay_buffer.Sample(batchSize);
        var loss = ComputeLoss(batch.Item1, batch.Item2, batch.Item3, batch.Item4, batch.Item5);

        optimizer.zero_grad();
        loss.backward();
        optimizer.step();


        var modelParameters = model.fc.parameters().ToList();
        var targetParameters = targetModel.fc.parameters().ToList();


        /*
        // Aggiornamento dei parametri della rete target
        for (int i = 0; i < modelParameters.Count(); i++)
        {
            var param = modelParameters[i];
            var targetParam = targetParameters[i];

            var updatedData = tau * param + (1 - tau) * targetParam;
            // Calcola i nuovi valori dei parametri
            Parameter par = new Parameter(updatedData);
            targetParam=par;
            
        }
        */
        targetModel.fc = model.fc;
       
    }
    public void SaveModel()
    {
        var model_parameters = model.fc.parameters();      // Save model parameters to a file
        using (var writer = new BinaryWriter(File.Open("model_parameters.bin", FileMode.Create)))
        {
            foreach (var parameter in model_parameters)
            {
                var tensor = parameter; 
                var data = tensor.cpu().data<float>(); // Move tensor to CPU before extracting data
                foreach (var value in data)
                {
                    writer.Write(value);
                }
            }
        }
    }
    
}

