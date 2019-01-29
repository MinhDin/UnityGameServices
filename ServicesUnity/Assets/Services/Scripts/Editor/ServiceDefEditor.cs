//#define WRITE_JSON_LINK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;
using System;
using System.Linq;

[CustomEditor(typeof(ServiceDef))]
public class ServiceDefEditor : Editor
{

    string DEFINE_FILE_PATH;
    string SUPPORTED_SERVICES_PATH;
    string SERVICES_LINK_PATH = "Assets/Services/ServicesLink.json";

    ServiceDef def;
    static StreamWriter writer;
    //static
    static bool[] foldout;
    static int foldoutIndex;
    static string[] toolbar;
    static int toolbarIndex;
    static string error;
    static List<ServiceEditor> services;
    static ServicesLink link;
    static ImportPackageQueue importPackage;
    bool initStaticServices;

    static ServiceDefEditor()
    {
        InitStatic();
    }

    static void InitStatic()
    {
        toolbarIndex = 0;
        foldoutIndex = 0;
        foldout = new bool[50];
        importPackage = ImportPackageQueue.Instance;
        //toolbar = new string[]{"Firebase", "OpenIAB", "Facebook", "Admob", "Flurry", "AppFlyer", "Applovin", "Zendesk"};		

        if (services == null)
        {
            services = new List<ServiceEditor>();
        }
    }

    private void OnEnable()
    {
        def = target as ServiceDef;
    }

    public void RewriteDefine()
    {
        writer = new StreamWriter(DEFINE_FILE_PATH, false);
        //writer.WriteLine("//This file is generated automatically by LavaServices. Please don't modify. It assumes you don't use this file.:\">");
        int len = services.Count;
        for (int i = 0; i < len; ++i)
        {
            services[i].OnWriteDefine(writer);
        }
        writer.Close();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset("Assets/Services/Scripts/Editor/ServiceDefEditor.cs");
    }
#if WRITE_JSON_LINK
	bool a;
#endif

    void GetAllServices()
    {
        services.Clear();

        string[] directories = Directory.GetDirectories(SUPPORTED_SERVICES_PATH);
        int lenDir = directories.Length;

        for (int i = 0; i < lenDir; ++i)
        {
            string classType = new DirectoryInfo(directories[i]).Name + "ServiceEditor";
            Type type = Type.GetType(classType);
            if (type != null)
            {
                services.Add((ServiceEditor)Activator.CreateInstance(type, (object)def));
            }
        }

        toolbar = services.Select(x => x.GetName()).ToArray();
    }

    public override void OnInspectorGUI()
    {
        if (!initStaticServices)
        {
            if (services == null)
            {
                services = new List<ServiceEditor>();
            }

            initStaticServices = true;
            DEFINE_FILE_PATH = Application.dataPath + "/csc.rsp";
            SUPPORTED_SERVICES_PATH = Application.dataPath + "/Services/Scripts/SupportedServices";

#if WRITE_JSON_LINK
			if(!a)
			{
				ServicesLink link = new ServicesLink();
				link.Links = new DownloadLink[]{
					new DownloadLink{Name = "AdmobPackage",
						Link="https://github.com/googleads/googleads-mobile-unity/releases/download/v3.15.1/GoogleMobileAds.unitypackage"},
					new DownloadLink{Name = "AppsFlyerPackage",
						Link="https://github.com/AppsFlyerSDK/Unity/raw/master/AppsFlyerUnityPlugin_v4.18.0.unitypackage"},
					new DownloadLink{Name = "FacebookPackage",
						Link="https://origincache.facebook.com/developers/resources/?id=FacebookSDK-current.zip"},
					new DownloadLink{Name = "FirebasePackage",
						Link="https://dl.google.com/firebase/sdk/unity/firebase_unity_sdk_5.4.4.zip"},
					new DownloadLink{Name = "OpenIABPackage",
						Link="https://github.com/onepf/OpenIAB/releases/download/0.9.8.6/openiab-plugin-0.9.8.6.unitypackage"},	
				};
				writer = new StreamWriter(SERVICES_LINK_PATH, false);
				writer.Write(JsonUtility.ToJson(link));
				writer.Close();
			}
			return;
#endif

            TextAsset linkJson = AssetDatabase.LoadAssetAtPath(SERVICES_LINK_PATH, typeof(TextAsset)) as TextAsset;
            if (linkJson != null)
            {
                link = JsonUtility.FromJson<ServicesLink>(linkJson.text);
            }
            else
            {
                initStaticServices = false;
                Debug.LogError("File not found : " + SERVICES_LINK_PATH);
                return;
            }

            new ServiceEditor(null);

			GetAllServices();
        }

        //validate variables
        if (foldout == null)
        {
            InitStatic();
        }

        if (services.Count == 0)
        {
            GetAllServices();
            return;
        }
        error = string.Empty;
        foldoutIndex = 0;

        //============

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbar);
        int len = services.Count;
        toolbarIndex = Mathf.Min(toolbarIndex, len - 1);
        services[toolbarIndex].OnInspectorGUI(this);

        GUILayout.Space(50);
        if (!string.IsNullOrEmpty(error))
        {
            //RedLabel(error);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(def);
        }
    }

    public string GetDownloadLinkByKey(string key)
    {
        if (link != null)
        {
            return link.GetDownloadLinkByKey(key);
        }

        return string.Empty;
    }
}
