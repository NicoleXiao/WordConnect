using UnityEngine;
using System.Collections;

public class SplashController {

    public static void HideSplash() {
        if (Application.platform == RuntimePlatform.Android) {
            using (AndroidJavaObject obj = new AndroidJavaClass ("com.toler.sdk.UnitySplashSDK").CallStatic<AndroidJavaObject> ("getInstance")) {
                obj.Call ("onHideSplash");
            }
        }
    }
   
}
