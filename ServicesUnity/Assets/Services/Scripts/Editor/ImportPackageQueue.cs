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
        Debug.Log("Import " + package);
        packages.Enqueue(package);
    }

    private void Update()
    {
        if(!isImporting && packages.Count > 0)    
        {
            isImporting = true;
            string package = packages.Dequeue();
            Debug.Log("Import From Queue " + package);
            AssetDatabase.ImportPackage(package, true);
        }
    }

    void OnReload()
    {
        Debug.Log("Reload script");
        isImporting = false;
    }
    void OnImportEnd(string packagesName)
    {
        Debug.Log("Import End");
        isImporting = false;
    }

    void OnImportFailed(string packageName, string error)
    {
        Debug.Log("Import FAILED");
        isImporting = false;
    }
}
