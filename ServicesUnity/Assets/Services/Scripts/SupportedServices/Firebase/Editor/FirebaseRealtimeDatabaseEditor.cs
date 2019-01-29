﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;
using System;

public class FirebaseRealtimeDatabaseEditor : ServiceEditor 
{
	FirebaseServiceEditor firebase;

    public FirebaseRealtimeDatabaseEditor(ServiceDef def, FirebaseServiceEditor firebase)
        : base(def)
    {
		this.firebase = firebase;
    }

	public override string GetName()
    {
        return "Firebase RealtimeDB";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!IsValidate())
        {
            if(def.UseFBRealtimeDatabase)
            {
                def.UseFBRealtimeDatabase = false;
                editor.RewriteDefine();
            }

            if (GreenButton("Import Firebase Remote Config Package"))
            {
                def.UseFBRealtimeDatabase = false;

                if((firebase.PackagesName != null) && (firebase.PackagesName.Count > 0))
                {
                    string finalName = firebase.PackagesName.Find(x => x.Contains("Analytics"));
                    ImportPackageQueue.Instance.ImportPackage(firebase.PackagePath + "/" + finalName);
                }
            }
            
            return;
        }
        if (!def.UseFBRealtimeDatabase)
        {
            if (GreenButton("Active Firebase Remote Config"))
            {
                def.UseFBRealtimeDatabase = true;
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
            def.UseFBRealtimeDatabase = false;
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
		Type type = Type.GetType("Firebase.RemoteConfig.FirebaseRemoteConfig, Firebase.RemoteConfig, Culture=neutral, PublicKeyToken=null");
		return type != null;
	}

	
	public override void OnWriteDefine(StreamWriter writer)
    {
		if(def.UseFBRealtimeDatabase)
		{
			writer.WriteLine("-define:SERVICE_FIREBASE_REALTIMEDATABASE");
		}
    }
}
