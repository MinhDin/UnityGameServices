using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using UnityEngine.Networking;

public class ServiceDownloadWindow : EditorWindow
{
    static WebClient webClient;
    static bool inProgress;
    static ServiceDownloadWindow window;
    UnityWebRequest request;
    public static ServiceDownloadWindow ShowWindow()
    {
        if(webClient == null)
        {
            webClient = new WebClient();
            //webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadComplete);
        }
        // Get existing open window or if none, make a new one:
        window = (ServiceDownloadWindow)EditorWindow.GetWindow(typeof(ServiceDownloadWindow), true);
        window.ShowPopup();

        return window;
    }

    public UnityWebRequest Download(string link, string destination)
    {
        if(inProgress)
        {
            return null;
        }

        inProgress = true;

        request = new UnityWebRequest(link);
        request.method = UnityWebRequest.kHttpVerbGET;
        request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.89 Safari/537.36");
        DownloadHandlerFile dh = new DownloadHandlerFile(destination);
        request.downloadHandler = dh;
        request.SendWebRequest();

        return request;
    }

    void OnGUI()
    {
        if(request != null)
        {
            EditorGUILayout.LabelField("Download percent " + (request.downloadProgress * 100).ToString("F2") + '%');
            if(request.isDone)
            {
                request = null;
                window.Close();
            }
        }
    }
}
