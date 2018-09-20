using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CurrencyBallance : MonoBehaviour {
    private void Start()
    {
        UpdateBalance();
        CurrencyController.onBalanceChanged += OnBalanceChanged;
        CurrencyController.onBallanceIncreased += OnBalanceIncreased;
    }

    private void UpdateBalance()
    {
        gameObject.SetText(CurrencyController.GetBalance().ToString());
    }

    private void OnBalanceChanged()
    {
        UpdateBalance();
    }
    private void OnBalanceIncreased(int add) {
        int newScore = CurrencyController.GetBalance ();
        int mOldScore = newScore - add;
        Sequence mScoreSequence = DOTween.Sequence ();
        mScoreSequence.Append (DOTween.To (delegate (float value) {
            var temp = Mathf.Floor (value);
           gameObject.SetText( temp + "");
        }, mOldScore, newScore, 0.4f));
    }
    private void OnDestroy()
    {
        CurrencyController.onBalanceChanged -= OnBalanceChanged;
        CurrencyController.onBallanceIncreased -= OnBalanceIncreased;
    }
}
