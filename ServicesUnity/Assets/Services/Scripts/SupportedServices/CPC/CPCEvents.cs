#if SERVICE_CPC
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class ServiceEvents
{
    public Func<float> CPC_RewardedAds;
    public Func<float> CPC_Banner;
    public Func<float> CPC_Interstitial;
}
#endif