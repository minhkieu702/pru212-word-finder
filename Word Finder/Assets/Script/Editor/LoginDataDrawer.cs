using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LoginData), false)]
[CanEditMultipleObjects]
[Serializable]
public class LoginDataDrawer : Editor
{
    private LoginData LoginDataInstance => target as LoginData;

    public async override void OnInspectorGUI()
    {
        await DrawLoginInputFieldAsync();
    }

    private async Task DrawLoginInputFieldAsync()
    {
        var emailTemp = LoginDataInstance.emailLoginField;
        var passwordTemp = LoginDataInstance.passwordLoginField;

        LoginDataInstance.emailLoginField = EditorGUILayout.TextField("Email Login Field", LoginDataInstance.emailLoginField);
        LoginDataInstance.passwordLoginField = EditorGUILayout.TextField("Password Login Field", LoginDataInstance.passwordLoginField);
        LoginDataInstance.warningLoginText = EditorGUILayout.TextField("Message", LoginDataInstance.warningLoginText);
        if (GUILayout.Button("Login"))
        {
            string message = await LoginDataInstance.Login(emailTemp, passwordTemp);
            LoginDataInstance.warningLoginText = message;
        }

        if (GUILayout.Button("Logout"))
        {
            LoginDataInstance.Logout();
            LoginDataInstance.emailLoginField = "";
            LoginDataInstance.passwordLoginField = "";
            LoginDataInstance.warningLoginText = "";
        }
    }
}