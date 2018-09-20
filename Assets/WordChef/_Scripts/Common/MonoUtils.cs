using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MonoUtils : MonoBehaviour {
    public Text letter;
    public Cell cell;
    public LineWord lineWord;
    public Transform textFlyTransform;
    public GameObject rubyFly;
    public GameObject rewardFly;
    public GameObject levelButton;
    public GameObject gemsEffect;

    public static MonoUtils instance;

    private void Awake()
    {
        instance = this;
    }
}
