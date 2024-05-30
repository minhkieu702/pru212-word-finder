using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[SerializeField]
[CreateAssetMenu]
public class ScoreboardData : ScriptableObject
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public DatabaseReference DBreference;

    [Header("UserData")]
    public List<UserObject> users;

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
        Debug.Log("Setting up Firebase Database");
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //List<UserID, UserObject>
    /*
     create an object that have username, dictionary<Level, Score>
     */
    public async Task<List<UserObject>> LoadUsersScoreBoard()
    {
        try
        {
            //Get data from users
            var DBTask = DBreference.Child("users").GetValueAsync();
            await DBTask;
            List<UserObject> userObjects = new List<UserObject>();
            if (DBTask.Result.Value == null)
            {
                return null;
            }
            else
            {
                DataSnapshot userids = DBTask.Result;
                foreach (DataSnapshot userid in userids.Children)
                {
                    UserObject user = new()
                    {
                        UserId = userid.Key,
                        Username = userid.Child("username").Value.ToString(),
                    };

                    foreach (DataSnapshot level in userid.Children)
                    {
                        if (level.Key != "username")
                        {
                            string levelName = level.Key;
                            int score = int.Parse(level.Child("score").Value.ToString());
                            user.ScoreLevel.Add(levelName, score);
                        }
                        Debug.Log("Level");
                    }
                    userObjects.Add(user);
                }
            }
            return userObjects;

        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
