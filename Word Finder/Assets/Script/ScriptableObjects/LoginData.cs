using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Firebase.Database;

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
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
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

            //Dictionary<string,int> keyValuesObject = await LoadUserData();

            _user = new()
            {
                UserId = User.UserId,
                Username = User.DisplayName,
                ScoreLevel = await LoadUserData()
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
        Task scoreTask = DBreference.Child("users").Child(User.UserId).Child("level1").Child("score").SetValueAsync(0);

        await Task.WhenAll(usernameTask, scoreTask);
    }
    private async Task<Dictionary<string, int>> LoadUserData()
    {
        try
        {
            var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
            await DBTask;

            Dictionary<string, int> levelscore = new Dictionary<string, int>();
            if (DBTask.Result.Value == null)
            {
                levelscore.Add("level1", 0);
                await InitialUserInfoToDatabase();
                warningLoginText = "init data success";
            }
            else
            {
                DataSnapshot snapshot = DBTask.Result;
                foreach (DataSnapshot levelSnapshot in snapshot.Children)
                {
                    if (levelSnapshot.Key != "username")
                    {
                        string levelName = levelSnapshot.Key;
                        int score = int.Parse(levelSnapshot.Child("score").Value.ToString());
                        levelscore.Add(levelName, score);
                    }
                }
                warningLoginText = "get data success";
            }
            return levelscore;
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
