using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AppsFlyerEditor : ServiceEditor 
{
	public AppsFlyerEditor(SettingDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Appflyer";
    }

	public override void OnInspectorGUI()
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

	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("#define SERVICE_APPSFLYER");
    }
}
