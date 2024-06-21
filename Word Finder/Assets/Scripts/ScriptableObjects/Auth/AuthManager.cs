using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Firebase.Database;
using System.Linq;
using System;

public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;

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
    public void RegisterButton()
    {
        Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text);
    }
    public void LoginButton()
    {
        Login(emailLoginField.text, passwordLoginField.text);
    }
    private async void Register(string _email, string _password, string _username)
    {
        if (string.IsNullOrEmpty(_username))
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
            return;
        }

        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
            return;
        }

        try
        {
            // Call the Firebase auth create user function passing the email and password
            AuthResult registerResult = await auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            User = registerResult.User;

            if (User != null)
            {
                // Create a user profile and set the username
                UserProfile profile = new UserProfile { DisplayName = _username };

                // Call the Firebase auth update user profile function passing the profile with the username
                await User.UpdateUserProfileAsync(profile);
                warningRegisterText.text = User.DisplayName + " was created. Let's login and play!!!";
                // Username is now set, now return to login screen
                if (UIManager.instance != null)
                {
                    UIManager.instance.LoginScreen();
                    warningRegisterText.text = "";
                }
            }
        }
        catch (FirebaseException firebaseEx)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
                default:
                    message = "Unknown error occurred";
                    break;
            }
            Debug.LogWarning($"Failed to register with {firebaseEx}");
            warningRegisterText.text = message;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error: {ex}");
            warningRegisterText.text = "An unexpected error occurred. Please try again.";
        }
    }

    private async void Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        try
        {
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(_email, _password);
            User = authResult.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "Logged In";

            _gameData = await LoadGameData();
            _scoreboard = await LoadScoreBoard();
            _user = LoadUserData(_scoreboard);
        }
        catch (FirebaseException firebaseEx)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
                default:
                    message = "Unknown error occurred";
                    break;
            }
            Debug.LogWarning($"Failed to login with {firebaseEx}");
            warningLoginText.text = message;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error: {ex}");
            warningLoginText.text = "An unexpected error occurred. Please try again.";
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
                    ),
                Score = 0
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
