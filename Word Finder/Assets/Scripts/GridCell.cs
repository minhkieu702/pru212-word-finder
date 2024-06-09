using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public TMP_InputField inputField;
    private int column;
    private int row;

    public void Initialize(int column, int row, string text)
    {
        this.column = column;
        this.row = row;
        inputField.text = text;

        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnEndEdit(string value)
    {
        // Clamp input to a single character
        if (value.Length > 1)
        {
            value = value.Substring(0, 1);
            inputField.text = value;
        }

        // Notify the BoardDataEditorUI to update the corresponding cell in BoardData
        BoardDataEditorUI.Instance.UpdateCellValue(column, row, value.ToUpper());
    }
}
