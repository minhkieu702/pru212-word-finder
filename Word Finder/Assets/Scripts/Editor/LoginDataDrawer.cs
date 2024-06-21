//using Google.MiniJSON;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(LoginData), false)]
//[CanEditMultipleObjects]
//[Serializable]
//public class LoginDataDrawer : Editor
//{
//    private LoginData LoginDataInstance => target as LoginData;
//    private char[,] matrix;

//    public override void OnInspectorGUI()
//    {
//        DrawLoginInputField();
//        if (matrix != null)
//        {
//            DrawMatrix(matrix);
//        }
//    }

//    #region DrawInspector
//    private void DrawLoginInputField()
//    {
//        EditorGUILayout.LabelField("Login Information", EditorStyles.boldLabel);
//        LoginDataInstance.emailLoginField = EditorGUILayout.TextField("Email Login Field", LoginDataInstance.emailLoginField);
//        LoginDataInstance.passwordLoginField = EditorGUILayout.TextField("Password Login Field", LoginDataInstance.passwordLoginField);
//        LoginDataInstance.warningLoginText = EditorGUILayout.TextField("Message", LoginDataInstance.warningLoginText);

//        if (GUILayout.Button("Login"))
//        {
//            _ = LoginUserAsync(); // Call the async method without awaiting it directly
//        }

//        if (GUILayout.Button("Logout"))
//        {
//            LogoutUser();
//        }

//        EditorUtility.SetDirty(LoginDataInstance);
//    }

//    private async Task LoginUserAsync()
//    {
//        string email = LoginDataInstance.emailLoginField;
//        string password = LoginDataInstance.passwordLoginField;

//        var user = await LoginDataInstance.Login(email, password);
//        if (user != null)
//        {
//            LoginDataInstance.warningLoginText = user.ToString();
//            var gameData = LoginDataInstance._gameData;
//            matrix = LoadLevelData(gameData, "cate1", "1");
//            EditorUtility.SetDirty(LoginDataInstance);
//            // Force the inspector to repaint to show the matrix
//            EditorApplication.QueuePlayerLoopUpdate();
//            Repaint();
//        }
//    }

//    private void LogoutUser()
//    {
//        LoginDataInstance.Logout();
//        LoginDataInstance.emailLoginField = "";
//        LoginDataInstance.passwordLoginField = "";
//        LoginDataInstance.warningLoginText = "";
//        matrix = null;
//        EditorUtility.SetDirty(LoginDataInstance);
//    }

//    private void DrawMatrix(char[,] matrix)
//    {
//        EditorGUILayout.LabelField("Generated Matrix", EditorStyles.boldLabel);

//        for (int row = 0; row < matrix.GetLength(0); row++)
//        {
//            EditorGUILayout.BeginHorizontal();
//            for (int column = 0; column < matrix.GetLength(1); column++)
//            {
//                EditorGUILayout.LabelField(matrix[row, column].ToString(), GUILayout.Width(20));
//            }
//            EditorGUILayout.EndHorizontal();
//        }
//    }
//    #endregion

//    #region LoadDataToMatrix
//    public char[,] LoadLevelData(Dictionary<string, List<LevelObject>> gameData, string category, string level)
//    {
//        var lettersWithAddress = new Dictionary<string, char>();
//        var board = gameData[category].FirstOrDefault(levelObject => levelObject.Level == level);

//        if (board == null)
//            throw new ArgumentException("Level not found");

//        foreach (var item in board.Words)
//        {
//            AddWordToDictionary(item, lettersWithAddress);
//        }

//        return FillMatrix(board, lettersWithAddress);
//    }

//    private void AddWordToDictionary(WordObject wordObject, Dictionary<string, char> lettersWithAddress)
//    {
//        string word = wordObject.Word.ToUpper();
//        int startColumn = wordObject.startColumn;
//        int endColumn = wordObject.endColumn;
//        int startRow = wordObject.startRow;
//        int endRow = wordObject.endRow;
//        char[] letters = word.ToCharArray();

//        AddToDictionary(startRow, startColumn, letters[0], lettersWithAddress);

//        for (int i = 1; i < word.Length; i++)
//        {
//            (startRow, startColumn) = GetNextPosition(startRow, startColumn, endRow, endColumn);
//            AddToDictionary(startRow, startColumn, letters[i], lettersWithAddress);
//        }

//        AddToDictionary(endRow, endColumn, letters[^1], lettersWithAddress);
//    }

//    private (int, int) GetNextPosition(int startRow, int startColumn, int endRow, int endColumn)
//    {
//        if (startColumn == endColumn)
//        {
//            return startRow < endRow ? (startRow + 1, startColumn) : (startRow - 1, startColumn);
//        }
//        if (startRow == endRow)
//        {
//            return startColumn < endColumn ? (startRow, startColumn + 1) : (startRow, startColumn - 1);
//        }
//        if (startColumn < endColumn)
//        {
//            return startRow < endRow ? (startRow + 1, startColumn + 1) : (startRow - 1, startColumn + 1);
//        }
//        return startRow < endRow ? (startRow + 1, startColumn - 1) : (startRow - 1, startColumn - 1);
//    }

//    private void AddToDictionary(int row, int column, char letter, Dictionary<string, char> lettersWithAddress)
//    {
//        string key = $"{row},{column}";
//        lettersWithAddress[key] = letter;
//    }

//    private char[,] FillMatrix(LevelObject board, Dictionary<string, char> lettersWithAddress)
//    {
//        var matrix = new char[board.Row, board.Column];
//        var random = new System.Random();
//        const string letters = "AQWSEDRFTGYHUJIKOLPMNBVCXZ";

//        for (int row = 0; row < board.Row; row++)
//        {
//            for (int column = 0; column < board.Column; column++)
//            {
//                string key = $"{row},{column}";
//                matrix[row, column] = lettersWithAddress.ContainsKey(key)
//                    ? lettersWithAddress[key]
//                    : letters[random.Next(letters.Length)];
//            }
//        }

//        return matrix;
//    }
//    #endregion
//}

