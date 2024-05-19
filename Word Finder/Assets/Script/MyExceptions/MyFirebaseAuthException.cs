using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase;
public class MyFirebaseAuthException : Exception
{
    public static string GetErrorMessage(Exception exception)
    {
        Debug.Log(exception.ToString());
        if (exception is FirebaseException firebaseEx)
        {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            return GetErrorMessage(errorCode);
        }

        return exception.ToString();
    }
    public static string GetErrorMessage(AuthError errorCode)
    {
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
        }
        return message;
    }
}
