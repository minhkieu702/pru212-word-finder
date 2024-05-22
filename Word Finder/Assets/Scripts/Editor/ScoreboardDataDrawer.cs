using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScoreboardData), false)]
[CanEditMultipleObjects]
[Serializable]
public class ScoreboardDataDrawer : Editor
{
    private ScoreboardData ScoreboardDataInstance => target as ScoreboardData;
    public async override void OnInspectorGUI()
    {
        await DrawScoreBoardFieldAsync();
    }

    private async Task DrawScoreBoardFieldAsync()
    {
        var scoreboard = await ScoreboardDataInstance.LoadUsersScoreBoard();
        foreach (var user in scoreboard)
        {
            EditorGUILayout.TextField("ha", "ha");
        }
    }
}
