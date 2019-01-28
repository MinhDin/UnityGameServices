#if SERVICE_FIREBASE
#if SERVICE_FIREBASE_REMOTECONFIG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;

namespace Services
{
    public class FirebaseRemoteConfigService : IService
    {
		Dictionary<string, object> defaults;
		FirebaseService root;
		int cachedTime;

		bool isAddedGetRemoteConfigData;

		public FirebaseRemoteConfigService(FirebaseService root)
        {
            this.root = root;
			cachedTime = 600;
			isAddedGetRemoteConfigData = false;
			defaults = new Dictionary<string, object>(50);
        }

		public override IService Init(SettingDef def, ServiceEvents serviceE)
        {
            if (!def.UseFirebase || !def.UseFBRemoteConfig)
            {
                return null;
            }

            base.Init(def, serviceE);
			
			FetchRemoteConfig();

			serviceE.FBSetRemoteConfigDeveloperMode += SetDeveloperMode;
			serviceE.FBAddValueToDefault += AddDefaultValue;
			serviceE.FBAddDictionaryToDefault += AddDefaultDictionary;

			return this;
		}

		public override void Dispose()
        {
            serviceE.FBSetRemoteConfigDeveloperMode -= SetDeveloperMode;
			serviceE.FBAddValueToDefault -= AddDefaultValue;
			serviceE.FBAddDictionaryToDefault -= AddDefaultDictionary;
			serviceE.FBGetRemoteConfigData = null;
        }

		void SetDeveloperMode()
		{
			Firebase.RemoteConfig.ConfigSettings a = new Firebase.RemoteConfig.ConfigSettings();
			a.IsDeveloperMode = true;
			Firebase.RemoteConfig.FirebaseRemoteConfig.Settings = a;
			cachedTime = 0;
		}

		void AddDefaultDictionary(Dictionary<string, object> newDictionary)
		{
			foreach (KeyValuePair<string, object> pair in newDictionary)
			{
				AddOrRenewData(pair.Key, pair.Value);
			}

			Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
			AddGetRemoteConfigData();
		}

		void AddDefaultValue(string key, object value)
		{	
			AddOrRenewData(key, value);
			Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
			AddGetRemoteConfigData();
		}

		void AddOrRenewData(string key, object value)
		{
			object oldValue;
			defaults.Add(key, value);
			if(defaults.TryGetValue(key, out oldValue))
			{
				defaults[key] = value;
			}
			else
			{
				defaults.Add(key, value);
			}
		}

		void FetchRemoteConfig()
		{
			Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(new System.TimeSpan(cachedTime)).ContinueWith(
			result => {
				if (result.IsFaulted)
				{
					#if DEBUG_FIREBASE
					Debug.Log("Fetch Error "+ result.Exception);
					#endif
				}
				else if (result.IsCompleted && !result.IsCanceled)
				{
					if (Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched()) 
					{
						#if DEBUG_FIREBASE
						Debug.Log("Fetch Compeleted! "+ result.Exception);
						#endif
						//if(Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(DefaultRemoteConfig.REMOVE_IAP_FOR_APP_REVIEW).LongValue == 1) {
						//	GameSettings.REMOVE_ADS_IAP = true;
						//} 
						//else 
						//{
						//	GameSettings.REMOVE_ADS_IAP = WillRemoveAdsRemoteConf();
						//}
						//ABHealthCheck();
						//AdsTimeCappedTest();
						PlayerPrefs.SetString(GameSettings.REMOTE_LEVEL_KEY, 
							Firebase.RemoteConfig.FirebaseRemoteConfig
								.GetValue(DefaultRemoteConfig.VERSION_KEY).StringValue);

						AddGetRemoteConfigData();
						//serviceE.FBGetRemoteConfigDataAsDouble += GetRemoteConfigDataAsDouble;
						//serviceE.FBGetRemoteConfigDataAsLong += GetRemoteConfigDataAsLong;
						//serviceE.FBGetRemoteConfigDataAsBool += GetRemoteConfigDataAsBool;

						if (serviceE.OnRemoteConfigFetched != null) 
						{
							serviceE.OnRemoteConfigFetched();
						}
					}
					else
					{
						#if DEBUG_FIREBASE
						Debug.Log("Remote config active failed : "+ result.Exception);
						#endif
					}
				}
			}
		);
		}

		ConfigValue GetRemoteConfigData(string key)
		{
			return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key);
		}

		void AddGetRemoteConfigData()
		{
			if(!isAddedGetRemoteConfigData)
			{
				isAddedGetRemoteConfigData = true;
				serviceE.FBGetRemoteConfigData += GetRemoteConfigData;
			}
		}
/*
		long GetRemoteConfigDataAsLong(string key)
		{
			return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).LongValue;
		}

		bool GetRemoteConfigDataAsBool(string key)
		{
			return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).BooleanValue;
		}

		double GetRemoteConfigDataAsDouble(string key)
		{
			return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).DoubleValue;
		}
 */
    }
}
#endif
#endif