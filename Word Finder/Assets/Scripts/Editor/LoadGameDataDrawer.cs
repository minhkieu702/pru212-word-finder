using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadGameData), false)]
[CanEditMultipleObjects]
[Serializable]
public class LoadGameDataDrawer : Editor
{
    private LoadGameData DataGameFromFirebaseInstance => target as LoadGameData;

    public async override void OnInspectorGUI()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        var board = await DataGameFromFirebaseInstance.GetGameData("game1");
        Debug.Log(board.Row);
    }
}
