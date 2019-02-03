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
        packagePath = PACKAGE_NAME;
        RegisterUpdate();
    }

    protected override void Update()
    {
        bool lastRequest = request != null;
        base.Update();
        if (lastRequest && (request == null) && PackagesName != null)
        {
            foreach (string file in PackagesName)
            {
                if (file.Contains("unitypackage") && file.Contains("facebook-unity"))
                {
                    string packagePath = "Assets/" + PACKAGE_NAME + "/" + file;
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
        if (BasicInspectorHeader(editor, "facebook-unity", ref def.UseFacebook))
        {
            BasicInspectorFooter(editor, ref def.UseFacebook);
        }
    }

    public override bool IsValidate()
    {
        Type type = Type.GetType("Facebook.Unity.FB, Facebook.Unity, Culture=neutral, PublicKeyToken=null");
        return type != null;
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