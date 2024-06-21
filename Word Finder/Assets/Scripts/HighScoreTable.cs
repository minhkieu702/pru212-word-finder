using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighScoreEntry> highScoreEntryList;
    private List<Transform> highScoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainer.Find("HighScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        highScoreEntryList = new List<HighScoreEntry>()
        {
            new HighScoreEntry
            {
                score = 300,
                name = "AAA"
            },
            new HighScoreEntry
            {
                score = 4234,
                name = "ABA"
            },
            new HighScoreEntry
            {
                score = 6564,
                name = "BBB"
            },
            new HighScoreEntry
            {
                score = 7675,
                name = "BBC"
            },
            new HighScoreEntry
            {
                score = 3434,
                name = "GHG"
            },
        };

        highScoreEntryTransformList = new List<Transform>();
        foreach (HighScoreEntry highScoreEntry in highScoreEntryList.OrderByDescending(x => x.score))
        {
            CreateHighScoreEntryTransform(highScoreEntry, entryContainer, highScoreEntryTransformList);
        }
    }
    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 100f;

        Transform entryTransForm = Instantiate(entryTemplate, entryContainer);
        RectTransform entryRectTransform = entryTransForm.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransForm.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;

        switch (rank)
        {
            default: rankString = rank + "TH"; break;
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }
        entryTransForm.Find("posText").GetComponent<Text>().text = rankString;

        double score = highScoreEntry.score;
        entryTransForm.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highScoreEntry.name;
        entryTransForm.Find("nameText").GetComponent<Text>().text = name;
        transformList.Add(entryTransForm);
    }
    private class HighScoreEntry
    {
        public double score;
        public string name;
    }


}

  
