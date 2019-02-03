using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;
using System.Net;
using System;

public class AppsFlyerServiceEditor : ServiceEditor 
{
	const string PACKAGE_NAME = "AppsFlyerPackage";

	public AppsFlyerServiceEditor(ServiceDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "AppsFlyer";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!IsValidate())
        {
            if(def.UseAppslovin)
            {
                def.UseAppslovin = false;
                editor.RewriteDefine();
            }

            if (GreenButton("Get AppsFlyer Package"))
            {
                def.UseAppslovin = false;
                DownloadPackage(editor);
            }
            
            return;
        }

		if(!def.UseAppsFlyer)
		{
			if(GreenButton("Active AppsFlyer"))
			{
				def.UseAppsFlyer = true;
				editor.RewriteDefine();
			}
			return;
		}

		def.AppsFlyerKey = EditorGUILayout.TextField("AppsFlyer Key", def.AppsFlyerKey);
		def.AppID_IOS = EditorGUILayout.TextField("AppID IOS", def.AppID_IOS);

		//end section
		GUILayout.Space(50);
		if(RedButton("Remove AppFlyer"))
		{
			def.UseAppsFlyer = false;
			editor.RewriteDefine();
		}
	}

	public override bool IsValidate()
	{
		Type type = Type.GetType("AppsFlyer, Assembly-CSharp-firstpass, Culture=neutral, PublicKeyToken=null");
        return type != null;
	}

	public override void DownloadPackage(ServiceDefEditor editor)
	{
		string link = editor.GetDownloadLinkByKey(PACKAGE_NAME);
        if(!string.IsNullOrEmpty(link))
        {
            WebClient webClient = new WebClient();
			string destination = Application.dataPath + "/" + PACKAGE_NAME + ".unitypackage";
            webClient.DownloadFile(link, destination);
            AssetDatabase.Refresh();
            AssetDatabase.ImportPackage("Assets/" + PACKAGE_NAME +  ".unitypackage", true);
        }
	}
	
	public override void OnWriteDefine(StreamWriter writer)
    {
		if(def.UseAppslovin)
		{
			writer.WriteLine("-define:SERVICE_APPSFLYER");
		}
    }
}
