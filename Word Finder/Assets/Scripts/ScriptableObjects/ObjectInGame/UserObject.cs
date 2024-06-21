using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserObject
{
    public string UserId {  get; set; }
    public string Username { get; set; }
    public Dictionary<string, UserScoreObject> CategoryScore { get; set; } = new Dictionary<string, UserScoreObject>();


    public override string ToString()
    {
        string s = Username + " ";
        foreach (var item in CategoryScore)
        {
            s = s + " " + item.Value.ScoreOfCategory;
        }
        return s;
    }
}
