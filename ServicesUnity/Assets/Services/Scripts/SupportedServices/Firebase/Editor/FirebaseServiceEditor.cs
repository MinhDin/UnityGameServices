using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Services;

public class FirebaseServiceEditor : ServiceEditor 
{
	FirebaseAnalyticsEditor analytics;
	FirebaseRealtimeDatabaseEditor realtimeDatabase;

	public FirebaseServiceEditor(ServiceDef def)
        : base(def)
    {
		analytics = new FirebaseAnalyticsEditor(def);
		realtimeDatabase = new FirebaseRealtimeDatabaseEditor(def);
    }

	public override string GetName()
    {
        return "Firebase";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!def.UseFirebase)
		{
			if(GreenButton("Active Firebase"))
			{
				def.UseFirebase = true;
				editor.RewriteDefine();
			}
			return;
		}
		
		def.UseFBAnalytics = BoldToggle("Use Analytics", def.UseFBAnalytics);
		
		realtimeDatabase.OnInspectorGUI(editor);

		
		//end section
		GUILayout.Space(50);
		if(RedButton("Remove Firebase"))
		{
			def.UseFirebase = false;
			editor.RewriteDefine();
		}
	}

	public override bool IsValidate()
	{
		return false;
	}

	public override void DownloadPackage(ServiceDefEditor editor)
	{

	}
	
	public override void OnWriteDefine(StreamWriter writer)
    {
		if(def.UseFirebase)
		{
			writer.WriteLine("-define:SERVICE_FIREBASE");
			analytics.OnWriteDefine(writer);
			realtimeDatabase.OnWriteDefine(writer);
		}
    }
}
