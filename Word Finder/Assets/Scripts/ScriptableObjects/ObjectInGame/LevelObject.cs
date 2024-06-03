using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject
{
    public string Level { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public List<WordObject> Words { get; set; }
}
