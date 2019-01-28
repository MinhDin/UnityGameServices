using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Services;
using System.Linq;

public class FirebaseServiceEditor : ServiceEditor 
{
	public const string PACKAGE_ROOT = "FirebasePackage";
	FirebaseAnalyticsEditor analytics;
	FirebaseRealtimeDatabaseEditor realtimeDatabase;
	
	ServiceEditor[] fbServices;
	string[] toolbar;
	int toolbarIndex;

	public FirebaseServiceEditor(ServiceDef def)
        : base(def)
    {
		fbServices = new ServiceEditor[]
		{
			new FirebaseAnalyticsEditor(def),
			new FirebaseRemoteConfigEditor(def),
			new FirebaseRealtimeDatabaseEditor(def),
		};
		toolbar = fbServices.Select(x => x.GetName()).ToArray();

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
		
		toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbar);

		fbServices[toolbarIndex].OnInspectorGUI(editor);
		//def.UseFBAnalytics = BoldToggle("Use Analytics", def.UseFBAnalytics);
		//realtimeDatabase.OnInspectorGUI(editor);
		
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
		return true;
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
