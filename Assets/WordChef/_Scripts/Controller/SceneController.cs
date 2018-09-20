using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
 * 场景的管理类，适用于返回按钮
 * */
public class SceneController : MonoBehaviourSingleton<SceneController> {

    private List<int> scene = new List<int> ();
    private List<int> showPauseIndex = new List<int> () {
        Const.SCENE_MAINGAME, Const.SCENE_CROSSY, Const.SCENE_PUZZLE, Const.SCENE_SEARCH, Const.SCENE_PICTURE
    };
    private void Awake() {
        m_instance = this;
    }

    public void AddScene() {
            int sceneIndex = SceneManager.GetActiveScene ().buildIndex;
            if (!scene.Contains (sceneIndex)) {
                scene.Add (sceneIndex);
            }
        
    }
    //功能暂时屏蔽
    private void Update() {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (scene.Count > 0) {
                //如果在游戏界面，并且打开了胜利界面，直接进到下一关的游戏
                if (SceneManager.GetActiveScene ().buildIndex == Const.SCENE_MAINGAME && DialogController.instance.IsDialogShowing (DialogType.Win)) {
                    WinDialog.instance.NextClick (false);
                }
                //先判断弹窗是否是打开的是的话，就关闭弹窗
                else if (DialogController.instance.IsDialogShowing ()) {
                    DialogController.instance.CloseCurrentDialog ();
                }  
                //如果在游戏界面，没有任何弹窗弹出的情况下，弹出暂停。
                else if (showPauseIndex.Contains (SceneManager.GetActiveScene ().buildIndex)) {
                    DialogController.instance.ShowDialog (DialogType.Pause);
                    PauseDialog pause = DialogController.instance.current.GetComponent<PauseDialog> ();
                    if (pause != null) {
                        if (SceneManager.GetActiveScene ().buildIndex == Const.SCENE_MAINGAME) {
                            pause.SetMenuIndex (Const.SCENE_MAP);
                        } else {
                            pause.SetMenuIndex (Const.SCENE_DAILY);
                        }
                    }
                } else {

                    BackLastScene ();
                }
            } else {
                Application.Quit ();
            }


        }
    }
    //回到上一个场景
    public void BackLastScene() {
        if (SceneManager.GetActiveScene ().buildIndex == Const.SCENE_HOME) {
            scene.Clear ();
            Application.Quit ();
            return;
        }
        // int index = (int)sceneStack.Pop ();
        if (scene.Count > 0) {
            int index = scene[scene.Count - 1];
            //如果是主菜单场景，直接返回home
            if (SceneManager.GetActiveScene ().buildIndex == Const.SCENE_MAP) {
                scene.Clear ();
                CUtils.BackScene (Const.SCENE_HOME);
                return;
            } else if (SceneManager.GetActiveScene ().buildIndex ==Const.SCENE_DAILY) {
                scene.Clear ();
                CUtils.BackScene (Const.SCENE_HOME);
                return;
            }
            scene.RemoveAt (scene.Count - 1);
            CUtils.BackScene (index, this);

        }
    }

}
