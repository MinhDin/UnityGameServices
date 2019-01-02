#if SERVICE_ZENDESK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class ZenDeskService : IService
    {
        public override IService Init(SettingDef def, ServiceEvents gameEvents)
		{
			if(!def.UseZendesk)
			{
				return null;
			}
			
			base.Init(def, gameEvents);
			

			ZopimNativeHelper.NATIVE_initZopim(this.def.ZendeskChatKey);

			this.serviceE.OpenLiveChat += OpenLiveChat;
			return this;
		}

		public override void Dispose()
		{
			this.serviceE.OpenLiveChat -= OpenLiveChat;
		}

		void OpenLiveChat()
		{
			ZopimNativeHelper.noPreChatFormCPP();
		}
    }
}
#endif