#if SSERVICE_FIREBASE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Analytics;

namespace Services
{
    public class FirebaseService : IService
    {
        #if UNITY_EDITOR
        public static string EDITOR_ID = "Editor ID";
        #endif
        public Firebase.Auth.FirebaseUser User { get { return user; } }
        public Firebase.Auth.FirebaseAuth Auth { get { return auth; } }
        public Firebase.FirebaseApp App { get { return app; } }
        Firebase.Auth.FirebaseUser user;
        Firebase.Auth.FirebaseAuth auth;
        Firebase.FirebaseApp app;

        IService analytics;
        IService realtimeDatabase;
        IService leaderBoard;
        IService competition;

        public override IService Init(SettingDef def, ServiceEvents serviceE)
        {
            if (!def.UseFirebase)
            {
                return null;
            }
            base.Init(def, serviceE);
#if UNITY_ANDROID
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    var dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        // Create and hold a reference to your FirebaseApp, i.e.
                        //   app = Firebase.FirebaseApp.DefaultInstance;
                        // where app is a Firebase.FirebaseApp property of your application class.

                        // Set a flag here indicating that Firebase is ready to use by your
                        // application.
                        
                        InitFirebaseAfterDependencies();
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(System.String.Format(
                          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                        // Firebase Unity SDK is not safe to use here.
                    }
                });
#else
            InitFirebaseAfterDependencies();
#endif

            serviceE.GetFirebaseUserID += GetUserID;
            return this;
        }

        string GetUserID()
        {
            #if UNITY_EDITOR
            return EDITOR_ID;
            #endif
            if(user == null)
            {
                return string.Empty;
            }

            return user.UserId;
        }

        void InitFirebaseAfterDependencies()
        {
            app = Firebase.FirebaseApp.DefaultInstance;
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            //sign in anonymous
            #if UNITY_EDITOR
            #else
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    if (serviceE.OnFBAnonymouslyLoginComplete != null)
                    {
                        serviceE.OnFBAnonymouslyLoginComplete(string.Empty);
                    }
                    return;
                }
                if (task.IsFaulted)
                {
                    if (serviceE.OnFBAnonymouslyLoginComplete != null)
                    {
                        serviceE.OnFBAnonymouslyLoginComplete(string.Empty);
                    }
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                if (serviceE.OnFBAnonymouslyLoginComplete != null)
                {
                    serviceE.OnFBAnonymouslyLoginComplete(newUser.UserId);
                }

                user = newUser;
#if SV_DEBUG_FIREBASE
                Debug.Log(string.Format("FB User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId));
#endif
            });
            #endif
            analytics = new FirebaseAnalyticsService(this).Init(def, serviceE);
            realtimeDatabase = new FirebaseRealtimeDatabaseService(this).Init(def, serviceE);
            leaderBoard = new FirebaseLeaderboardService(this).Init(def, serviceE);
            competition = new FirebaseCompetitionService(this).Init(def, serviceE);
        }

        public override void Dispose()
        {
            if (analytics != null)
            {
                analytics.Dispose();
            }
            if (realtimeDatabase != null)
            {
                realtimeDatabase.Dispose();
            }
            if (leaderBoard != null)
            {
                leaderBoard.Dispose();
            }

            serviceE.GetFirebaseUserID -= GetUserID;
        }


    }
}
#endif