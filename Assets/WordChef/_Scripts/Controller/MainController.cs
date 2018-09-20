using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Superpow;
using DG.Tweening;

public class MainController : BaseController {
    public Text levelNameText;
    [HideInInspector]
    public int starCount = 0;//当前关卡的小红花个数
    [HideInInspector]
    public int comboCount = 0;//当前关卡连词个数
    public Image starSlider;
    public Button starBtn;
    public Transform fly;
    public Text starText;
    public Image starlight;
    private int level;
    private bool isGameComplete;
    private GameLevel gameLevel;


    public static MainController instance;

    protected override void Awake() {
        base.Awake ();
        instance = this;
    }

    protected override void Start() {
        base.Start ();
        level = GameState.currentLevel;
        GameState.currentWorld = CSVReadManager.Instance.GetWorldData (GameState.currentLevel).mapId;
        gameLevel = CSVReadManager.Instance.GetWorldData (GameState.currentLevel).levelData;
        Pan.instance.Load (gameLevel.word);
        WordRegion.instance.Load (gameLevel);
        if (level == 1) {
            Timer.Schedule (this, 0.5f, () => {
                DialogController.instance.ShowDialog (DialogType.HowtoPlay);
            });
        }

        levelNameText.text = "Level" + " - " + GameState.currentLevel;
        if (Prefs.RewardStarCount >= 30) {
            starlight.DOFade (1, 0.5f).SetLoops (-1,LoopType.Yoyo);
            starBtn.enabled = true;
        } else {
            starBtn.enabled = false;
        }
        float progress = (float)Prefs.RewardStarCount / 30f;
        starSlider.fillAmount = progress > 1f ? 1f : progress;
        starBtn.onClick.AddListener (()=> {
            starBtn.enabled = false;
            int reward = Random.Range (10, 20);
            Transform parent = GameObject.Find ("Canvas").transform;
            Vector3 pos = starSlider.transform.position;
            GemsEffet effect= EffectManager.Instance.LoadGems (parent, pos,reward);
            effect.OnEffectComplete += () => {
                EventTrackingController.instance.LogGemsAdd (reward, "smallstarreward");
                CurrencyController.CreditBalance (reward);
                starlight.DOKill ();
                starlight.color = new Color (1, 1, 1, 0);
                Prefs.RewardStarCount = 0;
                starSlider.fillAmount = 0;
                Destroy (effect.gameObject);
            };

        });
        if (GameState.currentLevel == Prefs.unlockedLevel)
        {
            EventTrackingController.instance.LogLevelProcess(true,Prefs.unlockedWorld.ToString(),Prefs.unlockedSubWorld.ToString(), Prefs.unlockedLevel.ToString());
        }
        CheckLoginAchieve ();
    }

    private void CheckLoginAchieve() {
        if (PlayerDataManager.Instance.loginDay) {
            PlayerDataManager.Instance.loginDay = false;
            PlayerDataManager.Instance.JudeReachAchieve (3,PlayerDataManager.Instance.playerData.loginDayCount);
        }
        if (PlayerDataManager.Instance.continuousLoginDay) {
            PlayerDataManager.Instance.continuousLoginDay = false;
            PlayerDataManager.Instance.JudeReachAchieve (3, PlayerDataManager.Instance.playerData.continuousLoginCount);
        }
    }

    public void OnComplete() {
        if (isGameComplete) return;
        isGameComplete = true;
        if (GameState.currentLevel >= Prefs.unlockedLevel) {
            GameState.passLevelCount++;
            Prefs.PassLevelCount = Prefs.PassLevelCount + 1;
            PlayerDataManager.Instance.playerData.passLevel++;
            PlayerDataManager.Instance.JudeReachAchieve (1, PlayerDataManager.Instance.playerData.passLevel);
        }
    
        Timer.Schedule (this, 1f, () => {
            CPlayerPrefs.DeleteKey ("level_progress" + (GameState.currentLevel).ToString ());
            DialogController.instance.ShowDialog (DialogType.Win);
            Sound.instance.Play (Sound.Others.Win);
        });
    }

   

    /// <summary>
    /// 设置星星收集的进度
    /// 30个是满的
    /// </summary>
    public void SetStarProgress(int addCount) {
        if (addCount == 0) {
            return;
        }
        PlayerDataManager.Instance.playerData.accumulativeStar+=addCount;
        PlayerDataManager.Instance.JudeReachAchieve (7, PlayerDataManager.Instance.playerData.accumulativeStar);
        int mOldScore = starCount;
        int newScore = starCount+addCount;
        starCount += addCount;
        Transform flyObj = Instantiate (fly,starSlider.transform.parent);
        flyObj.transform.localPosition = new Vector3 (-195f,-312f,1f);
        flyObj.localScale = Vector3.one;
        flyObj.DOLocalMove (starSlider.transform.localPosition, 0.5f).OnComplete (()=> {
            Sequence mScoreSequence = DOTween.Sequence ();
            mScoreSequence.Append (DOTween.To (delegate (float value) {
                var temp = Mathf.Floor (value);
                starText.text = temp + "";
            }, mOldScore, newScore, 0.4f));
            Prefs.RewardStarCount += addCount;
            if (Prefs.RewardStarCount >= 30) {
                starlight.DOFade (1, 0.5f).SetLoops (-1, LoopType.Yoyo);
                starBtn.enabled = true;
            }
            float progress = (float)Prefs.RewardStarCount / 30f;
            starSlider.fillAmount = progress > 1f ? 1f : progress;
            if (starSlider.fillAmount == 1) {
                starBtn.enabled = true;
            }
            Destroy (flyObj.gameObject);
        });
        

    }
}
