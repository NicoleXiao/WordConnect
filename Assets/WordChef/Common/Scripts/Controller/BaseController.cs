using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class BaseController : MonoBehaviour
{
    public GameObject donotDestroyOnLoad;
    public string sceneName;
    public Music.Type music = Music.Type.None;
    protected int numofEnterScene;

    protected virtual void Awake()
    {
        if (DonotDestroyOnLoad.instance == null && donotDestroyOnLoad != null)
            Instantiate(donotDestroyOnLoad);

        iTween.dimensionMode = CommonConst.ITWEEN_MODE;
        CPlayerPrefs.useRijndael(CommonConst.ENCRYPTION_PREFS);

        numofEnterScene = CUtils.IncreaseNumofEnterScene(sceneName);
    }

    protected virtual void Start()
    {
        CPlayerPrefs.Save();
        if (JobWorker.instance.onEnterScene != null)
        {
            JobWorker.instance.onEnterScene(sceneName);
        }

#if UNITY_WSA && !UNITY_EDITOR
        StartCoroutine(SavePrefs());
#endif
        Music.instance.Play(music);
        ////关卡要大于等于第一个关卡的第五关才显示广告
        //if ( (GameState.unlockedWorld == 0 && GameState.unlockedSubWord == 0 && GameState.unlockedLevel >= 4)
        //    ||(GameState.unlockedSubWord>0)||(GameState.unlockedWorld>0) )
        //{
        //    if ( ConfigController.instance.config.showad )
        //    {
        //        Debug.Log ("Show Ads------------------------------");
        //        CUtils.ShowBannerAd ();
        //        if ( !CUtils.IsInterstitialReady () )
        //        {
        //            MonoBehaviour.print ("basecontroller request ad");
        //            CUtils.RequestInterstitialAd ();
        //        }

        //    }
        //}

        if (ConfigController.instance.config.showad)
        {
            Debug.Log("Show Ads------------------------------");
            if (Prefs.unlockedLevel >= 16)
            {
                if (!GameState.isShowBanner) {
                    GameState.isShowBanner = true;
                    CUtils.ShowBannerAd ();
                }
            }
            if (!CUtils.IsInterstitialReady())
            {
                MonoBehaviour.print("basecontroller request ad");
                CUtils.RequestInterstitialAd();
            }

        }



        if (!ConfigController.instance.config.showpurchase)
        {
            GameObject.Find("RubyBalance/AddButton").SetActive(false);
        }



    }


    //public virtual void OnApplicationPause(bool pause)
    //{
    //    Debug.Log("On Application Pause");
    //    CPlayerPrefs.Save();
    //    if (pause == false)
    //    {
    //        Timer.Schedule(this, 0.5f, () =>
    //        {
    //            if ((GameState.unlockedWorld == 0 && GameState.unlockedSubWord == 0 && GameState.unlockedLevel >= 4)
    //        || (GameState.unlockedSubWord > 0) || (GameState.unlockedWorld > 0))
    //            {
    //                if (ConfigController.instance.config.showad)
    //                {
    //                    Debug.Log("Show Ads------------------------------");
    //                    if (!CUtils.IsInterstitialReady())
    //                    {
    //                        MonoBehaviour.print("basecontroller request ad");
    //                        CUtils.RequestInterstitialAd();
    //                    }

    //                }
    //            }
    //        });
    //    }
    //}
    private void Update()
    {
        //if(Input.GetKeyDown (KeyCode.Escape)) {
        //    CUtils.LoadScene (0,true);
        //}
    }

    private IEnumerator SavePrefs()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            CPlayerPrefs.Save();
        }
    }
}
