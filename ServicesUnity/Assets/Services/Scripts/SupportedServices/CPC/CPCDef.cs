using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public partial class ServiceDef : ScriptableObject
    {
        public bool UseCPC;
        #if CPC_SERVICE
        public string CPCRemoteConfigLink;
        #endif
    }
}
