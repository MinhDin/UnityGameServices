#if SERVICE_ZENDESK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public static class ZopimNativeHelper
{
    [DllImport ("__Internal")]
    public static extern float NATIVE_initZopim(string id);

    [DllImport ("__Internal")]
    public static extern float noPreChatFormCPP();
}
#endif