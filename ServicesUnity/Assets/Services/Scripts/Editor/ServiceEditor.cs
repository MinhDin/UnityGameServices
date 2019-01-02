using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ServiceEditor
{
	protected SettingDef def;
	protected static FontStyle baseFontStyle;

	protected static Color baseBackgroundColor;
	protected static Color baseLabelColor;
	protected static string error;
	protected static GUIStyle gsAlterQuest;
	
	static ServiceEditor()
	{
		baseBackgroundColor = GUI.backgroundColor;
		baseFontStyle = EditorStyles.label.fontStyle;
		baseLabelColor = EditorStyles.label.normal.textColor;

		gsAlterQuest = new GUIStyle();
		gsAlterQuest.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
	}

	public ServiceEditor(SettingDef def)
	{
		this.def = def;
	}

	public virtual void OnInspectorGUI()
	{

	}

	public virtual void OnWriteDefine(StreamWriter writer)
	{

	}

	public virtual string GetName()
	{
		return "ERROR NAME";
	}
	//public abstract void RegisterThis();

	#region Helper
	protected static bool BoldToggle(string label, bool data)
	{
		EditorStyles.label.fontStyle = FontStyle.Bold;	
		bool rs = EditorGUILayout.Toggle(label, data, EditorStyles.toggle);
		EditorStyles.label.fontStyle = baseFontStyle;

		return rs;
	}

	protected static void RedLabel(string label)
	{
		EditorStyles.label.normal.textColor = Color.red;
		EditorGUILayout.LabelField(label);
		EditorStyles.label.normal.textColor = baseLabelColor;
	}

	protected static bool FileExist(string pathFromAsset)
	{
		return File.Exists(Application.dataPath + "/" + pathFromAsset);
	}

	protected static bool DirectoryExist(string pathFromAsset)
	{
		return Directory.Exists(Application.dataPath + "/" + pathFromAsset);
	}

	protected static bool RedButton(string label, params GUILayoutOption[] options)
	{
		GUI.backgroundColor = Color.red;
		bool rs = GUILayout.Button(label, options);
		GUI.backgroundColor = baseBackgroundColor;

		return rs;
	}

	protected static bool GreenButton(string label, params GUILayoutOption[] options)
	{
		GUI.backgroundColor = Color.green;
		bool rs = GUILayout.Button(label, options);
		GUI.backgroundColor = baseBackgroundColor;

		return rs;
	}

	protected void LogError(string log)
	{
		error += log + "\n";
	}
	
	protected void ClearLog()
	{
		error = string.Empty;
	}

	protected static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];
 
        for(int i = 0; i < pix.Length; i++)
            pix[i] = col;
 
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
 
        return result;
    }
	#endregion
}