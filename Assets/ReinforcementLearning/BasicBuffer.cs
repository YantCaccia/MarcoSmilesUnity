using System;
using System.Collections.Generic;
using System.Linq;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.optim;
using static TorchSharp.torch.nn;
using UnityEngine;
public class BasicBuffer
{
    private int maxSize;
    public Queue<(Tensor state, Tensor action, Tensor reward, Tensor nextState, Tensor done)> buffer;

    public BasicBuffer(int maxSize)
    {
        this.maxSize = maxSize;
        buffer = new Queue<(Tensor, Tensor, Tensor, Tensor, Tensor)>(maxSize);
    }

    public void Push(Tensor state, Tensor action, Tensor reward, Tensor nextState, Tensor done)
    {
        var experience = (state, action, reward, nextState, done);
        //Debug.Log(experience.state.ToString());
        if (buffer.Count >= maxSize)
        {
            buffer.Dequeue();
        }
        
        buffer.Enqueue(experience);
    }

    public (Tensor, Tensor, Tensor, Tensor, Tensor) Sample(int batchSize)
    {
        var stateBatch = torch.empty(new long[] { batchSize, 48 });
        var actionBatch = torch.empty(new long[] { batchSize, 1 });
        var rewardBatch = torch.empty(new long[] { batchSize, 1 });
        var nextStateBatch = torch.empty(new long[] { batchSize, 48 });
        var doneBatch = torch.empty(new long[] { batchSize, 1 });

        var random = new System.Random();
        var batch = buffer.ToList().OrderBy(x => random.Next()).Take(batchSize).ToList();
        //Debug.Log("BATCHSIZE =" + batchSize);
        // Itera attraverso il batch selezionato
        for (int i = 0; i < batchSize; i++)
        {
            
            var experience = batch[i];
            var (state, action, reward, nextState, done) = experience;

            // Aggiunge i valori ai tensori batch
            stateBatch[i] = state;  // Supponendo che lo stato sia un tensore
            actionBatch[i] = action;  // Supponendo che l'azione sia un tensore
            rewardBatch[i] = reward;
            nextStateBatch[i] = nextState;  // Supponendo che il nextState sia un tensore
            doneBatch[i] = done;
        }

        // Restituisce i batch come tuple di tensori
        return (stateBatch, actionBatch, rewardBatch, nextStateBatch, doneBatch);
    }


    public int Length()
    {
        return buffer.Count;
    }
}