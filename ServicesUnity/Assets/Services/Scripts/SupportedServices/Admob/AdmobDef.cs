using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ServiceDef : ScriptableObject
{
    public bool UseAdmob;
#if SERVICE_ADMOB
	public string AdmobAppID_IOS;
	public string AdmobAppID_Android;
	public string AdmobBannerID_IOS;
	public string AdmobBannerID_Android;
	public string AdmobInterstitialID_IOS;
	public string AdmobInterstitialID_Android;
	public string AdmobRewardedVideoID_IOS;
	public string AdmobRewardedVideoID_Android;
#endif
}
