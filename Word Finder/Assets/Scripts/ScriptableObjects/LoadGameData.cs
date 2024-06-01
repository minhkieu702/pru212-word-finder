using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

[SerializeField]
[CreateAssetMenu]
public class LoadGameData : ScriptableObject
{
    // Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;

    [Header("GameData")]
    public List<LevelObject> boardObjects;

    async void Awake()
    {
        // Initialize Firebase
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Database");
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async Task<List<LevelObject>> GetGameData(string gameid)
    {
        try
        {
            var DbTask = DBreference.Child("game").GetValueAsync();
            await DbTask;

            if (DbTask.Result.Value == null)
            {
                Debug.LogError("No data about game");
                return boardObjects;
            }
            List< LevelObject> boardObject = null;
            DataSnapshot dataSnapshot = DbTask.Result;
            foreach (var category in dataSnapshot.Children)
            {
                if (category.Key == gameid)
                {
                    //boardObject = CreateBoard(category);
                    break; // Exit the loop once the desired game data is found
                }

            }
            return boardObject;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return null;
        }
    }

    public LevelObject CreateBoard(DataSnapshot snapshot)
    {
        var boardObject = new LevelObject();
        List<WordObject> words = new List<WordObject>();

        foreach (var item in snapshot.Children)
        {
            /*switch (item.Key)
            {
                case "category":
                    boardObject.Category = item.Value.ToString();
                    break;
                case "board":
                    var board = item.Value as Dictionary<string, object>;

                    if (board.ContainsKey("grid"))
                    {
                        var grid = board["grid"] as Dictionary<string, object>;
                        if (grid.ContainsKey("column"))
                        {
                            boardObject.Column = int.Parse(grid["column"].ToString());
                        }
                        if (grid.ContainsKey("row"))
                        {
                            boardObject.Row = int.Parse(grid["row"].ToString());
                        }
                    }

                    if (board.ContainsKey("word"))
                    {
                        var wordData = board["word"] as Dictionary<string, object>;
                        foreach (var wordEntry in wordData)
                        {
                            string wordKey = wordEntry.Key;
                            var wordValue = wordEntry.Value as Dictionary<string, object>;

                            var word = new WordObject();
                            word.Word = wordKey;

                            if (wordValue.ContainsKey("endColumn"))
                            {
                                word.endColumn = int.Parse(wordValue["endColumn"].ToString());
                            }
                            if (wordValue.ContainsKey("endRow"))
                            {
                                word.endRow = int.Parse(wordValue["endRow"].ToString());
                            }
                            if (wordValue.ContainsKey("startColumn"))
                            {
                                word.startColumn = int.Parse(wordValue["startColumn"].ToString());
                            }
                            if (wordValue.ContainsKey("startRow"))
                            {
                                word.startRow = int.Parse(wordValue["startRow"].ToString());
                            }

                            words.Add(word);
                        }
                    }
                    break;
            }*/
        }
        boardObject.Words = words;
        return boardObject;
    }
}
