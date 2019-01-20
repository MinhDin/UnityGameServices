﻿#if SERVICE_APPSFLYER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Services
{
    public class AppsFlyerService : IService
    {

		const string click_ad = "click_ad";
		const string show_ad = "show_ad";
		const string show_banner = "show_banner";
		const string show_interstitial = "show_interstitial";
		const string click_banner_ad = "click_banner_ad";
		const string click_interstitial_ad = "click_interstitial_ad";
		const string complete_rewarded_ads = "complete_rewarded_ads";
		const string finish_tutorial = "finish_tutorial";
		const string open_2nd_session_d1 = "open_2nd_session_d1";
		const string open_2nd_session_d2 = "open_2nd_session_d2";
		const string finish_2nd_game = "finish_2nd_game";
		const string finish_3rd_game = "finish_3rd_game";
		const string finish_1st_game_d1 = "finish_1st_game_d1";
		const string af_revenue = "af_revenue";
		const string purchased_iap = "purchase_iap";
		const string purchased_iap_package = "purchase_iap_package";
		const string purchased_iap_value = "purchase_iap_value";

        public override IService Init(SettingDef def, ServiceEvents events)
        {
            if (!def.UseAppsFlyer)
            {
                return null;
            }
            base.Init(def, events);
            AppsFlyer.setAppsFlyerKey(def.AppsFlyerKey);

#if DEBUG_APPFLYER
            AppsFlyer.setIsDebug (true);
#endif

#if UNITY_IOS
            /* Mandatory - set your apple app ID
               NOTE: You should enter the number only and not the "ID" prefix */
            AppsFlyer.setAppID(def.AppID_IOS);
            AppsFlyer.trackAppLaunch();
			AppsFlyer.getConversionData();
#elif UNITY_ANDROID
			/* Mandatory - set your Android package name */
			AppsFlyer.setAppID (def.PackageName);
			/* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
			AppsFlyer.init (def.AppsFlyerKey,"AppsFlyerTrackerCallbacks");
#endif

			GameObject obj = new GameObject("AppsFlyerTrackerCallbacks");
			obj.AddComponent<AppsFlyer>();
			AppsFlyerTrackerCallbacks callback = obj.AddComponent<AppsFlyerTrackerCallbacks>();
			callback.ServiceE = serviceE;

			serviceE.OnBannerLeftApplication += OnClickAd;
			serviceE.OnBannerLeftApplication += OnClickBanner;
			serviceE.OnBannerLoaded += OnBannerLoadSuccess;
			serviceE.OnInterstitialOpened += OnShowInterstitial;
			serviceE.OnRewardVideoOpened += OnShowRewardedAds;
			serviceE.OnInterstitialLeftApplication += OnClickAd;
			serviceE.OnInterstitialLeftApplication += OnClickInterstitial;
			serviceE.OnRewardVideoLeftApplication += OnClickAd;
			serviceE.OnRewardVideoRewarded += OnCompetedRewardedAd;
			serviceE.OnTutorialFinishStep += OnTutorialFinish;
			serviceE.OnStartSession += OnStartSession;
			serviceE.OnLevelEnd += OnEndLevel;
			serviceE.OnPurchaseSucceeded += OnPurchasedIAPSuccess;
            return this;
        }


        public override void Dispose()
        {
			serviceE.OnBannerLeftApplication -= OnClickAd;
			serviceE.OnBannerLeftApplication -= OnClickBanner;
			serviceE.OnBannerLoaded -= OnBannerLoadSuccess;
			serviceE.OnInterstitialOpened -= OnShowInterstitial;
			serviceE.OnRewardVideoOpened -= OnShowRewardedAds;
			serviceE.OnInterstitialLeftApplication -= OnClickAd;			
			serviceE.OnInterstitialLeftApplication -= OnClickInterstitial;
			serviceE.OnRewardVideoLeftApplication -= OnClickAd;
			serviceE.OnRewardVideoRewarded -= OnCompetedRewardedAd;
			serviceE.OnTutorialFinishStep -= OnTutorialFinish;
			serviceE.OnStartSession -= OnStartSession;
			serviceE.OnLevelEnd -= OnEndLevel;
			serviceE.OnPurchaseSucceeded -= OnPurchasedIAPSuccess;
        }

		void OnPurchasedIAPSuccess(string sku)
		{
			IABPackage package = null;
			int len = def.OpenIABIOSSkus.Count;
			for(int i = 0; i < len; ++i)
			{
				if(def.OpenIABIOSSkus[i].SKU.Equals(sku))
				{
					package = def.OpenIABIOSSkus[i];
					break;
				}
			}
			
			TrackingParameter[] trackParam = new TrackingParameter[2];
			if(package != null)
			{				
				string price = package.USDPrice.Trim('$', ' ');
				trackParam[0] = new TrackingParameter(af_revenue, price);
				trackParam[1] = new TrackingParameter(purchased_iap_package, sku);
			}
			else
			{
				trackParam[0] = new TrackingParameter(af_revenue, "0");
				trackParam[1] = new TrackingParameter(purchased_iap_package, sku);
			}

			TrackEventParam(purchased_iap, trackParam);
		}

		void OnBannerLoadSuccess(string source)
		{
			OnShowAds(source);
			TrackEventNoParam(show_banner);
		}

		void OnShowInterstitial(string source)
		{
			OnShowAds(source);
			TrackEventNoParam(show_interstitial);
		}

		void OnShowRewardedAds(string source)
		{
			OnShowAds(source);
		}

		void OnShowAds(string source)
		{
			TrackEventNoParam(show_ad);
		}

		void OnEndLevel(int endCount)
		{
			int day = Mathf.CeilToInt((float)(DateTime.Now.Subtract(UpdateSecondsFromInstall.INITIAL_DAY).TotalSeconds - PlayerPrefs.GetInt(UpdateSecondsFromInstall.SecondsFromAnchorKey, 0)) / (60 * 60 * 24));
			if((endCount == 1) && (day == 1))
			{
				TrackEventNoParam(finish_1st_game_d1);
			}
			if(endCount == 2)
			{
				TrackEventNoParam(finish_2nd_game);				
			}
			else if(endCount == 3)
			{
				TrackEventNoParam(finish_3rd_game);
			}
		}

		void OnStartSession(int totalSession)
		{
			int day = Mathf.CeilToInt((float)(DateTime.Now.Subtract(UpdateSecondsFromInstall.INITIAL_DAY).TotalSeconds - PlayerPrefs.GetInt(UpdateSecondsFromInstall.SecondsFromAnchorKey, 0)) / (60 * 60 * 24));
			if(totalSession == 2)
			{
				if(day == 1)
				{
					TrackEventNoParam(open_2nd_session_d1);
				}
				else if(day == 2)
				{
					TrackEventNoParam(open_2nd_session_d2);
				}
			}
		}

		void OnTutorialFinish(int step)
		{
			if(step == 5)
			{
				TrackEventNoParam(finish_tutorial);
			}
		}

		void OnCompetedRewardedAd(string adNetwork, string type, double amount)
		{
			TrackingParameter[] trackParam = new TrackingParameter[1];
			trackParam[0] = new TrackingParameter(af_revenue, CPCMgr.CPC_RewardedAds.ToString());
			TrackEventParam(complete_rewarded_ads, trackParam);
		}

		void OnClickBanner(string adNetwork)
		{
			TrackingParameter[] trackParam = new TrackingParameter[1];
			trackParam[0] = new TrackingParameter(af_revenue, CPCMgr.CPC_Banner.ToString());
			TrackEventParam(click_banner_ad, trackParam);
		}

		void OnClickInterstitial(string adNetwork)
		{
			TrackingParameter[] trackParam = new TrackingParameter[1];
			trackParam[0] = new TrackingParameter(af_revenue, CPCMgr.CPC_Interstitial.ToString());
			TrackEventParam(click_interstitial_ad, trackParam);
		}

		void OnClickAd(string adNetwork)
		{
			TrackEventNoParam(click_ad);
		}

		void TrackEventNoParam(string eventName)
		{
			AppsFlyer.trackRichEvent(eventName, new Dictionary<string, string>());
		}

		void TrackEventParam(string eventName, TrackingParameter[] trackParams)
		{
			int len = trackParams.Length;
			Dictionary<string, string> dicParams = new Dictionary<string, string>(len);
			for(int i = 0; i < len; ++i)
			{
				dicParams.Add(trackParams[i].ID, trackParams[i].GetValueAsString());
			}

			AppsFlyer.trackRichEvent(eventName, dicParams);
		}

    }
}
#endif