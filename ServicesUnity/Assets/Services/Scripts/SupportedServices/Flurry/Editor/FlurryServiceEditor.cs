using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FlurryServiceEditor : ServiceEditor 
{
	public FlurryServiceEditor(SettingDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Flurry";
    }

	public override void OnInspectorGUI()
	{
		if(!def.UseFlurry)
		{
			if(GreenButton("Active Flurry"))
			{
				def.UseFlurry = true;
			}
			return;
		}

		def.FlurryKey_IOS = EditorGUILayout.TextField("Key IOS", def.FlurryKey_IOS);
		def.FlurryKey_Android = EditorGUILayout.TextField("Key Android", def.FlurryKey_Android);
		def.FlurryEnableLog = EditorGUILayout.Toggle("Enable Log", def.FlurryEnableLog);
		def.FlurryEnableCrashReport = EditorGUILayout.Toggle("Enable Crash Report", def.FlurryEnableCrashReport);
		def.FlurryReplicateDataToUnityAnalytics = EditorGUILayout.Toggle("Replicate Data to Unity Analytics", def.FlurryReplicateDataToUnityAnalytics);
		def.FlurryIOSIAPReportingEnabled = EditorGUILayout.Toggle("IOS IAP Reporting", def.FlurryIOSIAPReportingEnabled);

		if(RedButton("Remove Flurry"))
		{
			def.UseFlurry = false;
		}
	}

	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("#define SERVICE_FLURRY");
    }
}
