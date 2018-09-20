using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ShopDialog : Dialog
{
    public Text [] numRubyTexts, priceTexts;
    //public Image[] tagImages;//标签图标
   // public Sprite [] tagSprites;
  //  public GameObject[] adsTag;
    public RewardedButton rewardBtn;
    public Transform itemsTrans;
    public Image yellowLight, purpleLight;
    //protected override void Awake()
    //{
    //    if ( rewardBtn.activeSelf )
    //    {
    //        itemsTrans.localPosition = new Vector3 (itemsTrans.localPosition.x, -34.5f, itemsTrans.localPosition.z);
    //    }
    //    else
    //    {
    //        itemsTrans.localPosition = new Vector3 (itemsTrans.localPosition.x, 14f, itemsTrans.localPosition.z);
    //    }
    //    base.Awake ();
    //}
    protected override void PreShow()
    {
        base.PreShow ();
        if ( rewardBtn.IsAvailableToShow() )
        {
            itemsTrans.localPosition = new Vector3 (itemsTrans.localPosition.x, -316f, itemsTrans.localPosition.z);
        }
        else
        {
            itemsTrans.localPosition = new Vector3 (itemsTrans.localPosition.x, -177f, itemsTrans.localPosition.z);
        }
    }

    protected override void Start()
    {
      
        base.Start();
        yellowLight.DOFade (0f,0.5f).SetLoops(-1,LoopType.Yoyo);
        purpleLight.DOFade (0f, 0.5f).SetLoops (-1, LoopType.Yoyo);
      
   
        Purchaser.instance.onItemPurchased += OnItemPurchased;

        for(int i = 0; i < numRubyTexts.Length; i++)
        {
            numRubyTexts[i].text = Purchaser.instance.iapItems[i].value + " gems";
            priceTexts[i].text = Purchaser.instance.iapItems[i].price + "$";
           // adsTag [i].SetActive (Purchaser.instance.iapItems [i].isRemoveAd);
            //if ( Purchaser.instance.iapItems [i].productTag == IAPItem.ProductTag.Popular )
            //{
            //    tagImages [i].sprite = tagSprites [0];
            //    tagImages [i].gameObject.SetActive (true);
            //}
            //else if ( Purchaser.instance.iapItems [i].productTag == IAPItem.ProductTag.Valuable )
            //{
               
            //    tagImages [i].sprite = tagSprites [1];
            //    tagImages [i].gameObject.SetActive (true);
            //}
        }
    }
    public void ChangeItemsGroupPos( bool up )
    {
        if ( up )
        {
            itemsTrans.localPosition = new Vector3 (itemsTrans.localPosition.x, -177f, itemsTrans.localPosition.z);
        }
        else
        {
            itemsTrans.localPosition = new Vector3 (itemsTrans.localPosition.x, -316f, itemsTrans.localPosition.z);
        }
    }
    public void OnBuyProduct(int index)
	{
		Sound.instance.PlayButton();
        Purchaser.instance.BuyProduct(index);
    }

    private void OnItemPurchased(IAPItem item, int index)
    {
        // A consumable product has been purchased by this user.
        if (item.productType == ProductType.Consumable)
        {
            CurrencyController.CreditBalance(item.value);
            Toast.instance.ShowMessage("Your purchase is successful");
            // 判断是否是去除广告的
            if ( item.isRemoveAd )
            {
                CUtils.SetBuyItem ();
                CUtils.HideBannerAd();
            }
            EventTrackingController.instance.LogGemsAdd (item.value, "IAP",item.productID);
            EventTrackingController.instance.LogPurchaseEvent (item.price*100,item.productID,"shopdialog",item.receipt,item.signature);
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (item.productType == ProductType.NonConsumable)
        {
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        // Or ... a subscription product has been purchased by this user.
        else if (item.productType == ProductType.Subscription)
        {
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
    }
	
	private void OnDestroy()
    {
        Purchaser.instance.onItemPurchased -= OnItemPurchased;
    }
}
