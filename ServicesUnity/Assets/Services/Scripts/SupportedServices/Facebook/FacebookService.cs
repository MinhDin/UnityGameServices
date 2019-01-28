#if SERVICE_FACEBOOK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

namespace Services
{
    public class FacebookService : IService
    {
        string userName;
        string userID;

        public override IService Init(SettingDef def, ServiceEvents events)
        {
            if (!def.UseFacebook)
            {
                return null;
            }

            userName = string.Empty;

            base.Init(def, events);

            if (FB.IsInitialized)
            {
                InitSucceeded();
            }
            else
            {
                FB.Init(OnInitCompleted, OnHide);
            }

            serviceE.IsFBLoggedIn += IsLoggedIn;
            return this;
        }

        public override void Dispose()
        {
            serviceE.LoginFacebook = null;
            serviceE.IsFBLoggedIn -= IsLoggedIn;
            if (!string.IsNullOrEmpty(userID))
            {
                serviceE.GetFacebookUserName -= GetUserName;
                serviceE.GetFacebookUserID -= GetUserID;
            }
            if (FB.IsLoggedIn)
            {
                serviceE.AppRequest -= RequestApp;
                serviceE.GetFacebookFriends -= GetFriends;
                serviceE.RequestFacebookIcon -= LoadFriendsIcon;
                serviceE.SendNotificationToFriends -= SendNotificationToFriends;
            }
        }

        //services
        string GetUserName()
        {
            return userName;
        }

        string GetUserID()
        {
            return userID;
        }

        bool IsLoggedIn()
        {
            if (FB.IsInitialized)
            {
                return FB.IsLoggedIn;
            }
            else
            {
                return false;
            }
        }

        //Login
        void Login()
        {
            List<string> perms = new List<string> { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }

        void AuthCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook User ID 1: " + aToken.UserId);

                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }
#endif

                serviceE.AppRequest += RequestApp;
                serviceE.GetFacebookFriends += GetFriends;
                serviceE.RequestFacebookIcon += LoadFriendsIcon;
                serviceE.SendNotificationToFriends += SendNotificationToFriends;

                //Get User Name
                FB.API("me?fields=id,name", Facebook.Unity.HttpMethod.GET, GetUserName);
                
                userID = aToken.UserId;
                serviceE.GetFacebookUserID += GetUserID;
                if (serviceE.OnFacebookLoginSucceeded != null)
                {
                    serviceE.OnFacebookLoginSucceeded(aToken.UserId, aToken.TokenString);
                }
            }
            else
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("User cancelled login");
#endif
            }

        }
        //==========

        //Init
        void InitSucceeded()
        {
#if SV_DEBUG_FACEBOOK
            Debug.Log("Initialized Facebook Succeeded");
#endif
            serviceE.LoginFacebook = Login;

            if (FB.IsLoggedIn)
            {
                FB.Mobile.RefreshCurrentAccessToken();
                FB.ActivateApp();
                AuthCallback(null);
            }
        }

        void GetUserName(Facebook.Unity.IGraphResult result)
        {
#if UNITY_EDITOR
            serviceE.GetFacebookUserName += GetUserName;            
            return;
#endif

            userName = result.ResultDictionary["name"].ToString();

            if (!string.IsNullOrEmpty(userName))
            {
                serviceE.GetFacebookUserName += GetUserName;
                if(serviceE.OnGetFacebookName != null)
                {
                    serviceE.OnGetFacebookName(userName);
                }
            }

#if SV_DEBUG_FACEBOOK
            Debug.Log("Facebook User ID:" + result.ResultDictionary["id"].ToString());
            Debug.Log("Facebook User Name :" + userName);            
#endif
            
        }

        void OnInitCompleted()
        {
            if (FB.IsInitialized)
            {
                InitSucceeded();
            }
            else
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Failed to Initialized Facebook");
#endif
            }
        }

        void OnHide(bool isShowGame)
        {
            if (serviceE.OnFacebookHide != null)
            {
                serviceE.OnFacebookHide(isShowGame);
            }
        }
        //=============

        //Get Friends
        void GetFriends()
        {
            if (FB.IsInitialized && FB.IsLoggedIn)
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook  Start Get Friend");
#endif
                FB.API("/me/friends?fields=id,name", HttpMethod.GET, GetFriendCallback);
            }
        }

        void GetFriendCallback(IGraphResult result)
        {
            if (!string.IsNullOrEmpty(result.Error))
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook  Get Friend Error:" + result.Error);
#endif
            }
            else if (result.Cancelled)
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook  Get Friend Cancelled");
#endif
            }
            else if (string.IsNullOrEmpty(result.RawResult))
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook  Get Friend  Empty Result");
#endif
            }
            else
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook Friends:" + result.RawResult);
#endif

                if (serviceE.OnFriendListSucceeded != null)
                {
                    GetFriendResult rs = JsonUtility.FromJson<GetFriendResult>(result.RawResult);
                    int len = rs.data.Length;
                    AFriend[] friends = new AFriend[len];
                    for (int i = 0; i < len; ++i)
                    {
                        //rs.data[i].ConvertNameToASCII();
                        friends[i] = new AFriend(rs.data[i]);
                    }

#if SV_DEBUG_FACEBOOK
                    Debug.Log("Facebook Friends Send Event");
#endif
                    serviceE.OnFriendListSucceeded(friends);
                }
            }

        }
        //=========

        //Get Friends Picture
        void LoadFriendsIcon(string id, Action<Texture2D> callback)
        {
            if (FB.IsInitialized && FB.IsLoggedIn)
            {
                FB.API(id + "/picture", HttpMethod.GET, (x) =>
                    {
                        if (string.IsNullOrEmpty(x.Error) && x.Texture != null)
                        {
                            callback(x.Texture);
                        }
                    }
                );
            }
        }

        //=========
        void RequestApp()
        {
            if (FB.IsInitialized && FB.IsLoggedIn)
            {
                FB.AppRequest("Would you like to play this game with me ?",
                    null, new List<object>() { "app_non_users" }, null, null, string.Empty, "Invitation", AppRequestCallback);
            }
            else
            {
                //TextLog.Instance.Log("Invite friend service is not available.");
            }
        }

        void AppRequestCallback(IAppRequestResult result)
        {
            if (!string.IsNullOrEmpty(result.Error))
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook App Request Error:" + result.Error);
#endif
            }
            else if (result.Cancelled)
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook App RequestCancelled");
#endif
            }
            else if (string.IsNullOrEmpty(result.RawResult))
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook App Request Empty Result");
#endif
            }
            else
            {
#if SV_DEBUG_FACEBOOK
                Debug.Log("Facebook App Request success:" + result.RawResult);
#endif  
                if (serviceE.OnFacebookAppRequestSuccess != null)
                {
                    FacebookRequestResult rs = JsonUtility.FromJson<FacebookRequestResult>(result.RawResult);
                    serviceE.OnFacebookAppRequestSuccess(rs);
#if SV_DEBUG_FACEBOOK
                    int len = rs.to.Length;
                    for(int i = 0; i < len; ++i)
                    {
                        Debug.Log("ID " + i.ToString() + ":" + rs.to[i]);
                    }
#endif  
                }
            }
        }

        void SendNotificationToFriends(List<string> friendIDs)
        {
            if (FB.IsInitialized && FB.IsLoggedIn)
            {
                FB.AppRequest(userName + " sent you a gift in Word Home.",
                    friendIDs, null, null, null, string.Empty, "Gift", null);

            }
        }
    }
}
#endif