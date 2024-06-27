using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPopupTest : MonoBehaviour
{
    public GameObject winPopup;
    void Start()
    {
        winPopup.SetActive(false);
    }

    private void OnDisable()
    {
        GameEvents.OnBoardCompleted -= ShowInPopup;
        AdManager.OnInterstitialAdsClosed -= IntersititalAdCompleted;

    }

    private void OnEnable()
    {
        GameEvents.OnBoardCompleted += ShowInPopup;
        AdManager.OnInterstitialAdsClosed += IntersititalAdCompleted;
    }

    private void IntersititalAdCompleted()
    {

    }

    private void ShowInPopup()
    {
        AdManager.Instance.HideBanner();
        winPopup.SetActive(true);
    }

    public void UploadLevel()
    {
        PuzzleSaveManager.Instance.SaveFileCloud();
        SceneManager.LoadScene("FindPuzzle");
    }

}
