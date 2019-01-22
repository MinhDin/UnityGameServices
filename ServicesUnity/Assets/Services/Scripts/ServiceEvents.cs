using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Services
{


    public partial class ServiceEvents : ScriptableObject
    {
        //Common Game Events
        //public Action<int> OnTutorialStartStep;
        //public Action<int> OnTutorialFinishStep;
        public Action<int> OnStartSession;
        public Action<int> OnLevelEnd;//careful
        public Action<bool> EnableSound;
        //public Action<string, int> OnGetUserScore;//facebook ID, score

        //public Action<List<AGift>> OnGetGiftsFromFriends;

        //Services
        public Action RestorePurchase;
        public Action<string> PurchasePackage;
        public Action ShowLoading;
        public Action HideLoading;
        public Func<List<IABPackage>> GetPackages;
        public Action OnOpenIABNotInitialized;
        public Action OnOpenIABInitialized;
        public Action OnNoInternetConnection;
        public Action LoginFacebook;
        public Action AppRequest;
        public Action GetFacebookFriends;
        public Action<List<string>> SendNotificationToFriends;
        public Action<string, Action<Texture2D>> RequestFacebookIcon;
        public Action<string, TrackingParameter[]> TrackEvent;
        public Action<string> TrackEventNoParam;
        public Action RequestBanner;//successfull request banner is auto show.
        public Action StopShowBanner;
        public Action RequestInterstitial;
        public Func<bool> ShowInterstitial;
        public Action RequestRewardedVideo;
        public Func<bool> ShowRewardedVideo;
        public Action<string, string> SetUserProperty;
        public Action OpenLiveChat;
        public Func<string> GetFacebookUserName;
        public Func<string> GetFacebookUserID;
        public Func<bool> IsFBLoggedIn;
        public Func<string> GetFirebaseUserID;

        //Open IAB Callback
        public Action<List<string>> OnRestoreIABSucceeded;
        public Action OnRestoreIABFailed;
        public Action<string> OnPurchaseSucceeded;
        public Action<string> OnPurchaseFailed;

        //Facebook Callback
        public Action<string, string> OnFacebookLoginSucceeded;//userID, token String
        public Action<bool> OnFacebookHide;
        public Action<FacebookRequestResult> OnFacebookAppRequestSuccess;
        public Action<AFriend[]> OnFriendListSucceeded;
        public Action<string> OnGetFacebookName;

        //admob Callback   
        public Action<string> OnBannerLoaded;
        public Action OnBannerFailedToLoad;
        public Action<string> OnBannerOpened;
        public Action<string> OnBannerClosed;
        public Action<string> OnBannerLeftApplication;

        public Func<bool> IsInterstitialAvailable;
        public Action<string> OnInterstitialLoaded;
        public Action OnInterstitialFailedToLoad;
        public Action<string> OnInterstitialOpened;
        public Action<string> OnInterstitialClosed;
        public Action<string> OnInterstitialLeftApplication;

        public Func<bool> IsRewardVideoAvailable;
        public Action<string> OnRewardVideoLoaded;
        public Action OnRewardVideoFailedToLoad;
        public Action<string> OnRewardVideoOpened;
        public Action<string> OnRewardVideoStarted;
        public Action<string, string, double> OnRewardVideoRewarded;//ad network, type, amount
        public Action<string> OnRewardVideoClosed;
        public Action<string> OnRewardVideoLeftApplication;

        //AppsFlyer Callback
        public Action<string> OnMediaSourceNonOrganic;

        //Firebase Callback
        public Action<string> OnFBAnonymouslyLoginComplete;//userID
        public Action<string> OnFBConnectToDatabaseWithFBKCredential;//FacebookID   
        public Action OnRemoteConfigFetched;
        public Func<string> GetFBParameterValue;

        //Data

#if UNITY_EDITOR
        [MenuItem("Scriptable/Services/ServiceEvents")]
        public static void CreateAsset()
        {
            ServiceEvents asset = ScriptableObject.CreateInstance<ServiceEvents>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ServiceEvents.asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }

    public struct TrackingParameter
    {
        public enum TType
        {
            INT,
            STRING,
            FLOAT,
        }
        public string ID;
        public string StringValue;
        public int IntValue;
        public float FloatValue;
        public TType Type;

        public string GetValueAsString()
        {
            if (Type == TType.INT)
            {
                return IntValue.ToString();
            }
            else if (Type == TType.STRING)
            {
                return StringValue;
            }
            else if (Type == TType.FLOAT)
            {
                return FloatValue.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public TrackingParameter(string id, string value)
        {
            this.ID = id;
            StringValue = value;
            IntValue = 0;
            FloatValue = 0;
            Type = TType.STRING;
        }

        public TrackingParameter(string id, int value)
        {
            this.ID = id;
            IntValue = value;
            StringValue = string.Empty;
            FloatValue = 0;
            Type = TType.INT;
        }

        public TrackingParameter(string id, float value)
        {
            this.ID = id;
            IntValue = 0;
            StringValue = string.Empty;
            FloatValue = value;
            Type = TType.FLOAT;
        }
    }

    public class AFriend
    {
        [Flags]
        public enum TagChange
        {
            Level = 1,
            Icon = 2,
            Name = 4,
            CompetitionScore = 8,
            //CompetitionName = 16,
            All = Level | Icon | Name | CompetitionScore,// | CompetitionName,
        }

        public string ID;
        public string Name { get { return name; } }
        public int Level { get { return level; } }
        public int CompetitionScore { get { return competitionScore; } }
        //public string CompetitionName{get{return competitionName;}}
        public Texture2D Icon { get { return icon; } }
        Texture2D icon;
        int level;
        string name;
        int competitionScore;
        //string competitionName;
        public Action<AFriend, TagChange> OnValueChange;

        public AFriend(FriendStructParse parse)
        {
            ID = parse.id;
            name = parse.name;
            level = 0;
            icon = null;
        }

        public void SetName(string name)
        {
            if (!this.name.Equals(name))
            {
                this.name = name;
                NoticeValueChange(TagChange.Name);
            }
        }

        public void SetIcon(Texture2D tex)
        {
            if (icon != tex)
            {
                icon = tex;
                NoticeValueChange(TagChange.Icon);
            }
        }

        public void SetLevel(int level)
        {
            if (this.level != level)
            {
                this.level = level;
                NoticeValueChange(TagChange.Level);
            }
        }

        public void SetCompetitionScore(int score)
        {
            if (this.competitionScore != score)
            {
                this.competitionScore = score;
                NoticeValueChange(TagChange.CompetitionScore);
            }
        }

        /*
        public void SetCompetitionName(string name)
        {
            if(this.competitionName != name)
            {
                this.competitionName = name;
                NoticeValueChange(TagChange.CompetitionName);
            }
        }
         */

        public void NoticeValueChange(TagChange tag)
        {
            if (OnValueChange != null)
            {
                OnValueChange(this, tag);
            }
        }
    }

    [System.Serializable]
    public struct FriendStructParse
    {
        public string name;
        public string id;

        public FriendStructParse(string name, string id)
        {
            this.name = name;
            this.id = id;
        }
        public void ConvertNameToASCII()
        {
            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            System.Text.Encoding unicode = System.Text.Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(name);
            byte[] asciiBytes = System.Text.Encoding.Convert(unicode, ascii, unicodeBytes);

            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);

            name = new string(asciiChars);
        }
    }

    [System.Serializable]
    public class GetFriendResult
    {
        public FriendStructParse[] data;
    }

    public class AGift
    {
        public string FromID;

        public AGift(string fromID)
        {
            FromID = fromID;
        }
    }

    [System.Serializable]
    public class FacebookRequestResult
    {
        public string request;
        public string[] to;
    }
}