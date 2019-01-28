using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;

public class FirebaseAnalyticsEditor : ServiceEditor 
{
	public FirebaseAnalyticsEditor(ServiceDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Firebase Analytics";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!IsValidate())
        {
            if(def.UseFBAnalytics)
            {
                def.UseFBAnalytics = false;
                editor.RewriteDefine();
            }

            if (GreenButton("Import Firebase Package"))
            {
                def.UseFBAnalytics = false;
                DownloadPackage(editor);
            }
            
            return;
        }
        if (!def.UseFBAnalytics)
        {
            if (GreenButton("Active Firebase Analytics"))
            {
                def.UseFBAnalytics = true;
                editor.RewriteDefine();
            }
            else
            {
                return;
            }
        }

		//end section
        GUILayout.Space(50);
        if (RedButton("Remove Firebase Analytics"))
        {
            def.UseFBAnalytics = false;
            editor.RewriteDefine();
        }
	}

	void RemoteDatabaseValidate()
	{
		if(string.IsNullOrEmpty(def.FirebaseDatabaseURL))
		{	
			LogError("* Firebase database URL : https://YOUR-FIREBASE-APP.firebaseio.com/");
		}

		if(!FileExist("Firebase/Plugins/Firebase.Database.dll"))
		{
			LogError("* Firebase database package : FirebaseDatabase.unitypackage");
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
		if(def.UseFBAnalytics)
		{
			writer.WriteLine("-define:SERVICE_FIREBASE_ANALYTICS");
		}
    }
}
