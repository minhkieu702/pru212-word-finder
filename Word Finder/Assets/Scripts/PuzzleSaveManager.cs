using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Text;
using TMPro;
using Firebase.Extensions;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

public class PuzzleSaveManager : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    private DatabaseReference dbRef;

    public List<BoardData> onlineBoards;

    public BoardData newBoardData;
    public BoardData currentBoardData;

    public static PuzzleSaveManager Instance;

    public async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        //Check that all of the necessary dependencies for Firebase are present on the system
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
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
        if (dbRef == null)
        {
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        }
    }

    public void SaveFileLocal(string fileName, BoardData currentBoardData)
    {
        if (currentBoardData == null)
        {
            currentBoardData = this.currentBoardData;
        }
        string path = Path.Combine(Application.persistentDataPath, (fileName + ".json"));
        string json = JsonUtility.ToJson(currentBoardData);
        File.WriteAllText(path, json);
        Debug.Log("Board data saved to " + path);
    }

    public void LoadFileLocal(string fileName, BoardData currentBoardData)
    {
        if (currentBoardData == null)
        {
            currentBoardData = this.currentBoardData;
        }
        string path = Path.Combine(Application.persistentDataPath, (fileName + ".json"));
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, currentBoardData);
            Debug.Log("Board data loaded from " + path);
        }
        else
        {
            // If no BoardData exists, load template
            string json = JsonUtility.ToJson(newBoardData);
            JsonUtility.FromJsonOverwrite(json, currentBoardData);
            Debug.LogError("No save file found at " + path + ", loaded template instead!");
        }
    }

    public void DeleteFileLocal(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, (fileName + ".json"));
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Board data deleted from " + path);
        }
        else
        {
            Debug.LogError("No save file found at " + path);
        }
    }

    public string RandomString(int size)
    {
        var builder = new StringBuilder(size);

        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (var i = 0; i < size; i++)
        {
            var letter = letters[Random.Range(0, letters.Length - 1)];
            builder.Append(letter);
        }

        return builder.ToString();
    }

    public async void SaveFileCloud()
    {
        if (currentBoardData == null)
        {
            Debug.LogError("currentBoardData is null. Cannot save to cloud.");
            return;
        }

        try
        {
            // Wait until Firebase is initialized
            while (dbRef == null)
            {
                Debug.Log("Waiting for Firebase to initialize...");
                await Task.Delay(100);
            }

            bool usedCode;
            string randomPuzzleCode;

            do
            {
                randomPuzzleCode = RandomString(6);
                Debug.Log("Generated puzzle code: " + randomPuzzleCode);

                var puzzle = await dbRef.Child("puzzles").Child(randomPuzzleCode).GetValueAsync();
                Debug.Log("Puzzle retrieval result: " + (puzzle.Exists ? "Exists" : "Does not exist"));

                usedCode = puzzle.Exists;
            }
            while (usedCode);

            Debug.Log("Final puzzle code: " + randomPuzzleCode);

            string jsonData = JsonUtility.ToJson(currentBoardData);
            await dbRef.Child("puzzles").Child(randomPuzzleCode).SetRawJsonValueAsync(jsonData);

            Debug.Log("Upload successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception in SaveFileCloud: " + ex.Message);
        }
    }
    /*
    public async void LoadFileCloud(string code)
    {
        try
        {
            InitializeFirebase();

            string puzzleCode = code;

            if (string.IsNullOrEmpty(puzzleCode))
            {
                Debug.LogError("Puzzle code input is empty!");
                return;
            }

            Debug.Log("Attempting to load puzzle with code: " + puzzleCode);

            var puzzleDataSnapshot = await dbRef.Child("puzzles").Child(puzzleCode).GetValueAsync();
            Debug.Log(puzzleDataSnapshot.ToString());

            if (puzzleDataSnapshot.Exists)
            {
                string json = puzzleDataSnapshot.GetRawJsonValue();
                JsonUtility.FromJsonOverwrite(json, currentBoardData);
                Debug.Log("Puzzle data loaded successfully!");
            }
            else
            {
                Debug.LogError("No puzzle data found for the code: " + puzzleCode);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load puzzle data from cloud: " + ex.Message);
        }
    }

    public async void LoadAllFileCloud(List<string> codes)
    {
        try
        {
            InitializeFirebase();

            Debug.Log("Attempting to load puzzles");

            var puzzleDataSnapshot = await dbRef.Child("puzzles").GetValueAsync();
            Debug.Log(puzzleDataSnapshot.ToString());

            if (puzzleDataSnapshot.Exists)
            {
                codes.Add(puzzleDataSnapshot.Children.ToString());
            }
            else
            {
                Debug.LogError("No puzzle data found for the code");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load puzzle data from cloud: " + ex.Message);
        }
    }
    */
}
