using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System;

[SerializeField]
[CreateAssetMenu]
public class LoginData : ScriptableObject
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    //Login variables
    [Header("Login")]
    public string emailLoginField;
    public string passwordLoginField;
    public string warningLoginText;

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

    public async Task<string> Login(string _email, string _password)
    {
        if (_email.Trim() == "" || _password.Trim() == "")
        {
            return "Please input all fields.";
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
            return "Logged In";
        }
        catch (Exception)
        {
            // Handle FirebaseAuthException
            Debug.LogFormat($"User signed in fail");
            //FirebaseException exception = ex as FirebaseException;
            //var errorCode = (AuthError)exception.ErrorCode;
            return "Login Failed! Please Check Again.";
        }
    }

    public void Logout()
    {
        auth.SignOut();
        Debug.LogFormat("User logged out in successfully");
    }
}
