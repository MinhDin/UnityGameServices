using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class OpenIABServiceEditor : ServiceEditor 
{
	bool foldout;

	public OpenIABServiceEditor(SettingDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "OpenIAB";
    }

	public override void OnInspectorGUI()
	{
		if(!def.UseOpenIAB)
		{
			if(GreenButton("Active OpenIAB"))
			{
				def.UseOpenIAB = true;
			}
			return;
		}

		def.OpenIABManagerPrefab = EditorGUILayout.ObjectField("Manager Prefab", def.OpenIABManagerPrefab, typeof(GameObject), false) as GameObject;
		def.AndroidPublicKey = EditorGUILayout.TextField("Android Public Key", def.AndroidPublicKey);
		foldout = EditorGUILayout.Foldout(foldout, "IAB Packages");
		if(foldout)
		{
			
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			float size = (Screen.width - 60)  / (Screen.dpi > 250f ? 18.0f : 9.0f) - 4;
			EditorGUILayout.BeginHorizontal();
			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("Name", GUILayout.Width(size));
			EditorGUILayout.LabelField("Coin", GUILayout.Width(size));
			EditorGUILayout.LabelField("USD Price", GUILayout.Width(size));
			EditorGUILayout.LabelField("Bonus", GUILayout.Width(size));
			EditorGUILayout.LabelField("Texture Key", GUILayout.Width(size));
			EditorGUILayout.LabelField("Scale Image", GUILayout.Width(size));
			EditorGUILayout.LabelField("Flags", GUILayout.Width(size));
			EditorGUILayout.LabelField("SKU", GUILayout.Width(size));
			EditorStyles.label.fontStyle = FontStyle.Normal;
			EditorGUILayout.EndHorizontal();

			for(int i = 0; i < def.OpenIABIOSSkus.Count; ++i)
			{
				EditorGUILayout.BeginHorizontal();	
				if(i % 2 == 1)			
				{
					GUI.backgroundColor = new Color(0.75f, 0.83f, 0.83f, 1.0f);
				}
				else
				{
					GUI.backgroundColor = baseBackgroundColor;
				}
				def.OpenIABIOSSkus[i].Name = EditorGUILayout.TextField(def.OpenIABIOSSkus[i].Name, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].Coin = EditorGUILayout.IntField(def.OpenIABIOSSkus[i].Coin, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].USDPrice = EditorGUILayout.TextField(def.OpenIABIOSSkus[i].USDPrice, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].Bonus = EditorGUILayout.TextField(def.OpenIABIOSSkus[i].Bonus, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].TextureKey = EditorGUILayout.TextField(def.OpenIABIOSSkus[i].TextureKey, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].ScaleImage = EditorGUILayout.FloatField(def.OpenIABIOSSkus[i].ScaleImage, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].Flags = (IABPackageFlag)EditorGUILayout.EnumFlagsField(def.OpenIABIOSSkus[i].Flags, GUILayout.Width(size));
				def.OpenIABIOSSkus[i].SKU = EditorGUILayout.TextField(def.OpenIABIOSSkus[i].SKU, GUILayout.Width(size));
				if(GreenButton("^", GUILayout.Width(20), GUILayout.Height(20)))
				{
					var temp = def.OpenIABIOSSkus[i];
					def.OpenIABIOSSkus[i] = def.OpenIABIOSSkus[i-1];
					def.OpenIABIOSSkus[i-1] = temp;
				}
				if(RedButton("X", GUILayout.Width(20), GUILayout.Height(20)))
				{
					def.OpenIABIOSSkus.RemoveAt(i);
					i--;
				}

				GUI.backgroundColor = baseBackgroundColor;
				EditorGUILayout.EndHorizontal();
			}

			if(GreenButton("Add"))
			{
				def.OpenIABIOSSkus.Add(new IABPackage());
			}
			EditorGUILayout.EndVertical();

			
		}

		OpenIABValidate();
		//end section
		GUILayout.Space(50);
		if(RedButton("Remove OpenIAB"))
		{
			def.UseOpenIAB = false;
		}
	}

	void OpenIABValidate()
	{
		//if(!DirectoryExist("Plugins/OpenIAB"))
		//{
		//	LogError("* Import OpenIAB package : https://github.com/onepf/OpenIAB/releases");
		//}

		//if(def.OpenIABManagerPrefab != null)
		//{
		//	OpenIABEventManager manager = def.OpenIABManagerPrefab.GetComponent<OpenIABEventManager>();
		//	if(manager == null)
		//	{
		//		LogError("* Manager Prefab doesn't have OpenIABEventManager Component");
		//	}
		//}
	}

	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("#define SERVICE_OPENIAB");
    }
}
