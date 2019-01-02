#if SSERVICE_FLURRY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KHD;

namespace Services
{
	public class FlurryService : IService 
	{
		public override IService Init(SettingDef def, ServiceEvents events)
        {
            if (!def.UseFlurry)
            {
                return null;
            }
			base.Init(def, events);

			GameObject obj = new GameObject("FlurryAnalytics", typeof(FlurryAnalytics));
			FlurryAnalytics.Instance.SetDebugLogEnabled(def.FlurryEnableLog);
            FlurryAnalytics.Instance.replicateDataToUnityAnalytics = def.FlurryReplicateDataToUnityAnalytics;

#if UNITY_IOS
            FlurryAnalyticsIOS.SetIAPReportingEnabled(def.FlurryIOSIAPReportingEnabled);
#endif

			FlurryAnalytics.Instance.StartSession(def.FlurryKey_IOS, def.FlurryKey_Android, def.FlurryEnableCrashReport);

			events.TrackEvent += OnTrackEventParam;
			events.TrackEventNoParam += OnTrackEventNoParam;

			return this;
		}

		public override void Dispose()
        {
			serviceE.TrackEvent -= OnTrackEventParam;
			serviceE.TrackEventNoParam -= OnTrackEventNoParam;
		}

		void OnTrackEventNoParam(string eventName)
		{
			#if SV_DEBUG_FLURRY
			Debug.Log("Flurry Track No Param:" + eventName);
			#endif
			FlurryAnalytics.Instance.LogEvent(eventName);
		}

		void OnTrackEventParam(string eventName, TrackingParameter[] trackParams)
		{
			#if SV_DEBUG_FLURRY
			string trackParamsLog = "{";
			for(int i = 0; i < trackParams.Length; ++i)
			{
				trackParamsLog += trackParamsLog + ",";
			}
			trackParamsLog += "}";
			Debug.Log("Flurry Track Param:" + eventName + trackParamsLog);
			#endif
			int len = trackParams.Length;
			Dictionary<string, string> dicParams = new Dictionary<string, string>(len);
			for(int i = 0; i < len; ++i)
			{
				dicParams.Add(trackParams[i].ID, trackParams[i].GetValueAsString());
			}

			FlurryAnalytics.Instance.LogEventWithParameters(eventName, dicParams);
		}
	}
}
#endif