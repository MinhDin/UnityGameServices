using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Services;
using System.Linq;
using UnityEditor;
using System.Net;
using System;
using UnityEngine.Networking;

public class FirebaseServiceEditor : ServiceEditor 
{
	public const string PACKAGE_NAME = "FirebasePackage";
	
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

		packagePath = "FirebasePackage";
		RegisterUpdate();
    }

	public override string GetName()
    {
        return "Firebase";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if (!IsValidate())
        {
            if (def.UseFirebase)
            {
                def.UseFirebase = false;
                editor.RewriteDefine();
            }

            if (GreenButton("Download Firebase Package"))
            {
                def.UseFirebase = false;
                DownloadPackage(editor);
            }
            return;
        }

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
		return FileExist(PACKAGE_NAME);
	}

	public override void OnWriteDefine(StreamWriter writer)
    {
		if(def.UseFirebase)
		{
			foreach (ServiceEditor sv in fbServices)
			{
				sv.OnWriteDefine(writer);
			}			
		}
    }
}
