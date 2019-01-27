#if SERVICE_OPENIAB
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OnePF;

namespace Services
{
	public class OpenIABService : IService
	{
		OpenIABEventManager manager;
		IOpenIAB billing;
		
		public override IService Init(SettingDef def, ServiceEvents events)
		{
			if(!def.UseOpenIAB)
			{
				return null;
			}

			base.Init(def, events);
			int len;
			//manager
			manager = GameObject.Instantiate<GameObject>(def.OpenIABManagerPrefab).GetComponent<OpenIABEventManager>();		
#if UNITY_EDITOR	
			billing = new OpenIAB_Editor();
#elif UNITY_ANDROID
			billing = new OpenIAB_Android();
			#if SV_DEBUG_OPENIAB
            Debug.Log("********** Android OpenIAB plugin initialized **********");
			#endif
#elif UNITY_IOS
			billing = new OpenIAB_iOS();
			#if SV_DEBUG_OPENIAB
            Debug.Log("********** iOS OpenIAB plugin initialized **********");
			#endif
#elif UNITY_WP8
            billing = new OpenIAB_WP8();
            Debug.Log("********** WP8 OpenIAB plugin initialized **********");
#else
			Debug.LogError("OpenIAB billing currently not supported on this platform. Sorry.");
#endif			

			//map sku
			Options options = new Options();
            options.checkInventoryTimeoutMs = Options.INVENTORY_CHECK_TIMEOUT_MS * 2;
            options.discoveryTimeoutMs = Options.DISCOVER_TIMEOUT_MS * 2;
            options.checkInventory = false;
            options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;            
			options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;			
			
#if UNITY_EDITOR

#elif UNITY_IOS
			len = def.OpenIABIOSSkus.Count;
			for(int i = 0; i < len; ++i)
			{
				billing.mapSku(def.OpenIABIOSSkus[i].SKU, OpenIAB_iOS.STORE, def.OpenIABIOSSkus[i].SKU);
			}
			options.prefferedStoreNames = new string[] { OpenIAB_iOS.STORE};
            options.availableStoreNames = new string[] { OpenIAB_iOS.STORE};
#elif UNITY_ANDROID
			len = def.OpenIABIOSSkus.Count;
			for(int i = 0; i < len; ++i)
			{
				billing.mapSku(def.OpenIABIOSSkus[i].SKU, OpenIAB_Android.STORE_GOOGLE, def.OpenIABIOSSkus[i].SKU);
			}
			options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE};
            options.availableStoreNames = new string[] { OpenIAB_Android.STORE_GOOGLE};
			options.storeKeys.Add(OpenIAB_Android.STORE_GOOGLE, def.AndroidPublicKey);
			//billing.mapSku("sku", OpenIAB_Android.STORE_GOOGLE, "sku");
			//options.prefferedStoreNames = new string[] { OpenIAB_Android.STORE_AMAZON };
            //options.availableStoreNames = new string[] { OpenIAB_Android.STORE_AMAZON };
			//options.storeKeys = new Dictionary<string, string> { {OpenIAB_Android.STORE_GOOGLE, googlePublicKey} };
            //options.storeKeys = new Dictionary<string, string> { { OpenIAB_Android.STORE_YANDEX, yandexPublicKey } };
            //options.storeKeys = new Dictionary<string, string> { { OpenIAB_Android.STORE_SLIDEME, slideMePublicKey } };
#endif
            billing.init(options);

			//call
			events.PurchasePackage += PurchasePackage;
			events.GetPackages += GetPackages;
			events.RestorePurchase += RestorePurchase;
			//events.
			OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
        	OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
        	OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
        	OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        	OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
        	OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
        	OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
        	OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
			OpenIABEventManager.restoreSucceededEvent += restoreSucceededEvent;
			OpenIABEventManager.restoreFailedEvent += restoreFailedEvent;
			OpenIABEventManager.transactionRestoredEvent += transactionRestoredEvent;
			return this;
		}

		

		List<IABPackage> GetPackages()
		{
			return def.OpenIABIOSSkus;
		}
		
		public override void Dispose()
		{
			serviceE.PurchasePackage -= PurchasePackage;
			serviceE.GetPackages -= GetPackages;
			serviceE.RestorePurchase -= RestorePurchase;
			OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
			OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
			OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
			OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
			OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
			OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
			OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
			OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
			OpenIABEventManager.restoreSucceededEvent -= restoreSucceededEvent;
			OpenIABEventManager.restoreFailedEvent -= restoreFailedEvent;
			OpenIABEventManager.transactionRestoredEvent -= transactionRestoredEvent;
			billing.unbindService();
		}

		void transactionRestoredEvent(string sku){
			#if SV_DEBUG_OPENIAB
			Debug.Log("OpenIAB Restoring! " + sku);
			#endif
			GameSettings.RESTORING_PURCHASE = true;
			GameSettings.RESTORING_IAP_FAILED = false;
		}
		void restoreSucceededEvent(){
			#if SV_DEBUG_OPENIAB
			Debug.Log("OpenIAB Restore success!");
			#endif
			billing.queryInventory();
		}
		void restoreFailedEvent(string sku){
			#if SV_DEBUG_OPENIAB
			Debug.Log("OpenIAB Restore failed!");
			#endif
			if (serviceE.OnRestoreIABFailed != null) {
				serviceE.OnRestoreIABFailed();
			}
		}
		void RestorePurchase() {
			if (Application.internetReachability == NetworkReachability.NotReachable) {
				if (serviceE.OnNoInternetConnection != null)
				{
					serviceE.OnNoInternetConnection(); 
				}
				return;
			}
			#if UNITY_ANDROID
			if (!_isInitialized) {
				if (serviceE.OnOpenIABNotInitialized != null)
				{
					serviceE.OnOpenIABNotInitialized(); 
				}
				return;
			}
			serviceE.OnOpenIABInitialized();
			#endif

			#if !UNITY_EDITOR
			serviceE.ShowLoading();
			#endif
			
			#if UNITY_IOS
			billing.restoreTransactions();
			#elif UNITY_ANDROID
			billing.queryInventory();
			#endif
			
			#if UNITY_EDITOR
			billing.restoreTransactions();
			#endif
		}
		void CheckAlreadyBoughtNonConsumable(string sku, Action onBought, Action onNotBought){

		}
		bool delayPurchasing = false;
        void PurchasePackage(string id)
        {
			if (Application.internetReachability == NetworkReachability.NotReachable) {
				if (serviceE.OnNoInternetConnection != null)
				{
					serviceE.OnNoInternetConnection(); 
				}
				return;
			}
			#if UNITY_ANDROID
			if (!_isInitialized) {
				if (serviceE.OnOpenIABNotInitialized != null)
				{
					serviceE.OnOpenIABNotInitialized(); 
				}
				return;
			}
			serviceE.OnOpenIABInitialized();
			#endif

			#if SV_DEBUG_OPENIAB
			Debug.Log("OpenIAB Purchase Product:" + id);
			#endif


			#if UNITY_EDITOR
			if(serviceE.OnPurchaseSucceeded != null)
			{
				serviceE.OnPurchaseSucceeded(id);
			}
			#endif
			#if !UNITY_EDITOR
			serviceE.ShowLoading();
			#endif
			billing.purchaseProduct(id);
        }

        private void billingSupportedEvent()
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB billingSupportedEvent");
			#endif
        }
		
        private void billingNotSupportedEvent(string error)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB billingNotSupportedEvent: " + error);
			#endif
        }
        private void queryInventorySucceededEvent(Inventory inventory)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB queryInventorySucceededEvent: Inventory : " + inventory);
			Debug.Log("All Purchase : ");
			foreach (var sku in inventory.GetAllPurchases()){
				Debug.Log(sku);
				Debug.Log("SKU:" + sku.Sku);
			}
			#endif

			#if UNITY_ANDROID
			foreach (Purchase purchase in inventory.GetAllPurchases())
			{
				int len = def.OpenIABIOSSkus.Count;
				for(int i = 0; i < len; ++i)
				{					
					if((def.OpenIABIOSSkus[i].Flags & IABPackageFlag.CONSUMABLE) != 0)
					{
						if(purchase.Sku.Equals(def.OpenIABIOSSkus[i].SKU))
						{
							billing.consumeProduct(purchase);
							if(serviceE.OnPurchaseSucceeded != null)
							{
								serviceE.OnPurchaseSucceeded(purchase.Sku);
							}
							break;
						}
					}
				}	
			}
			#endif
			serviceE.OnRestoreIABSucceeded(inventory.GetAllOwnedSkus());
        }

        private void queryInventoryFailedEvent(string error)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB queryInventoryFailedEvent: " + error);
			#endif
        }

        private void purchaseSucceededEvent(Purchase purchase)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB purchaseSucceededEvent: " + purchase);
			#endif
			if(serviceE.OnPurchaseSucceeded != null)
			{
				serviceE.OnPurchaseSucceeded(purchase.Sku);
			}
			
			#if UNITY_ANDROID
			int len = def.OpenIABIOSSkus.Count;
			for(int i = 0; i < len; ++i)
			{
				if((def.OpenIABIOSSkus[i].Flags & IABPackageFlag.CONSUMABLE) != 0)
				{
					if(purchase.Sku.Equals(def.OpenIABIOSSkus[i].SKU))
					{
						billing.consumeProduct(purchase);
						break;
					}
				}
			}			
			#endif
        }

        private void purchaseFailedEvent(int errorCode, string errorMessage)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB purchaseFailedEvent: " + errorMessage);
			#endif
			if(serviceE.OnPurchaseFailed != null)
			{
				serviceE.OnPurchaseFailed(errorMessage);
			}
        }

        private void consumePurchaseSucceededEvent(Purchase purchase)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB consumePurchaseSucceededEvent: " + purchase);
			#endif
        }

        private void consumePurchaseFailedEvent(string error)
        {
			#if SV_DEBUG_OPENIAB
            Debug.Log("OpenIAB consumePurchaseFailedEvent: " + error);
			#endif
        }
    }
}
#endif