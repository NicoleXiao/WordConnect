using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPauseDialog :MyButton{
    public int menuIndex;
    public override void OnButtonClick() {
        base.OnButtonClick ();
        DialogController.instance.ShowDialog (DialogType.Pause,DialogShow.DONT_SHOW_IF_OTHERS_SHOWING);
        PauseDialog pause = DialogController.instance.current.GetComponent<PauseDialog> ();
        if (pause != null) {
            pause.SetMenuIndex (menuIndex);
        }
    }
}
