using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Services;

public class FirebaseRemoteConfigEditor : ServiceEditor
{
    public FirebaseRemoteConfigEditor(ServiceDef def)
        : base(def)
    {

    }

    public override void OnInspectorGUI(ServiceDefEditor editor)
	{

	}

	public override void OnWriteDefine(StreamWriter writer)
	{
		
	}
	
	public override bool IsValidate()
	{
		return false;
	}

	public override void DownloadPackage(ServiceDefEditor editor)
	{

	}

	public override string GetName()
	{
		return "ERROR NAME";
	}
}
