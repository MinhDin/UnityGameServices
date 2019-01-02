using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingDef : ScriptableObject
{
	public string AppID_IOS;
	public string PackageName;
	
	//Firebase services
	public bool UseFirebase;
	public bool UseFBAnalytics;
	public bool UseFBRealtimeDatabase;
	public bool UseFBLeaderBoard;
	public string FirebaseDatabaseURL = "https://word-link-e5bcf.firebaseio.com/";

	//Firebase Auth
	public bool UseFirebaseAuth;
	
	//OpenIAB
	public bool UseOpenIAB;
	public GameObject OpenIABManagerPrefab;
	public string AndroidPublicKey;
	public List<IABPackage> OpenIABIOSSkus;
	
	//Facebook
	public bool UseFacebook;

	//Admob
	public bool UseAdmob;
	public string AdmobAppID_IOS;
	public string AdmobAppID_Android;
	public string AdmobBannerID_IOS;
	public string AdmobBannerID_Android;
	public string AdmobInterstitialID_IOS;
	public string AdmobInterstitialID_Android;
	public string AdmobRewardedVideoID_IOS;
	public string AdmobRewardedVideoID_Android;

	//Flurry
	public bool UseFlurry;
	public string FlurryKey_IOS;
	public string FlurryKey_Android;
	public bool FlurryEnableLog = false;
	public bool FlurryEnableCrashReport = true;
	public bool FlurryReplicateDataToUnityAnalytics = false;
	public bool FlurryIOSIAPReportingEnabled = false;
	
	//AppsFlyer
	public bool UseAppsFlyer;
	public string AppsFlyerKey;

	//Applovin
	public bool UseApplovin;
	public string ApplovinKey;
	
	//Zendesk
	public bool UseZendesk;
	public string ZendeskChatKey;
	
#if UNITY_EDITOR
    [MenuItem("Scriptable/Services/Setting")]
    public static void CreateAsset()
    {
        SettingDef asset = ScriptableObject.CreateInstance<SettingDef>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ServiceEvents.asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif
}

[System.Serializable]
public class IABPackage
{
	public string Name;
	public string SKU;
	public int Coin;
	public string TextureKey;
	public string USDPrice;
	public string Bonus;
	public float ScaleImage;
	//public bool WillRemoveAds;
	public IABPackageFlag Flags;
}

[System.Flags]
public enum IABPackageFlag
{
	SHOW_IN_SHOP = 1,
	PROMOTE = 2,
	THEME = 4,
	REMOVE_ADS = 8,
	CONSUMABLE = 16,
}
