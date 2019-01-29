using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Services;
using System.Net;
using System;
using UnityEngine.Networking;

public class FacebookServiceEditor : ServiceEditor
{
    const string PACKAGE_NAME = "FacebookPackage";

	//UnityWebRequest request;
    public FacebookServiceEditor(ServiceDef def)
        : base(def)
    {
        //EditorApplication.update += Update;
        packagePath = "FacebookPackage";
		RegisterUpdate();
    }

    protected override void Update()
    {
        bool lastRequest = request != null;
        base.Update();
        if(lastRequest && (request == null) && PackagesName != null)
        {
            foreach(string file in PackagesName)
			{
				if(file.Contains("unitypackage") && file.Contains("facebook-unity"))
				{
					string packagePath = "Assets/" + PACKAGE_NAME + "/" + file;
					Debug.Log("Import :" + packagePath);
					//AssetDatabase.ImportPackage(packagePath, true);
					ImportPackageQueue.Instance.ImportPackage(packagePath);
				}
			}
        }
    }
/*
    private void Update()
    {
		if((request != null) && (request.isDone))
		{
			request = null;
			string destination = Application.dataPath + "/" + PACKAGE_NAME + ".zip";
			List<string> rs = DecompressSharpZip(destination, Application.dataPath + "/" + PACKAGE_NAME);
			foreach(string file in rs)
			{
				if(file.Contains("unitypackage") && file.Contains("facebook-unity"))
				{
					string packagePath = "Assets/" + PACKAGE_NAME + "/" + file;
					Debug.Log("Import :" + packagePath);
					//AssetDatabase.ImportPackage(packagePath, true);
					ImportPackageQueue.Instance.ImportPackage(packagePath);
				}
			}
		}
    }
*/
    public override string GetName()
    {
        return "Facebook";
    }

    public override void OnInspectorGUI(ServiceDefEditor editor)
    {
        if (!IsValidate())
        {
            if (def.UseFacebook)
            {
                def.UseFacebook = false;
                editor.RewriteDefine();
            }

            if (GreenButton("Download Facebook Package"))
            {
                def.UseFacebook = false;
				Debug.Log("Begin download.");
                DownloadPackage(editor);
				Debug.Log("After download.");
            }
            return;
        }

        if (!def.UseFacebook)
        {
            if (GreenButton("Active Facebook"))
            {
                def.UseFacebook = true;
                editor.RewriteDefine();
            }
            return;
        }

        //end section
        GUILayout.Space(50);
        if (RedButton("Remove Facebook"))
        {
            def.UseFacebook = false;
            editor.RewriteDefine();
        }
    }

    public override bool IsValidate()
    {
        return false;
    }
/*
    public override void DownloadPackage(ServiceDefEditor editor)
    {
        string link = editor.GetDownloadLinkByKey(PACKAGE_NAME);
        if (!string.IsNullOrEmpty(link))
        {
            Debug.Log("DOWN " + PACKAGE_NAME + link);
            string destination = Application.dataPath + "/" + PACKAGE_NAME + ".zip";
            Debug.Log("DOWN TO:" + destination);

			ServiceDownloadWindow downloadWindow = ServiceDownloadWindow.ShowWindow();
			request = downloadWindow.Download(link, destination);
        }
    }	
*/
    public override void OnWriteDefine(StreamWriter writer)
    {
        if (def.UseFacebook)
        {
            writer.WriteLine("-define:SERVICE_FACEBOOK");
        }
    }

}
