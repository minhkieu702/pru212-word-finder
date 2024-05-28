using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

[SerializeField]
[CreateAssetMenu]
public class LoadGameData : ScriptableObject
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;
    
    [Header("GameData")]
    public BoardObject BoardObject;

    async void Awake()
    {
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
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async Task<BoardObject> GetGameData(string gameid)
    {
        try
        {
            InitializeFirebase();
            var DbTask = DBreference.Child("game").GetValueAsync();//firstlayer
            await DbTask;

            if (DbTask.Result.Value == null)
            {
                Debug.LogError("No data about game");
                return null;
            }
            List<WordObject> wordObjects = new List<WordObject>();
            BoardObject boardObject = new BoardObject();
            DataSnapshot dataSnapshot = DbTask.Result;
            foreach (var snapshot in dataSnapshot.Children)
            {
                //first layer
                if (snapshot.Key == gameid)//secondlayer
                {
                    var board = snapshot.GetRawJsonValue();//inside secondlayer
                    boardObject = new BoardObject(board);
                }
            }
            return boardObject;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void CreateBoard(IEnumerable<DataSnapshot> board, ref BoardObject boardObject)
    {
        foreach (var item in board)
        {
            if (item.Key == "grid")//thirdlayer
            {
                
            }
        }
    }

    public void GetBoard()
    {

    }
}
