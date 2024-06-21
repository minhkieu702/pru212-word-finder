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
    private LoadGameData DataGameInstance => target as LoadGameData;

    public async override void OnInspectorGUI()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        //await DataGameInstance.GetGameData("game1");
    }

}
