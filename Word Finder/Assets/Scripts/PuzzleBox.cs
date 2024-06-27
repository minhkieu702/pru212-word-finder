using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBox : MonoBehaviour
{
    public TMP_Text wordText;
    public Button playButton;

    public void SetWord(string word)
    {
        wordText.text = word.ToUpper();
    }
}
