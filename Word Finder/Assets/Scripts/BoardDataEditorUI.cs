using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardDataEditorUI : MonoBehaviour
{
    public static BoardDataEditorUI Instance;

    public BoardData gameDataInstance;

    public TMP_InputField columnsInput;
    public TMP_InputField rowsInput;
    public TMP_InputField timeInput;
    public TMP_InputField newWordInput;
    public GameObject cellPrefab;
    public GameObject wordPrefab;
    public Transform wordsContainer;
    public Button clearBoardButton;
    public Button fillRandomButton;
    public Button saveButton;
    public Button loadButton;
    // public Button convertToUpperButton;
    public TMP_InputField fileNameInput;
    public Button addWordButton;
    public Button deleteButton;

    private List<List<GridCell>> boardCells;
    private List<GameObject> wordItems;
    public float cellOffset = 0.0f;
    public float topPosition;
    public string fileName;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fileName = "NewBoardData";
        fileNameInput.text = "NewBoardData";
        clearBoardButton.onClick.AddListener(ClearBoard);
        fillRandomButton.onClick.AddListener(FillBoardWithRandom);
        saveButton.onClick.AddListener(SaveBoardData);
        loadButton.onClick.AddListener(LoadBoardData);
        // convertToUpperButton.onClick.AddListener(ConvertBoardToUpper);
        deleteButton.onClick.AddListener(DeleteBoardData);
        addWordButton.onClick.AddListener(AddWord);

        columnsInput.onEndEdit.AddListener(delegate { UpdateBoardDimensions(); });
        rowsInput.onEndEdit.AddListener(delegate { UpdateBoardDimensions(); });
        timeInput.onEndEdit.AddListener(delegate { UpdateGameTime(); });
        fileNameInput.onEndEdit.AddListener(delegate { UpdateFileName(); });

        // columnsInput.text = gameDataInstance.Columns.ToString();
        // rowsInput.text = gameDataInstance.Rows.ToString();
        // timeInput.text = gameDataInstance.timeInSeconds.ToString();
        // LoadBoardData();
        UpdateBoardUI();
        UpdateWordsUI();
        UpdateBasicValue();
    }

    private void UpdateFileName()
    {
        fileName = fileNameInput.text;
    }

    private void UpdateGameTime()
    {
        if (float.TryParse(timeInput.text, out float time))
        {
            gameDataInstance.timeInSeconds = time;
        }
    }

    private void UpdateBoardDimensions()
    {
        if (int.TryParse(columnsInput.text, out int columns) && int.TryParse(rowsInput.text, out int rows))
        {
            if (columns != gameDataInstance.Columns || rows != gameDataInstance.Rows)
            {
                gameDataInstance.Columns = columns;
                gameDataInstance.Rows = rows;
                gameDataInstance.CreateNewBoard();
                UpdateBoardUI();
            }
        }
    }

    private void UpdateBoardUI()
    {
        // Clear existing cells
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        boardCells = new List<List<GridCell>>();
        var cellScale = GetCellScale(new Vector3(1.5f, 1.5f, 0.1f));

        for (int i = 0; i < gameDataInstance.Columns; i++)
        {
            var row = new List<GridCell>();
            for (int j = 0; j < gameDataInstance.Rows; j++)
            {
                var cellObj = Instantiate(cellPrefab, this.transform);
                var gridCell = cellObj.GetComponent<GridCell>();
                gridCell.Initialize(i, j, gameDataInstance.Board[i].Row[j]);
                gridCell.GetComponent<RectTransform>().localScale = cellScale;
                row.Add(gridCell);
            }
            boardCells.Add(row);
        }

        SetCellsPosition();
    }

    private Vector3 GetCellScale(Vector3 defaultScale)
    {
        var finalScale = defaultScale;
        var adjustment = 0.01f;
        while (ShouldScaleDown(finalScale))
        {
            finalScale.x -= adjustment;
            finalScale.y -= adjustment;

            if (finalScale.x <= 0 || finalScale.y <= 0)
            {
                finalScale.x = adjustment;
                finalScale.y = adjustment;
                return finalScale;
            }
        }
        return finalScale;
    }

    private bool ShouldScaleDown(Vector3 targetScale)
    {
        var squareRect = cellPrefab.GetComponent<RectTransform>().rect;
        var squareSize = new Vector2(0f, 0f);
        var startPosition = new Vector2(0f, 0f);

        squareSize.x = (squareRect.width * targetScale.x) + cellOffset;
        squareSize.y = (squareRect.height * targetScale.y) + cellOffset;

        var midWidthPosition = ((gameDataInstance.Columns * squareSize.x) / 2) * 0.01f;
        var midWidthHeight = ((gameDataInstance.Rows * squareSize.y) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y = midWidthHeight;
        //Debug.Log("Start Position: " + startPosition.x + ", " + startPosition.y);

        return startPosition.x < GetHalfScreenWidth() * -0.5f || startPosition.y > topPosition;
    }

    private float GetHalfScreenWidth()
    {
        float height = Camera.main.orthographicSize * 2;
        float width = (1.7f * height) * Screen.width / Screen.height;
        //Debug.Log("Screen Width: " + width);
        return width / 2;
    }

    private void SetCellsPosition()
    {
        if (boardCells.Count == 0 || boardCells[0].Count == 0) return;

        var cellRectTransform = boardCells[0][0].GetComponent<RectTransform>();
        var cellSize = new Vector2(cellRectTransform.rect.width, cellRectTransform.rect.height);
        //Debug.Log("Cell RectTransform / Size: " + cellRectTransform.rect.width + ", " + cellRectTransform.rect.height);
        //Debug.Log("Cell LocalScale: " + cellRectTransform.localScale.x + ", " + cellRectTransform.localScale.y);

        Vector2 offset = new Vector2
        {
            x = cellSize.x * cellRectTransform.localScale.x + cellOffset,
            y = cellSize.y * cellRectTransform.localScale.y + cellOffset
        };

        Vector2 startPosition = GetFirstCellPosition();
        //Debug.Log("Start position: " + startPosition.x + ", " + startPosition.y);

        for (int i = 0; i < gameDataInstance.Columns; i++)
        {
            for (int j = 0; j < gameDataInstance.Rows; j++)
            {
                var positionX = startPosition.x + offset.x * i;
                var positionY = startPosition.y - offset.y * j;

                Vector2 position = new Vector2(positionX, positionY);
                var cell = boardCells[i][j];
                cell.GetComponent<RectTransform>().anchoredPosition = position;
                //Debug.Log("Cell [" + i + ", " + j + "] position: " + startPosition.x + ", " + startPosition.y);
            }
        }
    }

    private Vector2 GetFirstCellPosition()
    {
        Vector2 startPosition = new Vector2(0f, transform.position.y);
        var cellRectTransform = boardCells[0][0].GetComponent<RectTransform>();
        var cellSize = new Vector2(cellRectTransform.rect.width, cellRectTransform.rect.height);

        cellSize.x *= cellRectTransform.localScale.x;
        cellSize.y *= cellRectTransform.localScale.y;

        var midWidthPosition = ((gameDataInstance.Columns - 1) * cellSize.x) / 2;
        var midWidthHeight = ((gameDataInstance.Rows - 1) * cellSize.y) / 2;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y += midWidthHeight;

        return startPosition;
    }

    public void UpdateCellValue(int column, int row, string value)
    {
        gameDataInstance.Board[column].Row[row] = value;
    }

    private void ClearBoard()
    {
        foreach (var row in gameDataInstance.Board)
        {
            for (int j = 0; j < row.Row.Length; j++)
            {
                row.Row[j] = " ";
            }
        }
        UpdateBoardUI();
    }

    private void FillBoardWithRandom()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < gameDataInstance.Columns; i++)
        {
            for (int j = 0; j < gameDataInstance.Rows; j++)
            {
                if (string.IsNullOrEmpty(gameDataInstance.Board[i].Row[j]) || gameDataInstance.Board[i].Row[j] == " ")
                {
                    int index = Random.Range(0, letters.Length);
                    gameDataInstance.Board[i].Row[j] = letters[index].ToString();
                }
            }
        }
        UpdateBoardUI();
    }

    //Deprecated - Fill random now do both
    /*
    private void ConvertBoardToUpper()
    {
        for (int i = 0; i < gameDataInstance.Columns; i++)
        {
            for (int j = 0; j < gameDataInstance.Rows; j++)
            {
                gameDataInstance.Board[i].Row[j] = gameDataInstance.Board[i].Row[j].ToUpper();
            }
        }
        UpdateBoardUI();
    }
    */

    private void AddWord()
    {
        string newWord = newWordInput.text.Trim();
        if (!string.IsNullOrEmpty(newWord))
        {
            gameDataInstance.SearchWords.Add(new BoardData.SearchingWord { Word = newWord });
            newWordInput.text = "";
            UpdateWordsUI();
        }
    }

    private void UpdateWordsUI()
    {
        // Clear existing word items
        foreach (Transform child in wordsContainer)
        {
            Destroy(child.gameObject);
        }

        wordItems = new List<GameObject>();
        foreach (var word in gameDataInstance.SearchWords)
        {
            var wordItemObj = Instantiate(wordPrefab, wordsContainer);
            var wordItem = wordItemObj.GetComponent<WordItem>();
            wordItem.SetWord(word.Word);
            wordItem.deleteButton.onClick.AddListener(() => DeleteWord(word));
            wordItems.Add(wordItemObj);
        }
    }

    private void DeleteWord(BoardData.SearchingWord word)
    {
        gameDataInstance.SearchWords.Remove(word);
        UpdateWordsUI();
    }

    private void UpdateBasicValue()
    {
        columnsInput.text = gameDataInstance.Columns.ToString();
        rowsInput.text = gameDataInstance.Rows.ToString();
        timeInput.text = gameDataInstance.timeInSeconds.ToString();
    }

    private void SaveBoardData()
    {
        PuzzleSaveManager.Instance.SaveFileLocal(fileName, gameDataInstance);
    }

    private void LoadBoardData()
    {
        PuzzleSaveManager.Instance.LoadFileLocal(fileName, gameDataInstance);
        UpdateBasicValue();
        UpdateBoardUI();
        UpdateWordsUI();
    }

    private void DeleteBoardData()
    {
        PuzzleSaveManager.Instance.DeleteFileLocal(fileName);
        gameDataInstance.CreateNewBoard();
        gameDataInstance.SearchWords.Clear();
        UpdateBasicValue();
        UpdateBoardUI();
        UpdateWordsUI();
    }
}
