using Facebook.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class HomeController : BaseController {
    public Button playBtn;
    public Button faceBookBtn;
    public Button dailyBtn;
    public GameObject facebookReward;
    private int loadSceneIndex;
    private string m_info;

    protected override void Awake() {
        base.Awake ();
        GameState.currentLevel = Prefs.unlockedLevel;
        GameState.currentWorld = Prefs.unlockedWorld;
        GameState.passLevelCount = Prefs.PassLevelCount;
        //按钮注册
        faceBookBtn.onClick.AddListener (() => {
            OnClickFaceBook ();
        });

        playBtn.onClick.AddListener (() => {
            ClickPlay ();
        });
        dailyBtn.onClick.AddListener (()=> {
            ClickDaily ();
        });
    }

    protected override void Start() {
        base.Start ();
        SplashController.HideSplash ();
        dailyBtn.transform.DOScale (1.05f, 0.8f).SetLoops (-1, LoopType.Yoyo);
        playBtn.transform.DOScale (1.05f, 0.8f).SetLoops (-1, LoopType.Yoyo);
        if (!FB.IsLoggedIn) {
            if (Prefs.FirstLoginFacebook == 1) {
                facebookReward.transform.GetChild (0).gameObject.SetActive (false);
            }
            facebookReward.transform.DOScale (1.05f, 0.8f).SetLoops (-1, LoopType.Yoyo);
            facebookReward.SetActive (true);
        } else {
            facebookReward.transform.DOKill ();
            facebookReward.SetActive (false);
        }

    }

    //向网站请求数据
    //IEnumerator GetData() {
    //    //使用Get方式访问HTTP地址    
    //    WWW www = new WWW ("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=2");
    //    //等待服务器的响应    
    //    yield return www;
    //    //如果出现错误    
    //    if (www.error != null) {
    //        //获取服务器的错误信息    
    //        m_info = null;
    //    }
    //    //获取服务器的响应文本    
    //    m_info = www.text;
    //    SetTime ();
    //}

    private void SetTime() {
        DateTime realtime=DateTime.Now;
        //if (!string.IsNullOrEmpty (m_info)) {
        //    Debug.Log ("Time: " + m_info);
        //    string timeStamp = m_info.Split ('=')[1].Substring (0, 10);
        //    realtime = GetTime (timeStamp);
        //} else {
        //    realtime = DateTime.Now;
        //}
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString ("DateTime"))) {
            string dateBefore = PlayerPrefs.GetString ("DateTime");
            if (dateBefore == realtime.ToString ("d")) {
                GameState.playVideoCount = Prefs.playVideoCount;
            } else {
                RestSign ();
                Prefs.playVideoCount = 0;
                GameState.playVideoCount = Prefs.playVideoCount;
                PlayerDataManager.Instance.NewDay ();
                PlayerDataManager.Instance.loginDay = true;
                PlayerDataManager.Instance.playerData.loginDayCount++;
                DateTime before = Convert.ToDateTime (dateBefore);
                TimeSpan sp = realtime - before;
                if (sp.Days == 1) {
                    PlayerDataManager.Instance.continuousLoginDay = true;
                    PlayerDataManager.Instance.playerData.continuousLoginCount++;
                }
            }
        } else {
            RestSign ();
            PlayerDataManager.Instance.NewDay ();
            PlayerDataManager.Instance.loginDay = true;
            PlayerDataManager.Instance.continuousLoginDay = true;
            PlayerDataManager.Instance.playerData.loginDayCount = 1;
            PlayerDataManager.Instance.playerData.continuousLoginCount = 1;
        }
        PlayerPrefs.SetString ("DateTime", realtime.ToString ("d"));
        CUtils.LoadScene (loadSceneIndex, true);
    }

    private void RestSign() {
        if (Prefs.SignDay == 0 || Prefs.SignDay == 28) {
            Prefs.SignDay = 1;
        } else {
            Prefs.SignDay = Prefs.SignDay + 1;
        }
        Prefs.IsTask = 0;
        Prefs.SignPop = 0;
        Prefs.Signed = 0;
        //刮刮奖相关
        Prefs.ScratchCardCount = 0;
        Prefs.ScratchCardGameCount = 0;
        Prefs.ScratchCardID = 0;
        Prefs.ScratchCardTaskFinish = 0;
    }

    private void ClickPlay() {
        playBtn.enabled = false;
        Sound.instance.PlayButton ();
        loadSceneIndex = Const.SCENE_MAINGAME;
        if (Prefs.FirstEnterGame == 0) {
            Prefs.FirstEnterGame = 1;
        } else {
            if (Prefs.SignPop == 0 && Prefs.Signed == 0) {
                loadSceneIndex = Const.SCENE_SIGN;
            }
        }
        if (GameState.firstClickPlay) {
            GameState.firstClickPlay = false;
            //  StartCoroutine (GetData ());
            SetTime ();
        } else {
            CUtils.LoadScene (loadSceneIndex, true);
        }
    }
    private void ClickDaily() {
        dailyBtn.enabled = false;
        loadSceneIndex = Const.SCENE_DAILY;
        Sound.instance.PlayButton ();
        if (GameState.firstClickPlay) {
            GameState.firstClickPlay = false;
            //StartCoroutine (GetData ());
            SetTime ();
        } else {
            CUtils.LoadScene (loadSceneIndex, true);
        }


    }

    public DateTime GetTime(string timeStamp) {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
        long lTime = long.Parse (timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan (lTime);
        return dtStart.Add (toNow);
    }

    public void OnClickFaceBook() {
        Debug.Log ("Facebook  is LoggedIn :" + FB.IsLoggedIn);
        if (FB.IsLoggedIn || ConfigController.Config.enableFacebookFeatures) {
            //CUtils.LikeFacebookPage (ConfigController.Config.facebookPageID);
        } else {
            FacebookController.instance.onFacebookLoginComplete += LoginReward;
            FacebookController.instance.LoginFacebook ();
        }

        Sound.instance.PlayButton ();
    }

    // 第一次登陆facebook的奖励
    private void LoginReward() {
        if (Prefs.FirstLoginFacebook == 0) {
            Prefs.FirstLoginFacebook = 1;
            facebookReward.SetActive (false);
            CurrencyController.CreditBalance (50);
            EventTrackingController.instance.LogGemsAdd (50, "facebookreward");
        }
    }

}
