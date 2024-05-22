using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(RegisterData), false)]
[CanEditMultipleObjects]
[Serializable]
public class RegisterDataDrawer : Editor
{
    private RegisterData RegisterDataInstance => target as RegisterData;

    public async override void OnInspectorGUI()
    {
        await DrawRegisterInputFieldAsync();
    }

    private async Task DrawRegisterInputFieldAsync()
    {
        var userTemp = RegisterDataInstance.usernameRegisterField;
        var emailTemp = RegisterDataInstance.emailRegisterField;
        var passwordTemp = RegisterDataInstance.passwordRegisterField;
        var verifyTemp = RegisterDataInstance.passwordRegisterVerifyField;

        RegisterDataInstance.usernameRegisterField = EditorGUILayout.TextField("Username Register Field", RegisterDataInstance.usernameRegisterField);
        RegisterDataInstance.emailRegisterField = EditorGUILayout.TextField("Email Register Field", RegisterDataInstance.emailRegisterField);
        RegisterDataInstance.passwordRegisterField = EditorGUILayout.TextField("Password Register Field", RegisterDataInstance.passwordRegisterField);
        RegisterDataInstance.passwordRegisterVerifyField = EditorGUILayout.TextField("Veirfy Register Field", RegisterDataInstance.passwordRegisterVerifyField);
        RegisterDataInstance.warningRegisterText = EditorGUILayout.TextField("Message", RegisterDataInstance.warningRegisterText);

        if (GUILayout.Button("Login"))
        {
            string message = await RegisterDataInstance.Register(userTemp, emailTemp, passwordTemp, verifyTemp);
            RegisterDataInstance.warningRegisterText = message;
        }
    }
}