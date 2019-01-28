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
	public List<string> PackagesName{get{return packagesName;}}
	List<string> packagesName;
	
	ServiceEditor[] fbServices;
	string[] toolbar;
	int toolbarIndex;
	
	UnityWebRequest request;

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

		EditorApplication.update += Update;
    }

	private void Update()
    {
		if((request != null) && (request.isDone))
		{
			request = null;
			string destination = Application.dataPath + "/" + PACKAGE_NAME + ".zip";
			packagesName = DecompressSharpZip(destination, Application.dataPath + "/" + PACKAGE_NAME);
		}
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
				Debug.Log("Begin download.");
                DownloadPackage(editor);
				Debug.Log("After download.");
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

	public override void DownloadPackage(ServiceDefEditor editor)
	{
		string link = editor.GetDownloadLinkByKey(PACKAGE_NAME);
        if (!string.IsNullOrEmpty(link))
        {
            Debug.Log("DOWN " + PACKAGE_NAME + link);
            string destination = Application.dataPath + "/" + PACKAGE_NAME + ".zip";
            Debug.Log("DOWN TO:" + destination);

			ServiceDownloadWindow downloadWindow = ServiceDownloadWindow.ShowWindow();
			downloadWindow.Download(link, destination);
        }
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
