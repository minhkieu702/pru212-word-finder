using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordItem : MonoBehaviour
{
    public TMP_Text wordText;
    public Button deleteButton;

    public void SetWord(string word)
    {
        wordText.text = word.ToUpper();
    }
}