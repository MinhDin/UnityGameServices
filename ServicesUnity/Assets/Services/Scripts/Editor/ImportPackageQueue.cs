using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportPackageQueue
{
    public static ImportPackageQueue Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ImportPackageQueue();
            }

            return _instance;
        }
    }

    static ImportPackageQueue _instance;
    Queue<string> packages;
    bool isImporting;

    public ImportPackageQueue()
    {
        packages = new Queue<string>(10);
        EditorApplication.update += Update;
        AssetDatabase.importPackageCancelled += OnImportEnd;
        AssetDatabase.importPackageCompleted += OnImportEnd;
        AssetDatabase.importPackageFailed += OnImportFailed;
        AssemblyReloadEvents.afterAssemblyReload += OnReload;
    }

    public void ImportPackage(string package)
    {
        packages.Enqueue(package);
    }

    private void Update()
    {
        if(!isImporting && packages.Count > 0)    
        {
            isImporting = true;
            string package = packages.Dequeue();
            AssetDatabase.ImportPackage(package, true);
        }
    }

    void OnReload()
    {
        isImporting = false;
    }
    void OnImportEnd(string packagesName)
    {
        isImporting = false;
    }

    void OnImportFailed(string packageName, string error)
    {
        isImporting = false;
    }
}
