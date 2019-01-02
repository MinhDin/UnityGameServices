using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;
using System;
using System.Linq;

[CustomEditor(typeof(SettingDef))]
public class SettingDefEditor : Editor
{
	string DEFINE_FILE_PATH;
	string SUPPORTED_SERVICES_PATH;

	SettingDef def;
	static StreamWriter writer;
	//static
	static bool[] foldout;
	static int foldoutIndex;
	static string[] toolbar;
	static int toolbarIndex;	
	static string error;
	static List<ServiceEditor> services;

	bool initStaticServices;

	static SettingDefEditor()
	{		
		InitStatic();
	}
	
	static void InitStatic()
	{
		toolbarIndex = 0;
		foldoutIndex = 0;
		foldout = new bool[50];
		//toolbar = new string[]{"Firebase", "OpenIAB", "Facebook", "Admob", "Flurry", "AppFlyer", "Applovin", "Zendesk"};		

		if(services == null)
		{
			services = new List<ServiceEditor>();
		}
	}

	private void OnEnable()
	{
		def = target as SettingDef;
	}

	void RewriteDefine()
	{
		writer = new StreamWriter(DEFINE_FILE_PATH, false);
		writer.Close();
	}
	public override void OnInspectorGUI()
	{
		//if(!initStaticServices)
		{
			if(services == null)
			{
				services = new List<ServiceEditor>();
			}
			services.Clear();
			initStaticServices = true;
			DEFINE_FILE_PATH = Application.dataPath + "/Services/Scripts/Define.h";
			SUPPORTED_SERVICES_PATH = Application.dataPath + "/Services/Scripts/SupportedServices";

			new ServiceEditor(null);
			string[] directories = Directory.GetDirectories(SUPPORTED_SERVICES_PATH);
			int lenDir = directories.Length;
			
			for(int i = 0; i < lenDir; ++i)
			{
				string classType = new DirectoryInfo(directories[i]).Name + "ServiceEditor";
				Type type = Type.GetType(classType);
				if(type != null)
				{
					services.Add((ServiceEditor)Activator.CreateInstance(type, (object)def));
				}
			}

			toolbar = services.Select(x => x.GetName()).ToArray();
		}

		//validate variables
		if(foldout == null)
		{
			InitStatic();
		}
		
		if(services.Count == 0)
		{
			Debug.Log("Can't find any services.");
			return;
		}
		error = string.Empty;
		foldoutIndex = 0;
		
		//============

		toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbar);
		int len = services.Count;
		toolbarIndex = Mathf.Min(toolbarIndex, len - 1);
		services[toolbarIndex].OnInspectorGUI();

		GUILayout.Space(50);
		if(!string.IsNullOrEmpty(error))
		{
			//RedLabel(error);
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(def);
		}
	}

	//Auth
	void AuthValidate()
	{
		if(def.UseFBRealtimeDatabase)
		{
			def.UseFirebaseAuth = true;
		}
		else 
		{
			def.UseFirebaseAuth = false;
		}
	}
}
