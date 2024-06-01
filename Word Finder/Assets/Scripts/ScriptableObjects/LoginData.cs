using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Firebase.Database;
using System.Linq;

[SerializeField]
[CreateAssetMenu]
public class LoginData : ScriptableObject
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public string emailLoginField;
    public string passwordLoginField;
    public string warningLoginText;

    [Header("UserData")]
    public UserObject _user;
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
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth ??= FirebaseAuth.DefaultInstance;
        DBreference ??= FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async Task<UserObject> Login(string _email, string _password)
    {
        InitializeFirebase();
        if (_email.Trim() == "" || _password.Trim() == "")
        {
            warningLoginText = "Please input all fields.";
            return null;
        }
        try
        {
            // Call the Firebase auth signin function passing the email and password
            Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

            // Wait until the task completes
            AuthResult result = await LoginTask;

            // User is now logged in
            // Now get the result
            User = result.User;
            Debug.LogFormat($"User signed in successfully: {User.DisplayName} ({User.Email})");
            warningLoginText = "Logged In";

            _user = new()
            {
                UserId = User.UserId,
                Username = User.DisplayName,
                CategoryScore = await LoadUserData()
            };
            return _user;
        }
        catch (Exception)
        {
            // Handle FirebaseAuthException
            Debug.LogFormat($"User signed in fail");
            //FirebaseException exception = ex as FirebaseException;
            //var errorCode = (AuthError)exception.ErrorCode;
            warningLoginText = "Login Failed! Please Check Again.";
            return null;
        }
    }

    private async Task InitialUserInfoToDatabase()
    {
        Task usernameTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(User.DisplayName);
        Task scoreTask = DBreference.Child("users").Child(User.UserId).Child("cate1").Child("1").Child("score").SetValueAsync(0);
        await Task.WhenAll(usernameTask, scoreTask);
    }

    private async Task<Dictionary<string, UserScoreObject>> LoadUserData()
    {
        try
        {
            var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
            await DBTask;
            if (DBTask.Result.Value == null)
            {
                await InitialUserInfoToDatabase();
                return new Dictionary<string, UserScoreObject>
                {
                    {
                        "cate1", new UserScoreObject()
                        {
                            LevelScore = new Dictionary<string, double>
                            {
                                {
                                    "1", 0
                                }
                            },
                            ScoreOfCategory = 0
                        }
                    }
                };
            }
            return DBTask.Result.Children
                .Where(cate => cate.Key != "username")
                .ToDictionary(cate => cate.Key,
                    cate => new UserScoreObject
                    {
                        ScoreOfCategory = 0,
                        LevelScore = cate.Children.ToDictionary(
                        level => level.Key,
                        level => double.Parse(level.Child("score").Value.ToString())
                )
                    });
        }
        catch (Exception)
        {
            warningLoginText = "Failed";
            return null;
        }
    }

    public void Logout()
    {
        auth.SignOut();
        Debug.LogFormat("User logged out in successfully");
    }
}
