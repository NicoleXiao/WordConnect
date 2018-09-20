using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseDialog : Dialog {
    private int menuIndex = 0;
    // public bool isWorldCupPause;
    protected override void Start() {
        base.Start ();
       
    }

    public void OnContinueClick() {
        Sound.instance.PlayButton ();
        Close ();
    }

    public void OnQuitClick() {
        // Application.Quit();
        Close ();
        GameState.unlockNewLevel = false;
        CUtils.BackScene (Const.SCENE_HOME);
    }

    public void OnMenuClick() {
        GameState.unlockNewLevel = false;
        CUtils.BackScene (menuIndex);
        Sound.instance.PlayButton ();
        Close ();
    }
    public void SetMenuIndex(int index) {
        menuIndex = index;
    }

    public void OnShareClick() {
        Sound.instance.PlayButton ();
        Close ();
    }

    public void OnHowToPlayClick() {
        Sound.instance.PlayButton ();
        DialogController.instance.ShowDialog (DialogType.HowtoPlay);
    }
}
