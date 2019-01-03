using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ZenDeskServiceEditor : ServiceEditor
{
	public ZenDeskServiceEditor(ServiceDef def)
        : base(def)
    {

    }

	public override string GetName()
    {
        return "ZenDesk";
    }

	public override void OnInspectorGUI(ServiceDefEditor editor)
	{
		if(!def.UseZendesk)
		{
			if(GreenButton("Active Zendesk"))
			{
				def.UseZendesk = true;
			}
			return;
		}

		def.ZendeskChatKey = EditorGUILayout.TextField("Zendesk Chat Key", def.ZendeskChatKey);

		//end section
		GUILayout.Space(50);
		if(RedButton("Remove Zendesk"))
		{
			def.UseZendesk = false;
		}
	}

	public override void OnWriteDefine(StreamWriter writer)
    {
		writer.WriteLine("#define SERVICE_ZENDESK");
    }
}
