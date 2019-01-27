#if SERVICE_ADMOB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

namespace Services
{
    public class AdmobService : IService
    {
        BannerView banner;
        InterstitialAd interstitial;
        RewardBasedVideoAd rewardedVideo;

        public override IService Init(ServiceDef def, ServiceEvents gameE)
        {
            if (!def.UseAdmob)
            {
                return null;
            }
            base.Init(def, gameE);
            //init
#if UNITY_IOS
            MobileAds.Initialize(def.AdmobAppID_IOS);
            MobileAds.SetiOSAppPauseOnBackground(true);
#elif UNITY_ANDROID
            MobileAds.Initialize(def.AdmobAppID_Android);
#endif

#if UNITY_ADMOB_MEDIATION_APPLOVIN
            //Applovin
            GoogleMobileAds.Api.Mediation.AppLovin.AppLovin.Initialize();
#endif
            rewardedVideo = RewardBasedVideoAd.Instance;
            // Called when an ad request has successfully loaded.
            rewardedVideo.OnAdLoaded += OnRewardVideoLoaded;
            // Called when an ad request failed to load.
            rewardedVideo.OnAdFailedToLoad += OnRewardVideoFailedToLoad;
            // Called when an ad is shown.
            rewardedVideo.OnAdOpening += OnRewardVideoOpened;
            // Called when the ad starts to play.
            rewardedVideo.OnAdStarted += OnRewardVideoStarted;
            // Called when the user should be rewarded for watching a video.
            rewardedVideo.OnAdRewarded += OnRewardVideoRewarded;
            // Called when the ad is closed.
            rewardedVideo.OnAdClosed += OnRewardVideoClosed;
            // Called when the ad click caused the user to leave the application.
            rewardedVideo.OnAdLeavingApplication += OnRewardVideoLeftApplication;

            serviceE.RequestBanner += RequestBanner;
            serviceE.StopShowBanner += StopShowBanner;
            serviceE.RequestInterstitial += RequestInterstitial;
            serviceE.ShowInterstitial += ShowInterstitial;
            serviceE.RequestRewardedVideo += RequestRewardedVideo;
            serviceE.ShowRewardedVideo += ShowRewardedVideo;
            serviceE.IsInterstitialAvailable += IsInterstitalAvailable;
            serviceE.IsRewardVideoAvailable += IsRewardedVideoAvailable;

            serviceE.EnableSound += OnSoundEnable;
            return this;
        }

        public override void Dispose()
        {
            serviceE.RequestBanner -= RequestBanner;
            serviceE.StopShowBanner -= StopShowBanner;
            serviceE.RequestInterstitial -= RequestInterstitial;
            serviceE.ShowInterstitial -= ShowInterstitial;
            serviceE.RequestRewardedVideo -= RequestRewardedVideo;
            serviceE.ShowRewardedVideo -= ShowRewardedVideo;
            serviceE.IsInterstitialAvailable -= IsInterstitalAvailable;
            serviceE.IsRewardVideoAvailable -= IsRewardedVideoAvailable;

            serviceE.EnableSound -= OnSoundEnable;

            if (banner != null)
            {
                banner.OnAdLoaded -= OnBannerLoaded;
                banner.OnAdFailedToLoad -= OnBannerFailedToLoad;
                banner.OnAdOpening -= OnBannerOpened;
                banner.OnAdClosed -= OnBannerClosed;
                banner.OnAdLeavingApplication -= OnBannerLeftApplication;
                banner.Destroy();
            }

            if (interstitial != null)
            {
                interstitial.OnAdLoaded -= OnIntertitialLoaded;
                interstitial.OnAdFailedToLoad -= OnInterstitialFailedToLoad;
                interstitial.OnAdOpening -= OnInterstitialOpened;
                interstitial.OnAdClosed -= OnInterstitialClosed;
                interstitial.OnAdLeavingApplication -= OnInterstitialLeftApplication;
                interstitial.Destroy();
            }

            rewardedVideo.OnAdLoaded -= OnRewardVideoLoaded;
            rewardedVideo.OnAdFailedToLoad -= OnRewardVideoFailedToLoad;
            rewardedVideo.OnAdOpening -= OnRewardVideoOpened;
            rewardedVideo.OnAdStarted -= OnRewardVideoStarted;
            rewardedVideo.OnAdRewarded -= OnRewardVideoRewarded;
            rewardedVideo.OnAdClosed -= OnRewardVideoClosed;
            rewardedVideo.OnAdLeavingApplication -= OnRewardVideoLeftApplication;
        }

        void OnSoundEnable(bool enable)
        {
            Debug.Log("OnSoundEnable, enable = " + enable);
            MobileAds.SetApplicationMuted(!enable);
            //MobileAds.SetApplicationVolume(enable? 1f : 0f);
        }

        #region RewardedVideo
        void RequestRewardedVideo()
        {
#if UNITY_IOS
            string adUnitId = def.AdmobRewardedVideoID_IOS;
#elif UNITY_ANDROID
            string adUnitId = def.AdmobRewardedVideoID_Android;
#else
            string adUnitId = "Editor ID";
#endif

#if UNITY_ADMOB_MEDIATION_VUNGLE
            GoogleMobileAds.Api.Mediation.Vungle.VungleRewardedVideoMediationExtras extras = new GoogleMobileAds.Api.Mediation.Vungle.VungleRewardedVideoMediationExtras();
#if UNITY_ANDROID
    extras.SetAllPlacements(new string[] { "REWARDED-7542357" });
#elif UNITY_IOS
    extras.SetAllPlacements(new string[] { "REWARDED-7542357" });
#endif
#endif
            AdRequest request = new AdRequest
                .Builder()
#if UNITY_ADMOB_MEDIATION_VUNGLE
                .AddMediationExtras(extras)
#endif
                .Build();

            rewardedVideo.LoadAd(request, adUnitId);
        }

        bool IsRewardedVideoAvailable()
        {
            return rewardedVideo.IsLoaded();
        }

        bool ShowRewardedVideo()
        {
            if (!rewardedVideo.IsLoaded())
            {
                //TextLog.Instance.Log("RewardedVideo show didn't load");
                return false;
            }

            rewardedVideo.Show();
            return true;
        }

        public void OnRewardVideoLoaded(object sender, EventArgs args)
        {
            //TextLog.Instance.Log("HandleRewardBasedVideoLoaded event received");
            if (serviceE.OnRewardVideoLoaded != null)
            {
                serviceE.OnRewardVideoLoaded(rewardedVideo.MediationAdapterClassName());
            }
        }

        public void OnRewardVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
#if SV_DEBUG_ADMOB
            Debug.Log(
                "HandleRewardBasedVideoFailedToLoad event received with message: "
                                 + args.Message);
#endif
            if (serviceE.OnRewardVideoFailedToLoad != null)
            {
                serviceE.OnRewardVideoFailedToLoad();
            }
        }

        public void OnRewardVideoOpened(object sender, EventArgs args)
        {
#if SV_DEBUG_ADMOB
            Debug.Log("HandleRewardBasedVideoOpened event received");
#endif
            if (serviceE.OnRewardVideoOpened != null)
            {
                serviceE.OnRewardVideoOpened(rewardedVideo.MediationAdapterClassName());
            }
        }

        public void OnRewardVideoStarted(object sender, EventArgs args)
        {
#if SV_DEBUG_ADMOB
            Debug.Log("HandleRewardBasedVideoStarted event received");
#endif
            if (serviceE.OnRewardVideoStarted != null)
            {
                serviceE.OnRewardVideoStarted(rewardedVideo.MediationAdapterClassName());
            }
        }

        public void OnRewardVideoClosed(object sender, EventArgs args)
        {
#if SV_DEBUG_ADMOB
            Debug.Log("HandleRewardBasedVideoClosed event received");
#endif
            if (serviceE.OnRewardVideoClosed != null)
            {
                serviceE.OnRewardVideoClosed(rewardedVideo.MediationAdapterClassName());
            }
        }

        public void OnRewardVideoRewarded(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
#if SV_DEBUG_ADMOB
            Debug.Log(
                "HandleRewardBasedVideoRewarded event received for "
                            + amount.ToString() + " " + type);
#endif
            if (serviceE.OnRewardVideoRewarded != null)
            {
                serviceE.OnRewardVideoRewarded(rewardedVideo.MediationAdapterClassName(), type, amount);
            }
        }

        public void OnRewardVideoLeftApplication(object sender, EventArgs args)
        {
#if SV_DEBUG_ADMOB
            Debug.Log("HandleRewardBasedVideoLeftApplication event received");
#endif
            if (serviceE.OnRewardVideoLeftApplication != null)
            {
                serviceE.OnRewardVideoLeftApplication(rewardedVideo.MediationAdapterClassName());
            }
        }
        #endregion

        #region Interstitial
        bool IsInterstitalAvailable()
        {
            if (interstitial == null)
            {
                return false;
            }

            return interstitial.IsLoaded();
        }
        void RequestInterstitial()
        {
#if UNITY_IOS
            string adUnitId = def.AdmobInterstitialID_IOS;
#elif UNITY_ANDROID
            string adUnitId = def.AdmobInterstitialID_Android;
#else
            string adUnitId = "Editor ID";
#endif
            if (interstitial != null)
            {
                interstitial.Destroy();
            }
            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(adUnitId);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the interstitial with the request.
            interstitial.LoadAd(request);

            // Called when an ad request has successfully loaded.
            interstitial.OnAdLoaded += OnIntertitialLoaded;
            // Called when an ad request failed to load.
            interstitial.OnAdFailedToLoad += OnInterstitialFailedToLoad;
            // Called when an ad is shown.
            interstitial.OnAdOpening += OnInterstitialOpened;
            // Called when the ad is closed.
            interstitial.OnAdClosed += OnInterstitialClosed;
            // Called when the ad click caused the user to leave the application.
            interstitial.OnAdLeavingApplication += OnInterstitialLeftApplication;
        }

        bool ShowInterstitial()
        {
            if (interstitial == null)
            {
                //TextLog.Instance.Log("Interstitial show null");
                return false;
            }

            if (!interstitial.IsLoaded())
            {
                //TextLog.Instance.Log("Interstitial show didn't load");
                return false;
            }

            interstitial.Show();
            return true;
        }

        public void OnIntertitialLoaded(object sender, EventArgs args)
        {
            //TextLog.Instance.Log("Interstitial Loaded");
            if (serviceE.OnInterstitialLoaded != null)
            {
                serviceE.OnInterstitialLoaded(interstitial.MediationAdapterClassName());
            }
        }

        public void OnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            //TextLog.Instance.Log("Interstitial Fail:" + args.Message);
            if (serviceE.OnInterstitialFailedToLoad != null)
            {
                serviceE.OnInterstitialFailedToLoad();
            }
        }

        public void OnInterstitialOpened(object sender, EventArgs args)
        {
            //TextLog.Instance.Log("Interstitial Opened");
            if (serviceE.OnInterstitialOpened != null)
            {
                serviceE.OnInterstitialOpened(interstitial.MediationAdapterClassName());
            }
        }

        public void OnInterstitialClosed(object sender, EventArgs args)
        {
            //TextLog.Instance.Log("Interstitial Closed");
            if (serviceE.OnInterstitialClosed != null)
            {
                serviceE.OnInterstitialClosed(interstitial.MediationAdapterClassName());
            }
        }

        public void OnInterstitialLeftApplication(object sender, EventArgs args)
        {
            //TextLog.Instance.Log("Interstitial Left App");
            if (serviceE.OnInterstitialLeftApplication != null)
            {
                serviceE.OnInterstitialLeftApplication(interstitial.MediationAdapterClassName());
            }
        }
        #endregion

        #region Banner
        void StopShowBanner()
        {
            if (banner != null)
            {
                banner.Destroy();
                banner = null;
            }
        }

        void RequestBanner()
        {
#if UNITY_IOS
            string adUnitId = def.AdmobBannerID_IOS;
#elif UNITY_ANDROID
            string adUnitId = def.AdmobBannerID_Android;
#endif
            if (banner != null)
            {
                banner.Destroy();
            }

            //if (Screen.safeArea.height != Screen.height)
            //{
            //    banner = new BannerView(adUnitId, AdSize.SmartBanner, 0, Mathf.RoundToInt(Screen.safeArea.yMax - 52));
            //}
            //else
#if UNITY_IOS            
            banner = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
#elif UNITY_ANDROID
            float ScreenWidthInch = Screen.width / Screen.dpi;
            float ScreenHeightInch = Screen.height / Screen.dpi;

            double diagonalInches = Math.Sqrt(ScreenWidthInch * ScreenWidthInch + ScreenHeightInch * ScreenHeightInch);

            if (diagonalInches > 7.0)// 6.5)
            {
                // 6.5inch device or bigger
                banner = new BannerView(adUnitId, AdSize.Leaderboard, AdPosition.Bottom);
            }
            else
            {
                banner = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
                //return AdSize.Banner;
                // smaller device
            }
#endif


            // Create an empty ad request.

            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            banner.LoadAd(request);

            // Called when an ad request has successfully loaded.
            banner.OnAdLoaded += OnBannerLoaded;
            // Called when an ad request failed to load.
            banner.OnAdFailedToLoad += OnBannerFailedToLoad;
            // Called when an ad is clicked.
            banner.OnAdOpening += OnBannerOpened;
            // Called when the user returned from the app after an ad click.
            banner.OnAdClosed += OnBannerClosed;
            // Called when the ad click caused the user to leave the application.
            banner.OnAdLeavingApplication += OnBannerLeftApplication;
        }


        public void OnBannerLoaded(object sender, EventArgs args)
        {
            if (serviceE.OnBannerLoaded != null)
            {
                serviceE.OnBannerLoaded(banner.MediationAdapterClassName());
            }
            //TextLog.Instance.Log("Banner Load");
        }

        public void OnBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (serviceE.OnBannerFailedToLoad != null)
            {
                serviceE.OnBannerFailedToLoad();
            }
            //TextLog.Instance.Log("Banner Fail:" + args.Message);
        }

        public void OnBannerOpened(object sender, EventArgs args)
        {
            if (serviceE.OnBannerOpened != null)
            {
                serviceE.OnBannerOpened(banner.MediationAdapterClassName());
            }
            //TextLog.Instance.Log("Banner Opened");
        }

        public void OnBannerClosed(object sender, EventArgs args)
        {
            if (serviceE.OnBannerClosed != null)
            {
                serviceE.OnBannerClosed(banner.MediationAdapterClassName());
            }
            //TextLog.Instance.Log("Banner Closed");
        }

        public void OnBannerLeftApplication(object sender, EventArgs args)
        {
            if (serviceE.OnBannerLeftApplication != null)
            {
                serviceE.OnBannerLeftApplication(banner.MediationAdapterClassName());
            }
            //TextLog.Instance.Log("Banner Left App");
        }
        #endregion
    }
}
#endif