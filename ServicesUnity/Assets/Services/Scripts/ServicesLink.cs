using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    [System.Serializable]
    public class ServicesLink
    {
        public DownloadLink[] Links;
        public string GetDownloadLinkByKey(string key)
        {
            int len = Links.Length;
            for(int i = 0; i < len; ++i)
            {
                if(Links[i].Name.Equals(key))
                {
                    return Links[i].Link;
                }
            }

            return string.Empty;
        }
    }

    [System.Serializable]
    public class DownloadLink
    {
        public string Name;
        public string Link;
    }
}