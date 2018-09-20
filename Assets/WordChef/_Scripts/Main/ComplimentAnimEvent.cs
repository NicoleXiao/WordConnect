using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplimentAnimEvent : MonoBehaviour {
    public GameObject mask;
    public void ShowMask() {
        mask.SetActive (true);
    }
    public void HideMask() {
        mask.SetActive (false);
    }
}
