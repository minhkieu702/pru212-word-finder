using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (DBreference == null)
        {
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }
    }

    //List<UserID, UserObject>
    /*
     create an object that have username, dictionary<Level, Score>
     */
    public async Task<List<UserObject>> LoadUsersScoreBoard()
    {
        try
        {
            InitializeFirebase();
            //Get data from users
            var DBTask = await DBreference.Child("users").GetValueAsync();
            List<UserObject> userObjects = new List<UserObject>();
            if (DBTask.Value == null)
            {
                return null;
            }
            
            userObjects = DBTask.Children.Select(userid =>
            {
                return new UserObject
                {
                    UserId = userid.Key,

                    Username = userid.Child("username").Value.ToString(),

                    CategoryScore = userid.Children
                    .Where(category => category.Key != "username")
                    .ToDictionary(
                        category => category.Key,
                        category => new UserScoreObject
                        {
                            LevelScore = category.Children.ToDictionary(
                                item => item.Key,
                                item => double.Parse(item.Child("score").Value.ToString())),
                            ScoreOfCategory = 0
                        })
                };
            }).ToList();
            return userObjects;
        }
        catch (System.Exception ex)
        {

            throw new System.Exception(ex.Message);
        }
    }
}
