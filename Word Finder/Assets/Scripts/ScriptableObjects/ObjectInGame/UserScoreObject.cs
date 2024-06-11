using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScoreObject
{
    public Dictionary<string, double> LevelScore { get; set; } = new();

    private double _score;
    public double ScoreOfCategory
    {
        get { return _score; }
        set
        {
            double sum = 0;
            foreach (var item in LevelScore)
            {
                sum += item.Value;
            }
            _score = sum;
        }
    }
}