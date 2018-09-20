using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager :MonoBehaviourSingleton<EffectManager> {
    private GemsEffet gems;

    private void Awake() {
        m_instance = this;
    }
    public GemsEffet LoadGems(Transform parent,Vector3 pos,int number, bool showPath = false) {
        gems = Instantiate (MonoUtils.instance.gemsEffect).GetComponent<GemsEffet> ();
        gems.transform.SetParent(parent, false);
        gems.transform.position =pos;
        gems.transform.localScale = Vector3.one;
        gems.GetComponent<Canvas> ().enabled = true;
        gems.GetComponent<Canvas> ().overrideSorting = true;
        gems.GetComponent<Canvas> ().sortingLayerName = "UI2";
        gems.GetComponent<GemsEffet> ().Show (number,showPath);
        
        return gems;
    }
    public void  DestroyGems() {
        if (gems != null) {
            Destroy (gems.gameObject);
        }
    }
}
