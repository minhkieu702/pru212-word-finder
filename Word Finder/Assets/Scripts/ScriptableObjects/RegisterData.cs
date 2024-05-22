using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

[SerializeField]
[CreateAssetMenu]
public class RegisterData : ScriptableObject
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    //Register variables
    [Header("Register")]
    public string usernameRegisterField;
    public string emailRegisterField;
    public string passwordRegisterField;
    public string passwordRegisterVerifyField;
    public string warningRegisterText;
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
    }

    public async Task<string> Register(string _username, string _email, string _password, string _passwordVerify)
    {
        InitializeFirebase();
        if (_username.Trim() == "" || _email.Trim() == "" || _password.Trim() == "" || _passwordVerify.Trim() == "")
        {
            return "Please input all fields.";
        }
        else if (_password != _passwordVerify)
        {
            return "Password Does Not Match!";
        }
        try
        {
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            AuthResult result = await RegisterTask;
            User = result.User;

            if (User != null)
            {
                //Create a user profile and set the username
                UserProfile profile = new UserProfile { DisplayName = _username };

                //Call the Firebase auth update user profile function passing the profile with the username
                Task ProfileTask = User.UpdateUserProfileAsync(profile);
                return "Registered Successfully";
            }
            throw new Exception();
        }
        catch (Exception ex)
        {
            return ex + "Register Failed! Please use another email.";
        }
    }
}
