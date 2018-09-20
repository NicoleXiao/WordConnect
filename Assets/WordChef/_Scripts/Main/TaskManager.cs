using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TaskManager : MonoBehaviour {
    public Transform Pop;
    public Text text;
    public Button hint;
    public Button taskFinish;
    public Image taskLight;
    public static TaskManager _instance;
    private string taskText = "";
    private bool isShow = true;
    // Use this for initialization
    private void Awake() {
        _instance = this;
        if (Prefs.IsTask == 1) {
            if (Prefs.ScratchCardTaskFinish == 0) {
                hint.gameObject.SetActive (true);
                taskFinish.gameObject.SetActive (false);
                List<CardData> card_data = CSVReadManager.Instance.GetCardData ();
                text.text = string.Format (card_data[Prefs.ScratchCardID].task, Prefs.ScratchCardCount);
                taskText = text.text;
                text.text = text.text + string.Format (" ({0}/{1})", Prefs.ScratchCardGameCount, Prefs.ScratchCardCount);
            } else if (Prefs.ScratchCardTaskFinish == 1) {
                Pop.gameObject.SetActive (false);
                FinishTask ();
            } else if (Prefs.ScratchCardTaskFinish == 2) {
                this.gameObject.SetActive (false);
            }
        } else {
            this.gameObject.SetActive (false);
        }
    }
    void FinishTask() {
        hint.gameObject.SetActive (false);
        taskFinish.gameObject.SetActive (true);
        taskLight.DOFade (1f, 0.5f).SetLoops (-1, LoopType.Yoyo);
    }
    void Start() {
        hint.onClick.AddListener (() => {
            Sound.instance.PlayButton ();
            if (!isShow) {
                isShow = true;
                hint.transform.DOKill ();
                Pop.DOLocalMoveX (-27f, 1f);
            } else {
                isShow = false;
                Pop.DOLocalMoveX (-680f, 1f);
            }
        });
        taskFinish.onClick.AddListener (() => {
            Sound.instance.PlayButton ();
            Prefs.ScratchCardTaskFinish = 2;
            List<CardData> card_data = CSVReadManager.Instance.GetCardData ();
            int number = card_data[Prefs.ScratchCardID].rewardNum * Prefs.ScratchCardCount;
            GemsEffet effet = EffectManager.Instance.LoadGems (transform, taskFinish.transform.position, number);
            effet.OnEffectComplete += () => {
                EventTrackingController.instance.LogGemsAdd (number, "scratchcardtaskfinish");
                CurrencyController.CreditBalance (number);
                this.gameObject.SetActive (false);
                Destroy (effet.gameObject);
            };

        });
    }

    public void UpdateText() {
        text.text = taskText + string.Format (" ({0}/{1})", Prefs.ScratchCardGameCount, Prefs.ScratchCardCount);
    }

    public void Hide() {
        Pop.DOLocalMoveX (-680f, 0.5f).OnComplete (() => {
            this.gameObject.SetActive (false);
            FinishTask ();
        });
    }
}
