using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TorchSharp;
using Unity.Properties;
using UnityEngine;

public class Metrics
{
    public Metrics() { }


    public double Accuracy(List<int> true_labels, List<int> predicted_labels)
    {
        int correct_pred = true_labels.Zip(predicted_labels, (truel, predl) => truel == predl).Count(isTrue => isTrue);
        return (double)correct_pred / true_labels.Count;
    } 

    public double  Precision(List<int> trueLabels, List<int> predictedLabels)
    {
        var distinct = trueLabels.Distinct().ToList();
        double  sum = 0;
        foreach (int x in distinct)
        {

            int tp = trueLabels.Zip(predictedLabels, (truel, predl) => truel == predl && truel==x).Count(isTrue => isTrue);
            
            int fp = trueLabels.Zip(predictedLabels, (truel, predl) => truel != predl && predl == x).Count(isTrue => isTrue);
            
            double precision = ((double)tp / (tp + fp));
            
            sum = sum + precision;
        }
        
        return sum/distinct.Count;

    }

    public double Recall(List<int> trueLabels, List<int> predictedLabels)
    {
        var distinct = trueLabels.Distinct().ToList();
        double sum = 0;
        
        Debug.Log(distinct.Count);
        foreach (int x in distinct){
            
            
            int tp = trueLabels.Zip(predictedLabels, (truel, predl) => truel == predl && predl == x).Count(isTrue => isTrue);
            int fn = trueLabels.Zip(predictedLabels, (truel, predl) => truel != predl && predl == x).Count(isTrue => isTrue);
            double recall = ((double)tp / (tp + fn));
            sum = sum + recall;
        }
        
        return  sum/distinct.Count;
    }

    public double F1Score(List<int> trueLabels, List<int> predictedLabels)
    {
        double precision = Precision(trueLabels, predictedLabels);
        double recall = Recall(trueLabels, predictedLabels);

        return (2 * (precision * recall)) / (precision + recall);
    }

}
