using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPopup : MonoBehaviour
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

    public void LoadNextLevel()
    {
        AdManager.Instance.ShowInterstitialAd();
        GameEvents.LoadNextLevelMethod();
    }

}
