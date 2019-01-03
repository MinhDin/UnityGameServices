using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
		}
	}

	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("#define SERVICE_FACEBOOK");
    }
}
