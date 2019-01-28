#if SERVICE_FIREBASE
#if SERVICE_FIREBASE_REALTIMEDATABASE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Analytics;
using System;

namespace Services
{
    public class FirebaseRealtimeDatabaseService : IService
    {
        Firebase.Auth.FirebaseUser user;
        Firebase.Auth.FirebaseAuth auth;

        bool isLoggedIn;
        string facebookID;

        FirebaseService root;

        public FirebaseRealtimeDatabaseService(FirebaseService root)
        {
            this.root = root;
        }

        public override IService Init(SettingDef def, ServiceEvents serviceE)
        {
            if (!def.UseFirebase || !def.UseFBRealtimeDatabase)
            {
                return null;
            }

            base.Init(def, serviceE);
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(def.FirebaseDatabaseURL);
#if UNITY_EDITOR
            FirebaseApp.DefaultInstance.SetEditorP12FileName("word-link-e5bcf-d503d2e36826.p12");
            FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("word-link-e5bcf@appspot.gserviceaccount.com");
            FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
#endif
            return this;
        }

        public override void Dispose()
        {
        }

        public void DeleteData(string path)
        {
#if !UNITY_EDITOR
        if (!readyToUse)
        {
            return;
        }
#endif

#if SV_DEBUG_FIREBASE
        Debug.Log(string.Format("Firebase Delete Data: {0}", path);
#endif
            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            tRef.RemoveValueAsync();
        }

        public void SaveData(string path, string value)
        {
#if !UNITY_EDITOR
        if (!readyToUse)
        {
            return;
        }
#endif
#if SV_DEBUG_FIREBASE
        Debug.Log(string.Format("Firebase Save Data: {0} ({1})",
            path, value));
#endif
            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            tRef.SetValueAsync(value);
            //tRef.SetRawJsonValueAsync(value);
        }

        public void GetData(string path, System.Action<string> rs)
        {
#if !UNITY_EDITOR
        if (!readyToUse)
        {
            return;
        }
#endif

            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            tRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
#if SV_DEBUG_FIREBASE
                Debug.Log("Firebase Get Value Fail:" + path);
#endif
                }
                else if (task.Result == null)
                {
#if SV_DEBUG_FIREBASE
                Debug.Log("Firebase Get Value Null:" + path);
#endif
                }
                else if (task.IsCompleted)
                {
#if SV_DEBUG_FIREBASE
                Debug.Log("Firebase Get Value Success:" + path + ":" + task.Result.Value.ToString() + ".");
#endif
                    DataSnapshot snapshot = task.Result;
                    rs(snapshot.Value.ToString());
                }
                else
                {
#if SV_DEBUG_FIREBASE
                Debug.Log("Firebase Get Value Unknow result:" + path);
#endif
                }
            });
        }

        //first amount > 0 : limit to first
        //first amount < 0 : limit to last
        //first amount == 0 : not limit
        public void GetDataAsListClass<T>(string path, string orderByChild, int limitAmountCode, System.Action<List<T>> callback) where T : IHaveKey
        {
#if !UNITY_EDITOR
        if (!readyToUse)
        {
            return;
        }
#endif

            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            Query query = null;
            if (!string.IsNullOrEmpty(orderByChild))
            {
                query = tRef.OrderByChild(orderByChild);
            }
            if (limitAmountCode != 0)
            {
                if (limitAmountCode > 0)
                {
                    if (query != null)
                    {
                        query = query.LimitToFirst(limitAmountCode);
                    }
                    else
                    {
                        query = tRef.LimitToFirst(limitAmountCode);
                    }
                }
                else
                {
                    if (query != null)
                    {
                        query = query.LimitToLast(-limitAmountCode);
                    }
                    else
                    {
                        query = tRef.LimitToLast(-limitAmountCode);
                    }
                }
            }

            if (query != null)
            {
                query.GetValueAsync().ContinueWith(task =>
                {
                    GetDataAsListClassCallback(task, path, callback);
                });
            }
            else
            {
                tRef.GetValueAsync().ContinueWith(task =>
                {
                    GetDataAsListClassCallback(task, path, callback);
                });
            }
        }

        void GetDataAsListClassCallback<T>(System.Threading.Tasks.Task<DataSnapshot> task, string path, System.Action<List<T>> callback) where T : IHaveKey
        {
            if (task.IsFaulted)
            {
#if SV_DEBUG_FIREBASE
            Debug.Log("Firebase Get Value Fail:" + path);
#endif
            }
            else if (task.IsCompleted)
            {
#if SV_DEBUG_FIREBASE
            Debug.Log("Firebase Get Value Success:" + path);
#endif
                DataSnapshot snapshot = task.Result;
                List<T> rs = new List<T>((int)snapshot.ChildrenCount);
                foreach (DataSnapshot child in snapshot.Children)
                {
                    T ele = JsonUtility.FromJson<T>(child.GetRawJsonValue());
                    ele.Key = child.Key;
                    rs.Add(ele);
                }
                callback(rs);
            }
        }

        public void GetDataAsClass<T>(string path, System.Action<T> callback)
        {
            if (!readyToUse)
            {
                return;
            }

            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            tRef.GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
#if SV_DEBUG_FIREBASE
               Debug.Log("Firebase Get Value Fail:" + path);
#endif
               }
               else if (task.IsCompleted)
               {
#if SV_DEBUG_FIREBASE
               Debug.Log("Firebase Get Value Success:" + path);
#endif
                   DataSnapshot snapshot = task.Result;
                   callback(JsonUtility.FromJson<T>(snapshot.GetRawJsonValue()));
               }
           });
        }

        public void SubcribeToChildChange(string path, EventHandler<ChildChangedEventArgs> callback)
        {

            if (!readyToUse)
            {
                return;
            }


            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            tRef.ChildChanged += callback;
        }

        public void IncreaseValueMultable(string path, int amount)
        {
            if (!readyToUse)
            {
                return;
            }

            string[] nodes = path.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            int len = nodes.Length;
            DatabaseReference tRef = dataRef;
            for (int i = 0; i < len; ++i)
            {
                tRef = tRef.Child(nodes[i]);
            }

            tRef.RunTransaction(multableData =>
            {
                int value;
                if (int.TryParse(multableData.Value.ToString(), out value))
                {
                    multableData.Value = (value + amount).ToString();
                    return TransactionResult.Success(multableData);
                }
                else
                {
                    multableData.Value = amount.ToString();
                    return TransactionResult.Success(multableData);
                }
            });
        }

    }

    public interface IHaveKey
    {
        string Key { get; set; }
    }

}
#endif
#endif