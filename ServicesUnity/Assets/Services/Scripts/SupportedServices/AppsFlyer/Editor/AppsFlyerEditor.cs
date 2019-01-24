using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;
using System.Net;

public class AppsFlyerEditor : ServiceEditor 
{
	const string PACKAGE_NAME = "AppsflyerPackage";

	public AppsFlyerEditor(ServiceDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Appflyer";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!def.UseAppsFlyer)
		{
			if(GreenButton("Active AppsFlyer"))
			{
				def.UseAppsFlyer = true;
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
		}
	}

	public override bool IsValidate()
	{
		return false;
	}

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
            AssetDatabase.ImportPackage("Assets/" + PACKAGE_NAME +  ".unitypackage", true);
        }
	}
	
	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("-define:SERVICE_APPSFLYER");
    }
}
