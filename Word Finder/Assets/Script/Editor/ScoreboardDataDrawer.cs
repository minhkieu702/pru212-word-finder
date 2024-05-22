using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[CustomEditor(typeof(ScoreboardData), false)]
[CanEditMultipleObjects]
[Serializable]
public class ScoreboardDataDrawer : Editor
{
    //private ScoreboardData ScoreboardDataInstance => target as ScoreboardData;

    //public async override void OnInspectorGUI()
    //{
    //    await DrawScoreboardFieldAsync();
    //}

    //private async Task DrawScoreboardFieldAsync()
    //{
    //    List<UserObject> users = await ScoreboardDataInstance.LoadUserScore();
    //    if (users == null)
    //    {
    //        return; // Don't draw anything if data is not loaded yet
    //    }

    //    foreach (var user in users)
    //    {
    //        EditorGUILayout.TextField("Username", user.Username);
    //        foreach (var levelScore in user.ScoreLevel)
    //        {
    //            EditorGUILayout.TextField("Level", levelScore.Key);
    //            EditorGUILayout.TextField("Score", levelScore.Value.ToString());
    //        }
    //    }
    //}
    private ScoreboardData ScoreboardDataInstance => target as ScoreboardData;
    private List<UserObject> users;

    private void OnEnable()
    {
        LoadData();
    }

    public override void OnInspectorGUI()
    {
        if (users == null)
        {
            return; // Don't draw anything if data is not loaded yet
        }

        foreach (var user in users)
        {
            EditorGUILayout.TextField("Username", user.Username);
            foreach (var levelScore in user.ScoreLevel)
            {
                EditorGUILayout.TextField("Level", levelScore.Key);
                EditorGUILayout.TextField("Score", levelScore.Value.ToString());
            }
        }
    }

    private async void LoadData()
    {
        users = await ScoreboardDataInstance.LoadUserScore();
    }

}
