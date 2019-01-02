#if SSERVICE_FIREBASE
#if SERVICE_FIREBASE_ANALYTICS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

namespace Services
{
    public class FirebaseAnalyticsService : IService
    {
        FirebaseService root;
        
        public FirebaseAnalyticsService(FirebaseService root)
        {   
            this.root = root;
        }

		public override IService Init(SettingDef def, ServiceEvents gameEvents)
        {
			if(!def.UseFirebase && !def.UseFBAnalytics)
			{
				return null;
			}

			base.Init(def, gameEvents);

			this.serviceE.TrackEvent += TrackEvent;
            this.serviceE.TrackEventNoParam += TrackEventNoParam;            
            this.serviceE.SetUserProperty += SetUserProperty;
            this.serviceE.GetFBParameterValue += GetParameterValue;

			return this;
		}

		public override void Dispose()
		{
			this.serviceE.TrackEvent -= TrackEvent;
            this.serviceE.TrackEventNoParam -= TrackEventNoParam;            
            this.serviceE.SetUserProperty -= SetUserProperty;
            this.serviceE.GetFBParameterValue -= GetParameterValue;
		}

        string GetParameterValue()
        {
            return Firebase.Analytics.FirebaseAnalytics.ParameterValue;
        }

		void TrackEvent(string eventName, TrackingParameter[] trackParam)
        {
            int len = trackParam.Length;
            Parameter[] sendParam = new Parameter[len];
            for(int i = 0; i < len; ++i)
            {
                switch(trackParam[i].Type)
                {
                    case TrackingParameter.TType.INT:
                        sendParam[i] = new Parameter(trackParam[i].ID, trackParam[i].IntValue);
                    break;
                    case TrackingParameter.TType.STRING:
                        sendParam[i] = new Parameter(trackParam[i].ID, trackParam[i].StringValue);
                    break;
                    case TrackingParameter.TType.FLOAT:
                        sendParam[i] = new Parameter(trackParam[i].ID, trackParam[i].FloatValue);
                    break;
                    default:
                        sendParam[i] = new Parameter(trackParam[i].ID, trackParam[i].StringValue);
                    break;
                }
                
            }
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, sendParam);
            
        }

        void TrackEventNoParam(string eventName)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        }
        
        void SetUserProperty(string name, string value)
        {
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(name, value);
        }
    }
}
#endif
#endif