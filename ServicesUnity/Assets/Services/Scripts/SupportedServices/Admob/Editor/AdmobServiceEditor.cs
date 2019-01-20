using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class AdmobServiceEditor : ServiceEditor
{
    public AdmobServiceEditor(ServiceDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Admob";
    }

    public override void OnInspectorGUI(ServiceDefEditor editor)
    {
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
        }
    }
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
        if(def.UseAdmob)
        {
		    writer.WriteLine("#define SERVICE_ADMOB");
        }
    }
}
