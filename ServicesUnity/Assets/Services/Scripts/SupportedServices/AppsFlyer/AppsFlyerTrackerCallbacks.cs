using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppsFlyerTrackerCallbacks : MonoBehaviour 
{
	public ServiceEvents ServiceE;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	
	public void didReceiveConversionData(string conversionData) 
	{
		if(string.IsNullOrEmpty(conversionData))
		{
			return;
		}
        AppsFlyerTrackerData trackerData = JsonUtility.FromJson<AppsFlyerTrackerData> (conversionData);
        if (trackerData != null) {
            printCallback ("AppsFlyerTrackerCallbacks:: got conversion data, media source = " + trackerData.media_source);
            if (trackerData.af_status != "Organic") {
				//Tracking
                //Firebase.Analytics.FirebaseAnalytics.SetUserProperty ("Mediasource", trackerData.media_source);
				//PlayerPrefs.SetString("Mediasource", trackerData.media_source);
				//PlayerPrefs.SetString("MediasourceDateTime", System.DateTime.Now.ToString());
				if(trackerData.media_source.Trim().ToLower().StartsWith("fb"))
				{
					ServiceE.OnMediaSourceNonOrganic("Facebook Ads");
				}
				else
				{
					ServiceE.OnMediaSourceNonOrganic(trackerData.media_source);
				}
            }
        }
	}
	
	public void didReceiveConversionDataWithError(string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got conversion data error = " + error);
	}
	
	public void didFinishValidateReceipt(string validateResult) {
		printCallback ("AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);
		
	}
	
	public void didFinishValidateReceiptWithError (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);
		
	}
	
	public void onAppOpenAttribution(string validateResult) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);
		
	}
	
	public void onAppOpenAttributionFailure (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);
		
	}
	
	public void onInAppBillingSuccess () {
		printCallback ("AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess");
		
	}
	public void onInAppBillingFailure (string error) {
		printCallback ("AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);
		
	}

	void printCallback(string str) {
        Debug.Log (str);
	}
}

public class AppsFlyerTrackerData
{
    public string af_click_lookback = "";
    public string af_cost_model = "";
    public string af_cost_value = "";
    public string af_status = "";
    public string af_message = "";
    public string campaign = "";
    public string click_time = "";
    public string idfa = "";
    public string install_time = "";
    public string media_source = "";
}