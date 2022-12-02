using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdController : MonoBehaviour
{
    //private string appKey = "ca-app-pub-2854576671634983~8337776201";
    //private string IAKey = "ca-app-pub-2854576671634983/2893877839";           //Benim ID lerim
    //private string RAKey = "ca-app-pub-2854576671634983/2465722172";

    public static AdController instance;

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    bool isBannerShowing;
    float interstitialTimer;
    private float maxIntersititialTimer = 60;

    public void initAdController()
    {
        if (instance==null)
        {

            instance = this;
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus => { });

            interstitialTimer = maxIntersititialTimer;

            this.RequestBanner();
            this.RequestInterstitial();
            this.RequestRewarded();
        }
    }
    void Update()
    {
        interstitialTimer -= Time.deltaTime;
    }

    #region BANNER AD
    private void RequestBanner()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        //kendi id mi ekleyecegim yer
#if UNITY_ANDROID  
            string adUnitId = "ca-app-pub-3940256099942544/6300978111"; 
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the bottom of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);

        this.isBannerShowing = true;

        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnBannerAdFailedToLoad;



    }

    private void HandleOnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestBanner();
    }

    public void showBannerAd() {
        if (!isBannerShowing)
        {
            this.bannerView.Show();
            this.isBannerShowing = true;

        }

    }
    public void hideBannerAd() {
        if (isBannerShowing)
        {
            this.bannerView.Hide();
            this.isBannerShowing = false;

        }
    }



    #endregion

    #region INTERSTITIAL AD
    private void RequestInterstitial()
    {
        //kendi id mi ekleyecegim yer

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);


        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnIntersititialAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnInterstitialAdOpening;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnInterstitialAdClosed;

    }
    private void HandleOnInterstitialAdClosed(object sender, EventArgs e)
    {
        interstitialTimer = maxIntersititialTimer;
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        RequestInterstitial();
    }
    private void HandleOnInterstitialAdOpening(object sender, EventArgs e)
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<AudioListener>().enabled = false;
    }
    private void HandleOnIntersititialAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestInterstitial();
    }
    public void showIntersititial()
    {
        if (this.interstitialTimer < 0 && this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }
    #endregion

    #region REWARDED AD
    private void RequestRewarded()
    {
        //kendi id mi ekleyecegim yer


#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);


        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        RequestRewarded();
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        LevelController.Current.giveMoneyToPlayer(LevelController.Current.score);
    }

    private void HandleRewardedAdOpening(object sender, EventArgs e)
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<AudioListener>().enabled = false;
    }

    private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestRewarded();
    }
    public void showRewardedAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }
    public bool isRewardedLoaded => rewardedAd.IsLoaded();
    #endregion
}
