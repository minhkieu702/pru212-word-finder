using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class SampleBoardDataDrawer : Editor
{
    static string json = "{ \"category\": \"cate1\", \"board\": { \"grid\": { \"row\": 10, \"column\": 10 }, \"word\": { \"word1\": { \"startRow\": 2, \"startColumn\": 3, \"endRow\": 2, \"endColumn\": 6, \"direction\": \"horizontal\" }, \"word2\": { \"startRow\": 5, \"startColumn\": 4, \"endRow\": 8, \"endColumn\": 4, \"direction\": \"vertical\" }, \"word3\": { \"startRow\": 7, \"startColumn\": 8, \"endRow\": 4, \"endColumn\": 5, \"direction\": \"diagonal\" } } } }";
    
    private SampleBoardData GameDataInstance => target as SampleBoardData;

    public override void OnInspectorGUI()
    {
        DrawBoardTable();
    }

    private void DrawBoardTable()
    {
        BoardObject board = new BoardObject(json);
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 35;

        var columnStyle = new GUIStyle();
        columnStyle.fixedWidth = 50;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.fixedWidth = 40;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var textFieldStyle = new GUIStyle();

        textFieldStyle.normal.background = Texture2D.grayTexture;
        textFieldStyle.normal.textColor = Color.white;
        textFieldStyle.fontStyle = FontStyle.Bold;
        textFieldStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginHorizontal(tableStyle);
        for (var x = 0; x < board.Column; x++)
        {
            EditorGUILayout.BeginVertical(x == -1 ? headerColumnStyle : columnStyle);
            for (var y = 0; y < GameDataInstance.Rows; y++)
            {
                if (x >= 0 && y >= 0)
                {
                    EditorGUILayout.BeginHorizontal(rowStyle);
                    var character = EditorGUILayout.TextArea(GameDataInstance.Board[x].Row[y], textFieldStyle);
                    if (GameDataInstance.Board[x].Row[y].Length > 1)
                    {
                        character = GameDataInstance.Board[x].Row[y].Substring(0, 1);
                    }
                    GameDataInstance.Board[x].Row[y] = character;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }
}
