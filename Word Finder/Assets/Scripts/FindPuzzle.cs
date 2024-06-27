using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindPuzzle : MonoBehaviour
{
    public TMP_InputField codeInput;
    public GameObject puzzleBox;
    public Transform puzzleList;
    public Button findPuzzleButton;

    private List<string> codes;
    private List<GameObject> puzzleBoxes;

    // Start is called before the first frame update
    void Start()
    {
        findPuzzleButton.onClick.AddListener(SearchPuzzle);
    }

    private void SearchPuzzle()
    {
        string code = codeInput.text.Trim();
        codes.Clear();
        if (string.IsNullOrEmpty(code))
        {
            Debug.Log("Hello");
            //PuzzleSaveManager.Instance.LoadAllFileCloud(codes);
        }
        else
        {
            code = code.ToUpper().Substring(0, 6);
            //PuzzleSaveManager.Instance.LoadFileCloud(code);
            codes.Add(code);
        }

        UpdateListUI();
    }

    private void UpdateListUI()
    {
        // Clear existing word items
        foreach (Transform child in puzzleList)
        {
            Destroy(child.gameObject);
        }

        puzzleBoxes = new List<GameObject>();
        foreach (var word in codes)
        {
            var puzzleBoxObj = Instantiate(puzzleBox, puzzleList);
            var boxItem = puzzleBoxObj.GetComponent<PuzzleBox>();
            boxItem.SetWord(word);
            boxItem.playButton.onClick.AddListener(() => PlayPuzzle(word));
            puzzleBoxes.Add(puzzleBoxObj);
        }
    }

    private void PlayPuzzle(string word)
    {

    }
}
