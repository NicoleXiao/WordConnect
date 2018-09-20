using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ScreenFader : MonoBehaviour {
    public Image fader;
    public static ScreenFader instance;
    public const float DURATION = 0.2f;

    private void Awake()
    {
        instance = this;
    }

    public void FadeOut(Action onComplete)
    {
        fader.DOKill ();
        fader.enabled = true;
        fader.DOFade (1,DURATION).OnComplete(()=> {
            if (onComplete != null) onComplete ();
        });
        //GetComponent<Animator>().SetTrigger("fade_out");
        //GetComponent<Image>().enabled = true;
        //Timer.Schedule(this, DURATION, () =>
        //{
        //    if (onComplete != null) onComplete();
        //});
    }

    public void FadeIn(Action onComplete)
    {
        fader.DOKill ();
        fader.DOFade (0, DURATION).SetDelay(0.1f).OnComplete (() => {
            fader.enabled = false;
            if (onComplete != null) onComplete ();
        });
        //Debug.Log ("Fade In");
        //GetComponent<Animator>().SetTrigger("fade_in");
        //Timer.Schedule(this, DURATION, () =>
        //{
        //    GetComponent<Image>().enabled = false;
        //    if (onComplete != null) onComplete();
        //});
    }

    public void GotoScene(int sceneIndex)
    {
        FadeOut(() =>
        {
            CUtils.LoadScene(sceneIndex);
        });
    }

    public void BackScene(int sceneIndex) {
        FadeOut (() => {
            SceneManager.LoadScene (sceneIndex);
        });
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log ("LevelFinish");
        //if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ScreenFader_Out"))
        //{
        //    Debug.Log ("GetAnim");
        //    FadeIn (null);
        //}
        FadeIn (null);
    }
}
