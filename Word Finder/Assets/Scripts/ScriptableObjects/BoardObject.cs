using System;
using System.Collections;
using System.Collections.Generic;

public class BoardObject
{
    public string Category { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public List<WordObject> Words { get; set; }

    //initialize object class with jsonString parameter
    public BoardObject()
    {
        
    }
    //public BoardObject(string jsonString)
    //{
    //    Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
    //    Dictionary<string, object> board = ((JObject)data["board"]).ToObject<Dictionary<string, object>>();
    //    Dictionary<string, object> grid = ((JObject)board["grid"]).ToObject<Dictionary<string, object>>();

    //    Category = data["category"].ToString();
    //    Row = Convert.ToInt32(grid["row"]);
    //    Column = Convert.ToInt32(grid["column"]);

    //    Dictionary<string, object> words = ((JObject)board["word"]).ToObject<Dictionary<string, object>>();
    //    List<WordObject> list = new List<WordObject>();

    //    foreach (KeyValuePair<string, object> word in words)
    //    {
    //        string wordKey = word.Key;
    //        Dictionary<string, object> wordValue = ((JObject)word.Value).ToObject<Dictionary<string, object>>();

    //        list.Add(new WordObject()
    //        {
    //            endColumn = Convert.ToInt32(wordValue["endColumn"]),
    //            startColumn = Convert.ToInt32(wordValue["startColumn"]),
    //            endRow = Convert.ToInt32(wordValue["endRow"]),
    //            startRow = Convert.ToInt32(wordValue["startRow"]),
    //            Word = wordKey
    //        });
    //    }
    //    Words = list;
    //}
}
