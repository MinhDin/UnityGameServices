#if SERVICE_CPC
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

#if SERVICE_FIREBASE && SERVICE_FIREBASE_REMOTECONFIG
#define FIREBASE_REMOTE_AVAILABLE
#endif

#if FIREBASE_REMOTE_AVAILABLE
using Firebase.RemoteConfig;
#endif

public class CPCMgr : MonoBehaviour 
{
	public static string[] RowNames =
        {"Country", "Format", "Platform", "CPC (USD)"};
    private static Dictionary<string, string> lookupDict;
    const float DEFAULT_CPC_VALUE = 0.01f;
    const string FAIL_CSV_Service = "parse_csv_cpc";
    const string FAIL_CSV_Error = "csv file contains no semi-colon";
    const string Key_ServiceError = "service_error";
    const string Key_Service = "SERVICE";
    const string Key_Error_Message = "ERROR_MESSAGE";
    const string Key_Url_Part1 = "URL_PART_1";
    const string Key_Url_Part2 = "URL_PART_2";
    const string Key_Http_Error_Code = "HTTP_ERROR_CODE";    
    string CPC_DATA_PATH;
    const string PP_LAST_CPC_DATA_DOWNLOAD_TIME = "LAST_CPC_DATA_DOWNLOAD_TIME";
    const string PP_CPC_VALUE_BANNER_ANDROID = "CPC_VALUE_BANNER_ANDROID";
    const string PP_CPC_VALUE_INTERSTITIAL_ANDROID = "CPC_VALUE_INTERSTITIAL_ANDROID";
    const string PP_CPC_VALUE_REWARDED_VIDEO_ANDROID = "CPC_VALUE_REWARDED_VIDEO_ANDROID";
    const string PP_CPC_VALUE_BANNER_IOS = "CPC_VALUE_BANNER_IOS";
    const string PP_CPC_VALUE_INTERSTITIAL_IOS = "CPC_VALUE_INTERSTITIAL_IOS";
    const string PP_CPC_VALUE_REWARDED_VIDEO_IOS = "CPC_VALUE_REWARDED_VIDEO_IOS";
    public ServiceEvents ServiceE;
    public static float CPC_Banner = DEFAULT_CPC_VALUE;
    public static float CPC_Interstitial = DEFAULT_CPC_VALUE;
    public static float CPC_RewardedAds = DEFAULT_CPC_VALUE;

       public static float PriceStringToFloat(string s, float default_value)
    {
        #if !DISABLE_TRACKING
        float result;
        try
        {
            result = float.Parse(s);
        }
        catch (Exception e)
        {
            Debug.Log("Error parse float: " + e);
            return default_value;
        }
        return result;
        #else
        return 1.0f;
        #endif
    }

    private void Awake()
    {
        CPC_DATA_PATH = Application.persistentDataPath + "/cpc.csv";
#if UNITY_IOS
        CPC_Banner = PlayerPrefs.GetFloat(PP_CPC_VALUE_BANNER_IOS, DEFAULT_CPC_VALUE);
        CPC_Interstitial = PlayerPrefs.GetFloat(PP_CPC_VALUE_INTERSTITIAL_IOS, DEFAULT_CPC_VALUE);
        CPC_RewardedAds = PlayerPrefs.GetFloat(PP_CPC_VALUE_REWARDED_VIDEO_IOS, DEFAULT_CPC_VALUE);
#elif UNITY_ANDROID
        CPC_Banner = PlayerPrefs.GetFloat(PP_CPC_VALUE_BANNER_ANDROID, DEFAULT_CPC_VALUE);
        CPC_Interstitial = PlayerPrefs.GetFloat(PP_CPC_VALUE_INTERSTITIAL_ANDROID, DEFAULT_CPC_VALUE);
        CPC_RewardedAds = PlayerPrefs.GetFloat(PP_CPC_VALUE_REWARDED_VIDEO_ANDROID, DEFAULT_CPC_VALUE);
#endif
    }

    private void Start()
    {    
        if (needUpdateCPCData())
        {
            StartCoroutine(_downloadCPCFile());
        }
    }

	private IEnumerator<float> _downloadCPCFile(){

            //Tracking params
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            int numberOfTry = 3;
            for (int i = 0; i < numberOfTry; i++)
            {
                //Debug.Log(i + " time try");
#if FIREBASE_REMOTE_AVAILABLE
                //Get Remote Config parameter
                ConfigValue cv = FirebaseRemoteConfig.GetValue(def.RemoteConfigLink);
                //Debug.Log("Source : " + cv.Source);
                bool hasConnection = (cv.Source == ValueSource.RemoteValue);
#if !UNITY_EDITOR
                //On success
                if (hasConnection)
                {
                    //Debug.Log("User has connection");
                }
                else
                {
                    //Debug.Log("User has no connection");
                //Fail no connection
                    continue;
                }
#endif
#endif
                #if FIREBASE_REMOTE_AVAILABLE
                string url = cv.StringValue;
                #else
                string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTdlH9VAxcEgctKub30ojfnURR99gdYudZG2qwxYStgDojyf4VPHT14ERVTxu4LpYuwGCl-YmcF3udY/pub?output=csv";
                #endif
                using (UnityWebRequest www = new UnityWebRequest(url))
                {
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.Send();
                    while (!www.isDone)
                    {
                        //Debug.Log("Downloading " + www.isDone);
                        if (www.isDone|| www.isNetworkError || www.isHttpError)
                            break;
                        else
                            yield return MEC.Timing.WaitForSeconds(0.5f);
                    }
                    if (www.isNetworkError || www.isHttpError) {
                        List<TrackingParameter> trackParam = new List<TrackingParameter>();
                        trackParam.Add(new TrackingParameter(Key_Service, "download_cpc"));
                        if(url.Length > 100)
                        {
                            trackParam.Add(new TrackingParameter(Key_Url_Part1, url.Substring(0, 100)));
                            trackParam.Add(new TrackingParameter(Key_Url_Part2, url.Substring(100, url.Length > 200 ? 100 : url.Length -100)));
                        }
                        else
                        {
                            trackParam.Add(new TrackingParameter(Key_Url_Part1, url));                            
                        }
                        if(www.isHttpError)
                        {
                            trackParam.Add(new TrackingParameter(Key_Http_Error_Code, (int)www.responseCode));
                            trackParam.Add(new TrackingParameter(Key_Error_Message, "HTTP error"));
                        }
                        if(www.isNetworkError)
                        {
                            trackParam.Add(new TrackingParameter(Key_Error_Message, "Network error"));
                        }

                        if(ServiceE.TrackEvent != null)
                        {
                            ServiceE.TrackEvent(Key_ServiceError, trackParam.ToArray());
                        }
                        //Debug.Log("Download cpc data file error:" + www.error);
                    }
                    else {
                        //exit loop
                        i = numberOfTry;
                        try {
                            //Debug.Log("Writing file at " + CPC_DATA_PATH);
                            File.WriteAllText(CPC_DATA_PATH, www.downloadHandler.text);
                        }
                        catch (Exception e) {
                            Debug.Log("Error write cpc data file" + e);
                        }
                        PlayerPrefs.SetString(
                            PP_LAST_CPC_DATA_DOWNLOAD_TIME, DateTime.Now.ToString());
                        Debug.Log("Download successfully at " + 
                            PlayerPrefs.GetString(PP_LAST_CPC_DATA_DOWNLOAD_TIME));
                        //Find CPC value
                        string country = Util.NativeHelper.GetCountry();
                        Debug.Log("Country is : " + country);
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            float cpc_banner_android = 
                                GetCPCByCountry(country, "Banner", "Android", www.downloadHandler.text);
                            PlayerPrefs.SetFloat(PP_CPC_VALUE_BANNER_ANDROID,
                                cpc_banner_android);

                            float cpc_interstitial_android = 
                                GetCPCByCountry(country, "Interstitial", "Android", www.downloadHandler.text);
                            PlayerPrefs.SetFloat(PP_CPC_VALUE_INTERSTITIAL_ANDROID,
                                cpc_interstitial_android);
                                
                            float cpc_rewarded_video_android = 
                                GetCPCByCountry(country, "Rewarded video", "Android", www.downloadHandler.text);
                            PlayerPrefs.SetFloat(PP_CPC_VALUE_REWARDED_VIDEO_ANDROID,
                                cpc_rewarded_video_android);
                            
                            CPC_Banner = cpc_banner_android;
                            CPC_Interstitial = cpc_interstitial_android;
                            CPC_RewardedAds = cpc_rewarded_video_android;
                        }
                        if (Application.platform == RuntimePlatform.IPhonePlayer)
                        {
                            float cpc_banner_ios = 
                                GetCPCByCountry(country, "Banner", "iOS", www.downloadHandler.text);
                            PlayerPrefs.SetFloat(PP_CPC_VALUE_BANNER_IOS,
                                cpc_banner_ios);
                            //Debug.Log("CPC Banner ios is : " + cpc_banner_ios);
                            
                            float cpc_interstitial_ios = 
                                GetCPCByCountry(country, "Interstitial", "iOS", www.downloadHandler.text);
                            PlayerPrefs.SetFloat(PP_CPC_VALUE_INTERSTITIAL_IOS,
                                cpc_interstitial_ios);
                            //Debug.Log("CPC Interstitial ios is : " + cpc_interstitial_ios);

                            float cpc_rewarded_video_ios = 
                                GetCPCByCountry(country, "Rewarded video", "iOS");
                            PlayerPrefs.SetFloat(PP_CPC_VALUE_REWARDED_VIDEO_IOS,
                                cpc_rewarded_video_ios);
                            //Debug.Log("CPC rewarded video ios is : " + cpc_rewarded_video_ios);

                            CPC_Banner = cpc_banner_ios;
                            CPC_Interstitial = cpc_interstitial_ios;
                            CPC_RewardedAds = cpc_rewarded_video_ios;
                        }

                    }
                }
            }
        }

    private bool needUpdateCPCData(){
            //Debug.Log(" ---- needUpdateCPCData ----");
            string timeString = PlayerPrefs.GetString(PP_LAST_CPC_DATA_DOWNLOAD_TIME, "");
            //First time
            if (timeString.Equals("")) {
                //Debug.Log("First time");
                return true;
            }
            //Parse error
            DateTime lastTime;
            try {
                lastTime = DateTime.Parse(timeString);
                //Debug.Log("Last time download cpc file:" + lastTime);
            } catch (Exception e) {
                Debug.Log("Can not parse last cpc data download time" + e);
                return true;
            }
            // If Delta time is more than 24 hours - 1day
            //Debug.Log("Delta time: " + DateTime.Now.Subtract(lastTime).TotalHours);
            if (DateTime.Now.Subtract(lastTime).TotalHours > 24f)
                return true;
            //If cpc file is not found
            if (!File.Exists(CPC_DATA_PATH))
                return true;
            return false;
        }

    /*
     * GetCountry()
     * Banner-Intersittial-Rewarded video
     * iOS-Android
    */

	
    public float GetCPCByCountry(string country, string adFormat, string platform, string text = null)
    {
        if (lookupDict == null)
        {
            //Create search dictionary        
            List<Dictionary<string, object>> data;
            if(text == null)
            {
                data = CSVReader.Read(File.ReadAllText(CPC_DATA_PATH));
            }
            else
            {
                data = CSVReader.Read(text);
            }
            //Check if parse success
            if (data.Count < 1)
            {
                //Dictionary<string, object> parameters = new Dictionary<string, object>();
                
                //parameters[FAIL_CSV_Service] = "parse_csv_cpc";
                //parameters[FAIL_CSV_Error] = "csv file contains no semi-colon";

                TrackingParameter[] trackParam = new TrackingParameter[2];
                trackParam[0] = new TrackingParameter(FAIL_CSV_Service, "parse_csv_cpc");
                trackParam[1] = new TrackingParameter(FAIL_CSV_Error, "csv file contains no semi-colon");
                if(ServiceE.TrackEvent != null)
                {
                    ServiceE.TrackEvent(Key_ServiceError, trackParam);
                }
                //TrackEvent(EventTrackingType.service_error, parameters, IgnoreAppflyerSDK);
                return DEFAULT_CPC_VALUE;
            }
            //Check if data has right column name
            foreach (var s in RowNames)
            {
                if (!data[0].ContainsKey(s))
                {
                    //Dictionary<string, object> parameters = new Dictionary<string, object>();
                    //parameters[ParameterTrackingName.SERVICE]
                    //    = "parse_csv_cpc";
                    //parameters[ParameterTrackingName.ERROR_MESSAGE]
                    //    = "csv file does not cotains column : " + s;
                    //TrackEvent(EventTrackingType.service_error, parameters, IgnoreAppflyerSDK);

                    TrackingParameter[] trackParam = new TrackingParameter[2];
                    trackParam[0] = new TrackingParameter(Key_Service, "parse_csv_cpc");
                    trackParam[1] = new TrackingParameter(Key_Error_Message, "csv file does not cotains column : " + s);
                    if(ServiceE.TrackEvent != null)
                    {
                        ServiceE.TrackEvent(Key_ServiceError, trackParam);
                    }
                    return DEFAULT_CPC_VALUE;
                }
            }
            //Debug.Log("---- CSV reader ----");
            //Create lookup dictionary
            lookupDict = new Dictionary<string, string>();
            foreach (var row in data)
            {
                lookupDict[row[RowNames[0]].ToString() +
                    row[RowNames[1]].ToString() + row[RowNames[2]].ToString()]
                    = row[RowNames[3]].ToString();
            }
        }
        string lookupKey = country + adFormat + platform;
        if (lookupDict.ContainsKey(lookupKey))
            return PriceStringToFloat(lookupDict[country + adFormat + platform],
                DEFAULT_CPC_VALUE);
        else
            return DEFAULT_CPC_VALUE;
    }
	 
	
}
#endif