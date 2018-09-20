using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class SlotNumManager : MonoBehaviour {
    public SlotMachineDialog dialog;
    public Transform numGroup1;
    public Transform numGroup2;
    public Transform numGroup3;
    public Transform claimTr;
    public Transform rubyTrans;
    public System.Action onSlotComplete;
    private Vector3 hide;//隐藏时候的坐标
    private List<int> nums = new List<int> ();

    void Start() {
        SetNum (numGroup1, 1);
        SetNum (numGroup2, 2);
        SetNum (numGroup3, 3);
    }

    void SetNum(Transform numGroup, int index) {
        for (int i = 0; i < 20; i++) {
            if (i == 0) {
                numGroup.GetChild (i).GetComponentInChildren<Text> ().text = SetFinalSlotNum (index).ToString ();
            } else {
                int id;
                if (i == 19) {
                    id = 0;
                } else {
                    id = Random.Range (0, 10); //随机下标
                }
                GameObject clone = Instantiate (numGroup.GetChild (0).gameObject, numGroup);
                clone.GetComponentInChildren<Text> ().text = id.ToString ();
            }
        }
    }



    public void CheckReward() {
        bool isSame = true;
        for (int i = 1; i < nums.Count; i++) {
            if (nums[0] != nums[i]) {
                isSame = false;
                break;
            }
        }
        int reward = nums[0] * 100 + nums[1] * 10 + nums[2];
        if (isSame) {
            reward = 500;
            PlayerDataManager.Instance.playerData.slotBounsCount++;
            PlayerDataManager.Instance.JudeReachAchieve (14, PlayerDataManager.Instance.playerData.slotBounsCount);
        }
        GemsEffet gems = EffectManager.Instance.LoadGems (dialog.transform, claimTr.transform.position,reward);
        gems.OnEffectComplete += () => {
           CurrencyController.CreditBalance (reward);
           EventTrackingController.instance.LogGemsAdd (reward, "slot");
           if (onSlotComplete != null) {
               onSlotComplete ();
           }
           Destroy (gems.gameObject);
       };

    }
    
  
    public int SetFinalSlotNum(int index) {
        //如果玩家通过第二个章节 则 固定第一个数字为1
        if ((Prefs.unlockedSubWorld <= 1) && index == 1) {
            nums.Add (1);
            return 1;
        }

        List<SlotChance> chance = CSVReadManager.Instance.GetSlotChange (index);
        int num = Random.Range (0, 100);
        //  Debug.Log("Number -------------" +num);
        int currentRate = 0;
        int nextRate = 0;
        for (int i = 0; i < chance.Count; i++) {
            if (i != 0) {
                currentRate += chance[i - 1].chance;
            }
            nextRate += chance[i].chance;
            if (num >= currentRate && num < nextRate) {
                // Debug.Log ("Slot num " + index + " : " + chance[i].num);
                nums.Add (chance[i].num);
                return chance[i].num;
            }

        }
        return 0;
    }
}



