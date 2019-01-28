using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Services;
using System.IO;
using UnityEditor;

public class CPCServiceEditor : ServiceEditor
{
    public CPCServiceEditor(ServiceDef def)
        : base(def)
    {

    }

    public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!def.UseCPC)
		{
			if(GreenButton("Active CPC"))
			{
				def.UseCPC = true;
				editor.RewriteDefine();
			}
			return;
		}

		#if CPC_SERVICE
        EditorGUILayout.TextField("Remote Config", def.CPCRemoteConfigLink);
		#endif

		//end section
		GUILayout.Space(50);
		if(RedButton("Remove AppFlyer"))
		{
			def.UseCPC = false;
			editor.RewriteDefine();
		}
	}

	public override void OnWriteDefine(StreamWriter writer)
	{
		if(def.UseCPC)
		{
			writer.WriteLine("-define:SERVICE_CPC");
		}
	}
	
	public override bool IsValidate()
	{
		return true;
	}
	
	public override string GetName()
	{
		return "CPC";
	}
}
