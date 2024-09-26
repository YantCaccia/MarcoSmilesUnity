using Google.Protobuf.Reflection;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TorchSharp.Modules;
using UnityEngine;

public class MSVenv
{
    public float[] observation_space = new float[48];
    public int[] action_space = new int[12];
       
    public int feature_index = 0;
    public List<int> labels_array = new List<int>();
    public List<float[]> features = new List<float[]>();
    public float[] state = new float[48];
    public int actual_label;

    public MSVenv(List<float[]> f, List<int> l)
    {
        
        labels_array = l;
        
        features = f;
        actual_label = labels_array[feature_index];
        state = features[feature_index];
    }

    public float[] reset()
    {
       
        if (feature_index == features.Count)
        {
            feature_index = 0;
        }
        state = features[feature_index];
        actual_label = labels_array[feature_index];
        feature_index++;
        return state;
    }

    public (float[], int, bool, object) Step(int action)
    {
        bool done = false;
        int reward;

        if (action == actual_label)
        {
            // Nota raggiunta, azione corretta
            reward = 100;
            done = true;
        }
        else
        {
            reward = 0;
            done = true;
        }

        // Restituisce lo stato, la ricompensa, se l'episodio è terminato, e un oggetto vuoto
        return (state, reward, done, new object());
    }




}




