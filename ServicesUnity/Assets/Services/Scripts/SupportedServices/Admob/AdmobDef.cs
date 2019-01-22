using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
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

	[System.Serializable]
    public class IABPackage
    {
        public string Name;
        public string SKU;
        public int Coin;
        public string TextureKey;
        public string USDPrice;
        public string Bonus;
        public float ScaleImage;
        //public bool WillRemoveAds;
        public IABPackageFlag Flags;
    }

    [System.Flags]
    public enum IABPackageFlag
    {
        SHOW_IN_SHOP = 1,
        PROMOTE = 2,
        THEME = 4,
        REMOVE_ADS = 8,
        CONSUMABLE = 16,
    }
}