using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ServiceMgr : MonoBehaviour
    {
        List<IService> services;
        public ServiceEvents Events;
        public SettingDef Def;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            services = new List<IService>();

            IService aService;
#if SERVICE_FIREBASE
			//firebase
			aService = new FirebaseService().Init(Def, Events);
			if(aService != null)
			{
				services.Add(aService);
			}
#endif
#if SERVICE_OPENIAB
			//openIAB
			aService = new OpenIABService().Init(Def, Events);
			if(aService != null)
			{
				services.Add(aService);
			}	
#endif
#if SERVICE_FACEBOOK
			//facebook
			aService = new FacebookService().Init(Def, Events);
			if(aService != null)
			{
				services.Add(aService);
			}		
#endif
#if SERVICES_ADMOB
			//admob
			aService = new AdmobService().Init(Def, Events);
			if(aService != null)
			{
				services.Add(aService);
			}	
#endif
#if SERVICE_FLURRY
            //flurry
            aService = new FlurryService().Init(Def, Events);
            if (aService != null)
            {
                services.Add(aService);
            }
#endif
#if SERVICE_APPSFLYER
            //appsflyer
            aService = new AppsFlyerService().Init(Def, Events);
            if (aService != null)
            {
                services.Add(aService);
            }
#endif
            //applovin
            //aService = new ApplovinService().Init(Def, Events);
            //if(aService != null)
            //{
            //	services.Add(aService);
            //}	
#if SERVICE_ZENDESK
            //zendesk
            aService = new ZenDeskService().Init(Def, Events);
            if (aService != null)
            {
                services.Add(aService);
            }
#endif
        }

        public void Dispose()
        {
            int len = services.Count;
            for (int i = 0; i < len; ++i)
            {
                services[i].Dispose();
            }
        }

        private void OnApplicationQuit()
        {
            //Dispose();
        }
    }
}
