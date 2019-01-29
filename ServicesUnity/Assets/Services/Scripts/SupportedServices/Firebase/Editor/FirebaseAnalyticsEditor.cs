using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;
using System;

public class FirebaseAnalyticsEditor : ServiceEditor 
{
    FirebaseServiceEditor firebase;

	public FirebaseAnalyticsEditor(ServiceDef def, FirebaseServiceEditor firebase)
        : base(def)
    {
        this.firebase = firebase;
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

            if (GreenButton("Import Firebase Analytics Package"))
            {
                def.UseFBAnalytics = false;
                if((firebase.PackagesName != null) && (firebase.PackagesName.Count > 0))
                {
                    string finalName = firebase.PackagesName.Find(x => x.Contains("Analytics"));
                    ImportPackageQueue.Instance.ImportPackage(firebase.PackagePath + "/" + finalName);
                }
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
        Type type = Type.GetType("Firebase.Analytics.FirebaseAnalytics, Firebase.Analytics, Culture=neutral, PublicKeyToken=null");
		return type != null;
	}
	
	public override void OnWriteDefine(StreamWriter writer)
    {
		if(def.UseFBAnalytics)
		{
			writer.WriteLine("-define:SERVICE_FIREBASE_ANALYTICS");
		}
    }
}
