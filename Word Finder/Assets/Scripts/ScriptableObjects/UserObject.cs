using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserObject
{
    public string UserId {  get; set; }
    public string Username { get; set; }
    public Dictionary<string, int> ScoreLevel {  get; set; } = new Dictionary<string, int>();
    public override string ToString()
    {
        string s = "";
        s = UserId + " " + Username;
        string temp = "";
        foreach (KeyValuePair<string, int> item in ScoreLevel)
        {
            temp = temp + " " + item.Key + " " + item.Value;
        }
        return s+temp;
    }
}
