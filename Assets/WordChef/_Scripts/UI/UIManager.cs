using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//UI换肤的管理类 暂时屏蔽功能
public class UIManager : MonoBehaviour {
    //private const string defaultSkinPath = "defaultSkin/";
    //private const string bronzeSkinPath = "bronzeSkin/";
    //private const string sliverSkinPath = "silverSkin/";
    //private const string goldSkinPath = "goldSkin/";
    //private string currentPath = "";
    //private int skinIndex;
    //void Awake() {
    //   // skinIndex = Prefs.UISkin;
       
    //    // StartCoroutine (Change());
    //}
    //private void Start() {
    //    //if ( skinIndex != 0 ) {
    //    //    SetSkinPath ();
    //    //    ChangeImage (this.transform);
    //    //}
    //}
    //public void ChangeSkin(int skin) {
        
    //    //skinIndex = skin;
    //    //Debug.Log ("Change Skin Index :" + skinIndex);
    //    //Prefs.UISkin = skin;
    //    //SetSkinPath ();
    //    //ChangeImage (this.transform);
    //}
    //IEnumerator Change() {
    //    yield return new WaitForSeconds (0.000001f);
    //    if ( skinIndex != 0 ) {
    //        ChangeImage (this.transform);
    //    }
    //}
    //void SetSkinPath() {
    //    switch ( skinIndex ) {
    //        case 0:
    //            currentPath = defaultSkinPath;
    //            break;
    //        case 1:
    //            currentPath = bronzeSkinPath;
    //            break;
    //        case 2:
    //            currentPath = sliverSkinPath;
    //            break;
    //        case 3:
    //            currentPath = goldSkinPath;
    //            break;
    //    }

    //}
    //void ChangeImage( Transform trans ) {
    //    foreach ( Transform child in trans ) {
    //        if ( child.GetComponent<Image> () != null && child.GetComponent<Image> ().sprite != null ) {
    //            ResManager.Instance.LoadSpriteFromStreamingAsset (currentPath + child.GetComponent<Image> ().sprite.name + ".png", delegate ( Sprite sp ) {
    //                child.GetComponent<Image> ().sprite = sp;
    //            }, new Vector2 (0.5f, 0.5f), false);
    //        }
    //        if ( child.GetComponent<SpriteRenderer> () != null && child.GetComponent<SpriteRenderer> ().sprite != null ) {
             
    //            ResManager.Instance.LoadSpriteFromStreamingAsset (currentPath + child.GetComponent<SpriteRenderer> ().sprite.name + ".png", delegate ( Sprite sp ) {
    //                child.GetComponent<SpriteRenderer> ().sprite = sp;
    //            }, new Vector2 (0.5f, 0.5f), false);
    //        }
    //        if ( child.childCount != 0 ) {
    //            ChangeImage (child);
    //        }
    //    }
    //}
    //void LoadSprite( Sprite sprite ) {

    //        ResManager.Instance.LoadSpriteFromStreamingAsset (currentPath + sprite.name + ".png", delegate ( Sprite sp ) {
    //            sprite = sp;
    //        }, new Vector2 (0.5f, 0.5f), false);
        
    //}

}
