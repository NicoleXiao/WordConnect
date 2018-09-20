using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardedVideoDialog : Dialog {
    public Text amountText;
    public Text messageText;
    public Transform claimTr;
    private int number;
	public void SetAmount(int amount)
    {
        number = amount;
        amountText.text = "X"+amount.ToString();
        messageText.text = string.Format(messageText.text, amount);
    }

    public void Claim()
    {
        Sound.instance.PlayButton ();
        GemsEffet gems= EffectManager.Instance.LoadGems (transform, claimTr.transform.position,number);
        gems.OnEffectComplete += () => {
            CurrencyController.CreditBalance (number);
            EventTrackingController.instance.LogGemsAdd (number, "rewardvideo");
            Close ();
            Destroy (gems.gameObject);
        };

    }
}
