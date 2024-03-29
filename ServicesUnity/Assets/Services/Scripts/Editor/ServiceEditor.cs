﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Services;
using ICSharpCode.SharpZipLib.Zip;
using System;
using UnityEngine.Networking;
using System.Linq;

public class ServiceEditor
{
    public List<string> PackagesName { get { return packagesName; } }
    protected List<string> packagesName;

    protected ServiceDef def;
    protected static FontStyle baseFontStyle;

    protected static Color baseBackgroundColor;
    protected static Color baseLabelColor;
    protected static string error;
    protected static GUIStyle gsAlterQuest;

    protected UnityWebRequest request;
    protected string packagePath = string.Empty;

    static ServiceEditor()
    {
        baseBackgroundColor = GUI.backgroundColor;
        baseFontStyle = EditorStyles.label.fontStyle;
        baseLabelColor = EditorStyles.label.normal.textColor;

        gsAlterQuest = new GUIStyle();
        gsAlterQuest.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
    }

    public ServiceEditor(ServiceDef def)
    {
        this.def = def;
    }

    public virtual void OnEnable()
    {
        RefreshPackagesName();
    }

    void RefreshPackagesName()
    {
        packagesName = null;

        if (DirectoryExist(packagePath))
        {
            packagesName = Directory.GetFiles(Application.dataPath + "/" + packagePath, "*.unitypackage", SearchOption.AllDirectories)
                                .Select(x => Path.GetFileName(x)).ToList();
        }
        else if (FileExist(packagePath + ".unitypackage"))
        {
            packagesName = new List<string>() { packagePath + ".unitypackage" };
        }
    }
    public virtual void OnInspectorGUI(ServiceDefEditor editor)
    {

    }

    public virtual void OnWriteDefine(StreamWriter writer)
    {

    }

    public virtual bool IsValidate()
    {
        return false;
    }

    public virtual void ImportPackage(string filter)
    {
        if(PackagesName == null)
        {
            Debug.Log("Import Failed : package name null.");
            return;
        }

        if(string.IsNullOrEmpty(packagePath))
        {
            Debug.Log("Import Failed : package path empty");
            return;
        }
        foreach (string file in PackagesName)
        {
            if (file.Contains("unitypackage") && file.Contains(filter))
            {
                string path = "Assets/" + packagePath + "/" + file;
                //AssetDatabase.ImportPackage(packagePath, true);
                ImportPackageQueue.Instance.ImportPackage(path);
                return;//success
            }
        }

        return;//fail
    }

    protected bool BasicInspectorHeader(ServiceDefEditor editor, string importFilter, ref bool activeParam)
    {
        string name = GetName();

        if (!IsValidate())
        {
            if (activeParam)
            {
                activeParam = false;
                editor.RewriteDefine();
            }

            if((packagesName != null) && (packagesName.Count > 0))
            {
                if(GreenButton(
                    string.Format("Import {0} Package", name)))
                    {
                        ImportPackage(importFilter);
                    }
            }
            if (GreenButton("Get Facebook Package"))
            {
                activeParam = false;
                DownloadPackage(editor);
            }
            return false;
        }

        if (!activeParam)
        {
            if (GreenButton("Active Facebook"))
            {
                activeParam = true;
                editor.RewriteDefine();
            }
            return false;
        }

        return true;
    }

    protected void BasicInspectorFooter(ServiceDefEditor editor, ref bool activeParam)
    {
        //end section
        GUILayout.Space(50);
        if (RedButton("Remove " + GetName()))
        {
            activeParam = false;
            editor.RewriteDefine();
        }
    }

    public virtual void DownloadPackage(ServiceDefEditor editor)
    {
        if (string.IsNullOrEmpty(packagePath))
        {
            Debug.LogError("Package Path is null");
            return;
        }

        RefreshPackagesName();
        if ((packagesName == null) || (packagesName.Count == 0))
        {
            if (FileExist(packagePath + ".zip"))
            {
                Debug.Log("DECOMPRESS " + packagePath + ".zip");
                string destination = Application.dataPath + "/" + packagePath + ".zip";
                packagesName = DecompressSharpZip(destination, Application.dataPath + "/" + packagePath);
            }
            else
            {
                string link = editor.GetDownloadLinkByKey(packagePath);
                if (!string.IsNullOrEmpty(link))
                {
                    Debug.Log("DOWNLOAD " + packagePath + link);
                    string destination = Application.dataPath + "/" + packagePath + ".zip";
                    Debug.Log("DOWNLOAD to:" + destination);

                    ServiceDownloadWindow downloadWindow = ServiceDownloadWindow.ShowWindow();
                    request = downloadWindow.Download(link, destination);
                }
            }
        }
    }

    public virtual string GetName()
    {
        return "ERROR NAME";
    }

    protected void RegisterUpdate()
    {
        EditorApplication.update += Update;
    }

    protected virtual void Update()
    {
        if ((request != null) && (request.isDone))
        {
            request = null;
            if (DirectoryExist(packagePath))
            {
                if ((packagesName == null) || (packagesName.Count == 0))
                {
                    packagesName = Directory.GetFiles(Application.dataPath + "/" + packagePath, "*.unitypackage", SearchOption.AllDirectories)
                                        .Select(x => Path.GetFileName(x)).ToList();
                }
                return;
            }
            else if (FileExist(packagePath + ".unitypackage"))
            {
                if ((packagesName == null) || (packagesName.Count == 0))
                {
                    packagesName = new List<string>() { packagePath + ".unitypackage" };
                }
            }
            else if (FileExist(packagePath + ".zip"))
            {
                Debug.Log("DECOMPRESS " + packagePath + ".zip");
                string destination = Application.dataPath + "/" + packagePath + ".zip";
                packagesName = DecompressSharpZip(destination, Application.dataPath + "/" + packagePath);
                return;
            }

        }
    }

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
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    protected List<string> DecompressSharpZip(string path, string folderName)
    {
        List<string> rs = new List<string>(10);
        using (ZipInputStream s = new ZipInputStream(File.OpenRead(path)))
        {

            ZipEntry theEntry;

            while ((theEntry = s.GetNextEntry()) != null)
            {

                string directoryName = folderName;//Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);

                // create directory
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                if (fileName != String.Empty)
                {
                    using (FileStream streamWriter = File.Create(directoryName + "/" + fileName))
                    {

                        rs.Add(fileName);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        return rs;
    }
    #endregion
}