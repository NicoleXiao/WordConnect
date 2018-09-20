using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardFly : MonoBehaviour {

    public Sprite diamond;
   public  void ShowDiamond() {
        this.GetComponent<SpriteRenderer> ().sprite = diamond;
    }
}
