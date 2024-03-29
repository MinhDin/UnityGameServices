﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using Services;
using System.Net;

public class AdmobServiceEditor : ServiceEditor
{
    const string PACKAGE_NAME = "AdmobPackage";

    public AdmobServiceEditor(ServiceDef def)
        : base(def)
    {
        packagePath = PACKAGE_NAME;
        RegisterUpdate();
    }

    protected override void Update()
    {
        bool lastRequest = request != null;
        base.Update();
        if (lastRequest && (request == null) && PackagesName != null)
        {
            foreach (string file in PackagesName)
            {
                if (file.Contains("unitypackage") && file.Contains("Admob"))
                {
                    string packagePath = "Assets/" + PACKAGE_NAME + "/" + file;
                    //AssetDatabase.ImportPackage(packagePath, true);
                    ImportPackageQueue.Instance.ImportPackage(packagePath);
                }
            }
        }
    }

    public override string GetName()
    {
        return "Admob";
    }

    public override void OnInspectorGUI(ServiceDefEditor editor)
    {
        if (!IsValidate())
        {
            if (def.UseAdmob)
            {
                def.UseAdmob = false;
                editor.RewriteDefine();
            }

            if ((packagesName != null) && (packagesName.Count == 0))
            {
                if (GreenButton("Get Admob Package"))
                {
                    def.UseAdmob = false;
                    DownloadPackage(editor);
                }
            }
            else
            {
                if (GreenButton("Import Admob Package"))
                {
                    def.UseAdmob = false;
                    DownloadPackage(editor);
                }
            }


            return;
        }
        if (!def.UseAdmob)
        {
            if (GreenButton("Active Admob"))
            {
                def.UseAdmob = true;
                editor.RewriteDefine();
            }
            else
            {
                return;
            }
        }

#if SERVICE_ADMOB
        def.AdmobAppID_IOS = EditorGUILayout.TextField("Admob ID IOS", def.AdmobAppID_IOS);
        def.AdmobAppID_Android = EditorGUILayout.TextField("Admob ID Android", def.AdmobAppID_Android);
        def.AdmobBannerID_IOS = EditorGUILayout.TextField("Banner ID IOS", def.AdmobBannerID_IOS);
        def.AdmobBannerID_Android = EditorGUILayout.TextField("Banner ID Android", def.AdmobBannerID_Android);
        def.AdmobInterstitialID_IOS = EditorGUILayout.TextField("Interstitial ID IOS", def.AdmobInterstitialID_IOS);
        def.AdmobInterstitialID_Android = EditorGUILayout.TextField("Interstitial ID Android", def.AdmobInterstitialID_Android);
        def.AdmobRewardedVideoID_IOS = EditorGUILayout.TextField("Rewarded Video ID IOS", def.AdmobRewardedVideoID_IOS);
        def.AdmobRewardedVideoID_Android = EditorGUILayout.TextField("Rewarded Video ID Android", def.AdmobRewardedVideoID_Android);
#endif

        //end section
        GUILayout.Space(50);
        if (RedButton("Remove Admob"))
        {
            def.UseAdmob = false;
            editor.RewriteDefine();
        }
    }

    public override bool IsValidate()
    {
        Type type = Type.GetType("GoogleMobileAds.Api.BannerView, Assembly-CSharp, Culture=neutral, PublicKeyToken=null");
        return type != null;
    }

    /*
	public override void DownloadPackage(ServiceDefEditor editor)
	{
        string link = editor.GetDownloadLinkByKey(PACKAGE_NAME);
        if(!string.IsNullOrEmpty(link))
        {
            WebClient webClient = new WebClient();
            Debug.Log("DOWN " + PACKAGE_NAME + link);
			string destination = Application.dataPath + "/" + PACKAGE_NAME + ".unitypackage";
            Debug.Log("DOWN TO:" + destination);
            webClient.DownloadFile(link, destination);
            AssetDatabase.Refresh();
            ImportPackageQueue.Instance.ImportPackage("Assets/" + PACKAGE_NAME +  ".unitypackage");
        }
	}
     */
    /*
        void AdmobValidate()
        {
            if(!DirectoryExist("GoogleMobileAds"))
            {
                LogError("* Please import GoogleMobileAds.unitypackage");
            }

            if(string.IsNullOrEmpty(def.AdmobAppID_IOS))
            {	
                LogError("* Get App Id from Admob : https://apps.admob.com/");
            }
        }
     */
    public override void OnWriteDefine(StreamWriter writer)
    {
        if (def.UseAdmob)
        {
            writer.WriteLine("-define:SERVICE_ADMOB");
        }
    }
}
