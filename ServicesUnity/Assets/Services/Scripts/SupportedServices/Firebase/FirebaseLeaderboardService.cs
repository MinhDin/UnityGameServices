#if REMOVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

namespace Services
{
    public class FirebaseLeaderboardService : IService
    {
        DatabaseReference databaseRef;
        string facebookID;

        FirebaseService root;

        public FirebaseLeaderboardService(FirebaseService root)
        {
            this.root = root;
        }


        public override IService Init(SettingDef def, ServiceEvents gameEvents)
        {
            if (!def.UseFacebook || !def.UseFirebase || !def.UseFBRealtimeDatabase || !def.UseFBLeaderBoard)
            {
                return null;
            }

            base.Init(def, gameEvents);


            serviceE.OnFacebookLoginSucceeded += OnFacebookLoginSucceeded;

            return this;
        }

        public override void Dispose()
        {
            serviceE.OnFacebookLoginSucceeded -= OnFacebookLoginSucceeded;
            if (databaseRef != null)
            {
                //serviceE.PostScoreGlobal -= PostScoreGlobal;
                //serviceE.RequestUserScore -= GetScoreFirebaseLeaderboard;
                serviceE.RequestGiftsFromFriends -= GetGiftsFromFriends;
                serviceE.SendGiftToID -= SendGiftToID;
                serviceE.RequestRemoveGiftFromFriend -= RemoveGiftsFromFriends;
            }
        }

        void OnFacebookLoginSucceeded(string facebookID, string token)
        {
#if DEBUG_FIREBASE
            Debug.Log("FB Facebook connected to database");
#endif

            this.facebookID = facebookID;
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            serviceE.PostScoreGlobal += PostScoreGlobal;
            serviceE.RequestUserScore += GetScoreFirebaseLeaderboard;
            serviceE.RequestGiftsFromFriends += GetGiftsFromFriends;
            serviceE.SendGiftToID += SendGiftToID;
            serviceE.RequestRemoveGiftFromFriend += RemoveGiftsFromFriends;

        }

        void PostScoreGlobal(int score)
        {
#if DEBUG_FIREBASE
			Debug.Log("FB Post Score:" + score.ToString());
#endif
            databaseRef.Child("Users").Child(facebookID).Child("Score").SetValueAsync(score);
        }

        void SendGiftToID(string giftToID)
        {
#if DEBUG_FIREBASE
			Debug.Log("FB Send Gift To :" + giftToID);
#endif
            string userName;
            if (serviceE.GetFacebookUserName != null)
            {
                userName = serviceE.GetFacebookUserName();
            }
            else
            {
                userName = "Your Good Friend";
            }
            databaseRef.Child("Users").Child(giftToID).Child("Gift").Child(facebookID).SetValueAsync(true);
        }

        void GetScoreFirebaseLeaderboard(string facebookID)
        {
#if DEBUG_FIREBASE
			Debug.Log("FB Start Get User Score :" + facebookID);
#endif
            databaseRef.Child("Users").Child(facebookID).Child("Score").GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted && task.Result != null)
                    {
#if DEBUG_FIREBASE
						Debug.Log("FB Get User Score Success:" + facebookID + ". Score:" + task.Result.Value.ToString());
#endif
                        if (serviceE.OnGetUserScore != null)
                        {
#if DEBUG_FIREBASE
							Debug.Log("FB Send Event Get User Score");
#endif
                            int score = 0;
                            if (int.TryParse(task.Result.Value.ToString(), out score))
                            {
                                serviceE.OnGetUserScore(facebookID, score);
                            }
                            else
                            {
                                serviceE.OnGetUserScore(facebookID, 0);
                            }
                        }
                    }
                    else
                    {
#if DEBUG_FIREBASE
						Debug.Log("FB Get User Score Failed:" + facebookID);						
#endif
                        if (serviceE.OnGetUserScore != null)
                        {
                            serviceE.OnGetUserScore(facebookID, 0);
                        }
                    }
                });
        }

        void GetGiftsFromFriends()
        {
#if DEBUG_FIREBASE
			Debug.Log("FB Get Gifts from friends");
#endif
            databaseRef.Child("Users").Child(this.facebookID).Child("Gift").LimitToFirst(25).GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted && task.Result != null)
                    {
                        int count = (int)task.Result.ChildrenCount;
                        if (serviceE.OnGetGiftsFromFriends != null)
                        {
                            List<AGift> gifts = new List<AGift>(count);
                            foreach (DataSnapshot gift in task.Result.Children)
                            {
                                gifts.Add(new AGift(gift.Key));
                            }

                            serviceE.OnGetGiftsFromFriends(gifts);
                        }
#if DEBUG_FIREBASE
						Debug.Log("FB Get " + count.ToString() + " Gifts from friends success");
#endif

                    }
                    else
                    {
#if DEBUG_FIREBASE
						Debug.Log("FB Get Gifts from friends Failed");
#endif
                        if (serviceE.OnGetGiftsFromFriends != null)
                        {
                            serviceE.OnGetGiftsFromFriends(new List<AGift>());
                        }
                    }
                });
        }

        void RemoveGiftsFromFriends(string id)
        {
#if DEBUG_FIREBASE
			databaseRef.Child("Users").Child(this.facebookID).Child("Gift").Child(id).RemoveValueAsync().ContinueWith(task => 
				{
					if(task.IsCompleted)
					{
						Debug.Log("Remove Gift From " + id + " success.");
					}
					else
					{
						Debug.Log("Remove Gift From " + id + " failed.");
					}
				});
#else
            databaseRef.Child("Users").Child(this.facebookID).Child("Gift").Child(id).RemoveValueAsync();
#endif
        }
    }
}
#endif