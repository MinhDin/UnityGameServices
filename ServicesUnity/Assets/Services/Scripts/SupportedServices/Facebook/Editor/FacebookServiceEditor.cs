using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FacebookServiceEditor : ServiceEditor 
{
	public FacebookServiceEditor(SettingDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "Facebook";
    }

	public override void OnInspectorGUI()
	{
		if(!def.UseFacebook)
		{
			if(GreenButton("Active Facebook"))
			{
				def.UseFacebook = true;
			}
			return;
		}

		//end section
		GUILayout.Space(50);
		if(RedButton("Remove Facebook"))
		{
			def.UseFacebook = false;
		}
	}

	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("#define SERVICE_FACEBOOK");
    }
}
