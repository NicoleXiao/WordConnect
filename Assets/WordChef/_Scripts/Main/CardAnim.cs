using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnim : MonoBehaviour {
    public void CardAnimEnd() {
        WorldGroupManager._instance.ShowMapEffect ();
    }
}
