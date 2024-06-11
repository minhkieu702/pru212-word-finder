using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Linq;

[SerializeField]
[CreateAssetMenu]
public class LoadGameData : ScriptableObject
{
    // Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;

    [Header("GameData")]
    public Dictionary<string, List<LevelObject>> GameData = new();

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
        DBreference ??= FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async Task<Dictionary<string, List<LevelObject>>> AssignGameData()
    {
        try
        {
            InitializeFirebase();
            var DBTask = await DBreference.Child("game").GetValueAsync();
            if (DBTask.Value == null)
            {
                return null;
            }
            return DBTask.Children.ToDictionary(
                cate => cate.Key,
                cate => cate.Children.Select(
                    level => new LevelObject
                    {
                        Level = level.Key,
                        Column = int.Parse(level.Child("grid").Child("column").Value.ToString()),
                        Row = int.Parse(level.Child("grid").Child("row").Value.ToString()),
                        Words = level.Child("word").Children.Select(
                            word => new WordObject
                            {
                                Word = word.Key,
                                endColumn = int.Parse(word.Child("endColumn").Value.ToString()),
                                startColumn = int.Parse(word.Child("startColumn").Value.ToString()),
                                endRow = int.Parse(word.Child("endRow").Value.ToString()),
                                startRow = int.Parse(word.Child("startRow").Value.ToString()),
                            }
                            ).ToList()
                    }
            ).ToList());
        }
        catch (Exception)
        {

            throw;
        }
    }
}
