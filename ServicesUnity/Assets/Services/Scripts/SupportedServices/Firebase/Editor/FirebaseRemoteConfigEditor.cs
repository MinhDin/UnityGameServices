using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Services;
using System;

public class FirebaseRemoteConfigEditor : ServiceEditor
{
	FirebaseServiceEditor firebase;

    public FirebaseRemoteConfigEditor(ServiceDef def, FirebaseServiceEditor firebase)
        : base(def)
    {
		this.firebase = firebase;
    }

    public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!IsValidate())
        {
            if(def.UseFBRemoteConfig)
            {
                def.UseFBRemoteConfig = false;
                editor.RewriteDefine();
            }

            if (GreenButton("Import Firebase Remote Config Package"))
            {
                def.UseFBRemoteConfig = false;

                if((firebase.PackagesName != null) && (firebase.PackagesName.Count > 0))
                {
                    string finalName = firebase.PackagesName.Find(x => x.Contains("Analytics"));
                    ImportPackageQueue.Instance.ImportPackage(firebase.PackagePath + "/" + finalName);
                }
            }
            
            return;
        }
        if (!def.UseFBRemoteConfig)
        {
            if (GreenButton("Active Firebase Remote Config"))
            {
                def.UseFBRemoteConfig = true;
                editor.RewriteDefine();
            }
            else
            {
                return;
            }
        }

		//end section
        GUILayout.Space(50);
        if (RedButton("Remove Firebase Remote Config"))
        {
            def.UseFBRemoteConfig = false;
            editor.RewriteDefine();
        }
	}

	public override void OnWriteDefine(StreamWriter writer)
	{
		if(def.UseFBAnalytics)
		{
			writer.WriteLine("-define:SERVICE_FIREBASE_REMOTECONFIG");
		}
	}
	
	public override bool IsValidate()
	{
		Type type = Type.GetType("Firebase.RemoteConfig.FirebaseRemoteConfig, Firebase.RemoteConfig, Culture=neutral, PublicKeyToken=null");
		return type != null;
	}

	public override string GetName()
	{
		return "Remote Config";
	}
}
