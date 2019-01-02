#if REMOVE
#if SERVICE_FIREBASE
#if SERVICE_FIREBASE_ANALYTICS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;

namespace Services
{

    public class FirebaseCompetitionService : IService
    {
        //const string KEY_SAVE_NAME = "Key_Save_UserName";

        Firebase.Auth.FirebaseAuth auth;
        DatabaseReference databaseRef;

        int score;
        string weekID;

        FirebaseService root;

        //string userName;

        bool requestYourCompetitiveScore;
        bool requestTopCompetitiveScore;
        string weekIDQuery;
        List<CompetitiveTopResult> competitiveRs;
        CompetitiveTopResult yourRs;
        bool isQueryCompetitiveFailed;

        public FirebaseCompetitionService(FirebaseService root)
        {
            this.root = root;
            //userName = PlayerPrefs.GetString(KEY_SAVE_NAME, string.Empty);
        }

        public override IService Init(SettingDef def, ServiceEvents serviceE)
        {
            if (!def.UseFirebase || !def.UseFBRealtimeDatabase)
            {
                return null;
            }

            base.Init(def, serviceE);
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            //serviceE.RequestCompetitiveTop += RequestCompetitiveTop;
           // serviceE.RequestSyncCompetitionScore += SyncCompetitionScore;
            //serviceE.RequestCompetitionScore += GetScoreCompetition;
            //serviceE.GetCompetitionName += GetCompetitionName;
            //serviceE.RequestSetCompetitionName += RequestSetCompetitionName;

            return this;
        }

        public override void Dispose()
        {
            //serviceE.RequestCompetitiveTop -= RequestCompetitiveTop;
            //serviceE.RequestSyncCompetitionScore -= SyncCompetitionScore;
            //serviceE.RequestCompetitionScore -= GetScoreCompetition;
            //serviceE.GetCompetitionName -= GetCompetitionName;
            //serviceE.RequestSetCompetitionName -= RequestSetCompetitionName;
        }

        //string GetCompetitionName()
        //{
        //    return userName;
        //}

        void RequestSetCompetitionName(string weekID, string name)
        {
            //if(userName != name)
            {
                UpdateCompetitionName(weekID, name);
            }
        }

        void RequestCompetitiveTop(string weekID, int amount)
        {
            weekIDQuery = weekID;
            //requestTopCompetitiveScore = false;
            //requestYourCompetitiveScore = false;
            //isQueryCompetitiveFailed = false;
            /*
                        databaseRef.Child("Competition").Child(weekID).Child(root.User.UserId).GetValueAsync().ContinueWith(task =>
                        {
                            if (task.IsCompleted && task.Result != null)
                            {
                                int score = 0;
                                int.TryParse(task.Result.Child("Score").Value.ToString(), out score);
                                yourRs = new CompetitiveTopResult(task.Result.Key, task.Result.Child("Name").Value.ToString(), score);

                                requestYourCompetitiveScore = true;

                                Refresh2RowQueryCompetitive();
            #if DEBUG_FIREBASE
                                Debug.Log("RequestCompetitive User Score:" + );
            #endif
                            }
                            else
                            {
            #if DEBUG_FIREBASE
                                Debug.Log("RequestCompetitive User Failed");
            #endif
                            }
                        });
             */
#if DEBUG_FIREBASE
            Debug.Log("RequestCompetitiveTop");
#endif
            databaseRef.Child("Competition").Child(weekID).OrderByChild("Score").LimitToLast(amount).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && task.Result != null)
                {
                    int count = (int)task.Result.ChildrenCount;
#if DEBUG_FIREBASE
                    Debug.Log("RequestCompetitiveTop Success :" + count.ToString());
#endif
                    if (serviceE.OnGetCompetitiveTopResult != null)
                    {
                        competitiveRs = new List<CompetitiveTopResult>(count);

                        foreach (DataSnapshot row in task.Result.Children)
                        {
                            if (row == null)
                            {
#if DEBUG_FIREBASE
                                Debug.Log("RequestCompetitiveTop Row NULL");
#endif
                                continue;
                            }

                            int score;
                            if (row.HasChild("Score"))
                            {
                                if (!int.TryParse(row.Child("Score").Value.ToString(), out score))
                                {
                                    score = 0;
                                }
                            }
                            else
                            {
                                continue;
                            }
                            string name;
                            if(row.HasChild("Name"))
                            {
                                name = row.Child("Name").Value.ToString();
                            }
                            else
                            {
                                name = "Anonymous";
                            }
                            competitiveRs.Add(new CompetitiveTopResult(row.Key, name, score));
#if DEBUG_FIREBASE
                            Debug.Log("RequestCompetitiveTop Item :" + name + ":" + score.ToString());
#endif
                        }
                    }

                    //requestTopCompetitiveScore = true;

                    if (serviceE.OnGetCompetitiveTopResult != null)
                    {
                        competitiveRs.Reverse();
#if DEBUG_FIREBASE
                        Debug.Log("RequestCompetitiveTop Send Event :");
#endif
                        serviceE.OnGetCompetitiveTopResult(weekID, competitiveRs);
                    }
                    //Refresh2RowQueryCompetitive();

                }
                else
                {

#if DEBUG_FIREBASE
                    Debug.Log("RequestCompetitiveTop Failed ");
#endif
                    //requestTopCompetitiveScore = true;

                    //Refresh2RowQueryCompetitive();
                }
            });
        }
        /*
                void Refresh2RowQueryCompetitive()
                {
                    if (!requestYourCompetitiveScore)
                    {
                        return;
                    }

                    if (!requestTopCompetitiveScore)
                    {
                        return;
                    }

                    if (isQueryCompetitiveFailed)
                    {
                        return;
                    }

                    if (serviceE.OnGetCompetitiveTopResult != null)
                    {
                        serviceE.OnGetCompetitiveTopResult(weekID, competitiveRs, yourRs);
                    }
                }
         */
        void SyncCompetitionScore(string weekID, int score)
        {
#if DEBUG_FIREBASE
            Debug.Log("SyncCompetitionScore");
            if (databaseRef == null)
            {
                Debug.Log("databaseRef == null");
            }
            if (root == null)
            {
                Debug.Log("root == null");
            }
            if (root.User == null)
            {
                Debug.Log("root.User == null");
            }
#endif

#if UNITY_EDITOR
            databaseRef.Child("Competition").Child(weekID).Child(FirebaseService.EDITOR_ID).Child("Score").SetValueAsync(score);
            return;
#endif
            if (root.User == null)
            {
                return;
            }
            if (serviceE.GetFacebookUserID != null)
            {
                string facebookID = serviceE.GetFacebookUserID();

                databaseRef.Child("Competition").Child(weekID).Child(root.User.UserId).Child("Score").SetValueAsync(score);
                databaseRef.Child("Users").Child(facebookID).Child("Competition").Child(weekID).SetValueAsync(score);
            }
            else
            {
                databaseRef.Child("Competition").Child(weekID).Child(root.User.UserId).Child("Score").SetValueAsync(score);
            }

            /*
            if (serviceE.GetFacebookUserName != null)
            {
                string facebookName = serviceE.GetFacebookUserName();
                if (userName != facebookName)
                {
                    UpdateCompetitionName(facebookName);
                }
            }
            else
            {
                UpdateCompetitionName(GetRandomName());
            }
             */
        }

        void GetScoreCompetition(string weekID, string facebookID)
        {
            if(string.IsNullOrEmpty(weekID))
            {
                return;
            }
#if DEBUG_FIREBASE
            Debug.Log("GetScoreCompetitive Get User Score :" + facebookID);
#endif
            databaseRef.Child("Users").Child(facebookID).Child("Competition").Child(weekID).GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted && task.Result != null)
                    {
#if DEBUG_FIREBASE
                        Debug.Log("GetScoreCompetitive Get User Score Success:" + facebookID + ". Score:" + task.Result.Value.ToString());
#endif
                        if (serviceE.OnGetCompetitiveScore != null)
                        {
                            int score = 0;
                            if (int.TryParse(task.Result.Value.ToString(), out score))
                            {
                                serviceE.OnGetCompetitiveScore(facebookID, score);
                            }
                            else
                            {
                                serviceE.OnGetCompetitiveScore(facebookID, 0);
                            }
                        }
                    }
                    else
                    {
#if DEBUG_FIREBASE
                        Debug.Log("GetScoreCompetitive Get User Score Failed:" + facebookID);
#endif
                        if (serviceE.OnGetCompetitiveScore != null)
                        {
                            serviceE.OnGetCompetitiveScore(facebookID, 0);
                        }
                    }
                });
        }

        void UpdateCompetitionName(string weekID, string name)
        {
            if(string.IsNullOrEmpty(this.weekID))
            {
                return;
            }
#if UNITY_EDITOR
            databaseRef.Child("Competition").Child(weekID).Child("Editor ID").Child("Name").SetValueAsync(name);
            return;
#endif
#if DEBUG_FIREBASE
            Debug.Log(string.Format("FB UpdateCompetitionName:{0}:{1}", weekID, name));
#endif
            //userName = name;
            databaseRef.Child("Competition").Child(weekID).Child(root.User.UserId).Child("Name").SetValueAsync(name);
            //PlayerPrefs.SetString(KEY_SAVE_NAME, userName);
#if DEBUG_FIREBASE
            Debug.Log(string.Format("FB UpdateCompetitionName Compeleted"));
#endif
        }
    }

    public struct CompetitiveTopResult
    {
        public string Name;
        public int Score;
        public string CompetitiveID;

        public CompetitiveTopResult(string competitiveID, string name, int score)
        {
            CompetitiveID = competitiveID;
            Name = name;
            Score = score;
        }
    }
}
#endif
#endif
#endif