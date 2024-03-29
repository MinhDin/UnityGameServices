﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Services
{
    public partial class ServiceDef : ScriptableObject
    {
        public string AppID_IOS;
        public string PackageName;

        //Firebase services
        public bool UseFirebase;
        public bool UseFBAnalytics;
        public bool UseFBRemoteConfig;
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
        public bool UseAppslovin;
        public string ApplovinKey;

        //Zendesk
        public bool UseZendesk;
        public string ZendeskChatKey;

#if UNITY_EDITOR
        [MenuItem("Scriptable/Services/Setting")]
        public static void CreateAsset()
        {
            ServiceDef asset = ScriptableObject.CreateInstance<ServiceDef>();

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
}
