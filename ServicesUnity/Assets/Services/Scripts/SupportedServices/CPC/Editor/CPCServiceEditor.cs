using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Services;
using System.IO;

public class CPCServiceEditor : ServiceEditor
{
    public CPCServiceEditor(ServiceDef def)
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
		return "CPC";
	}
}
