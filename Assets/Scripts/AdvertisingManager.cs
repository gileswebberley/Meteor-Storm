using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


public class AdvertisingManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsListener
{
    //From Ivo at skills city
    [SerializeField] string androidGameId;
    [SerializeField] string iOSGameId;
    [SerializeField] bool testMode = true;
    string adId = null;

    void Awake()
    {
        CheckPlatform();   
    }

    void Start()
    {
        
    }

    void CheckPlatform()
    {
        //Conditional Compilation: https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
        string gameId = null;

#if UNITY_IOS
         {
            gameId = iOSGameId;
            adId = "Rewarded_iOS";
         }
#elif UNITY_ANDROID || UNITY_EDITOR
        {
            gameId = androidGameId;
            adId = "Rewarded_Android";
        }
#endif


        Advertisement.Initialize(gameId, testMode, false, this);
    }

    IEnumerator WaitFordAd()
    {
        while (!Advertisement.isInitialized)
        {
            yield return null;
        }
        LoadAd();
    }

    void LoadAd()
    {
        Advertisement.AddListener(this);
        Advertisement.Load(adId);
    }

    public void WatchAd()
    {
        Advertisement.Show(adId);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            // REWARD PLAYER
            Debug.Log("Unity Ads Rewarded Ad Completed");
        }
        else if (showResult == ShowResult.Skipped)
        {
            // DO NOT REWARD PLAYER
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }

        Advertisement.Load(placementId);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsReady(string placementId)
    {
    }
}
