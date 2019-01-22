using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Services;

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
        if (!def.UseZendesk)
        {
            if (GreenButton("Active Zendesk"))
            {
                def.UseZendesk = true;
                editor.RewriteDefine();
            }
            return;
        }

        def.ZendeskChatKey = EditorGUILayout.TextField("Zendesk Chat Key", def.ZendeskChatKey);

        //end section
        GUILayout.Space(50);
        if (RedButton("Remove Zendesk"))
        {
            def.UseZendesk = false;
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
        if (def.UseZendesk)
        {
            writer.WriteLine("-define:SERVICE_ZENDESK");
        }
    }
}
