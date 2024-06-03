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

    [Header("Scoreboard")]
    public List<UserObject> _scoreboard;

    [Header("GameData")]
    public Dictionary<string, List<LevelObject>> _gameData = new();

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

            _gameData = await LoadGameData();

            _scoreboard = await LoadScoreBoard();

            _user = LoadUserData(_scoreboard);

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

    private UserObject LoadUserData(List<UserObject> scoreboard)
    {
        try
        {
            var user = scoreboard.FirstOrDefault(user => user.UserId == User.UserId);
            if (user == null)
            {
                InitialUserInfoToDatabase();
                return new UserObject
                {
                    CategoryScore = new Dictionary<string, UserScoreObject>
                    {
                        {
                            _gameData.Keys.First(),
                            new UserScoreObject { LevelScore = new Dictionary<string, double> { { "1", 0} } }
                        }
                    },
                    UserId = User.UserId,
                    Username = User.DisplayName
                };
            }
            return user;
        }
        catch (Exception)
        {

            throw;
        }
    }

    private async void InitialUserInfoToDatabase()
    {
        Task usernameTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(User.DisplayName);
        Task scoreTask = DBreference.Child("users").Child(User.UserId).Child(_gameData.Keys.First()).Child("1").Child("score").SetValueAsync(0);
        await Task.WhenAll(usernameTask, scoreTask);
    }

    public async Task<List<UserObject>> LoadScoreBoard()
    {
        try
        {
            InitializeFirebase();
            var dbTask = await DBreference.Child("users").GetValueAsync();
            if (dbTask.Value == null)
            {
                return null;
            }

            var userObjects = dbTask.Children.Select(userid => new UserObject
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
                                item => double.Parse(item.Child("score").Value.ToString())
                            ),
                            ScoreOfCategory = 0
                        }
                    )
            }).ToList();

            return userObjects;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load scoreboard: {ex.Message}");
        }
    }

    public void Logout()
    {
        auth.SignOut();
        Debug.LogFormat("User logged out in successfully");
    }

    public async Task<Dictionary<string, List<LevelObject>>> LoadGameData()
    {
        try
        {
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
