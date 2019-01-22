using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;

public class FacebookServiceEditor : ServiceEditor 
{
	public FacebookServiceEditor(ServiceDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Facebook";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!def.UseFacebook)
		{
			if(GreenButton("Active Facebook"))
			{
				def.UseFacebook = true;
				editor.RewriteDefine();
			}
			return;
		}

		//end section
		GUILayout.Space(50);
		if(RedButton("Remove Facebook"))
		{
			def.UseFacebook = false;
			editor.RewriteDefine();
		}
	}

	public override bool IsValidate()
	{
		return false;
	}

	public override void DownloadPackage(ServiceDefEditor editor)
	{

	}
	
	public override void OnWriteDefine(StreamWriter writer)
    {
		if(def.UseFacebook)
		{
			writer.WriteLine("-define:SERVICE_FACEBOOK");
		}
    }
}
