using UnityEngine;
using System.Collections;

public class Compliment :MonoBehaviourSingleton<Compliment> {

    public Animator anim;
    public SpriteRenderer sRenderer;
    public enum Type { Threehit, Fourhit, GoodJob, Great, Welldone, Excellent, Awesome };
    public Sprite[] sprites;
    private void Awake() {
        m_instance = this;
    }
    public void Show(Type type)
    {
        if (!IsAvailable2Show()) return;

        sRenderer.sprite = sprites[(int)type];
        anim.SetTrigger("show");
    }
    /// <summary>
    /// 当combo数值到达1时候，不给予玩家任何鼓励提示；给予1朵小红花；
    /// 当combo数值到达2时候，给予玩家GoodJob提示；给予3朵小红花；
    ///当combo数值到达3时候，给予玩家welldone提示；给予5朵小红花
    ///当combo数值到达4时候，给予玩家excellent提示；给予8朵小红花
    ///当combo数值到达5时候，给予玩家amazing提示；给予12朵小红花
    ///当combo数值大于等于6时候，给予玩家fantastic提示；给予15朵小红花
    public void ShowRandom(int comboCount)
    {
        if (comboCount <=1) {
            return;
        }
        if (!IsAvailable2Show()) return;

        // sRenderer.sprite = CUtils.GetRandom(sprites);
        //因为任务的关系，这里必须记录下随机的id值
        int index =comboCount - 2 > 0 ? comboCount - 2: 0;
        index = index > 4 ? 4 : index;
        sRenderer.sprite = sprites[index];
        anim.SetTrigger("show");
        if (index == 4) {
            PlayerDataManager.Instance.playerData.fantasticCount++;
            PlayerDataManager.Instance.JudeReachAchieve (2, PlayerDataManager.Instance.playerData.fantasticCount);
        }
        if (Prefs.IsTask==1 && Prefs.ScratchCardTaskFinish == 0) {
            CardManager.Instance.CheckTask (index);
        }
    }
   public IEnumerator DelayShowAchieve() {
        yield return new WaitForSeconds (1f);
        ShowAchievement ();
    }
    public void ShowAchievement() {
        if (!IsAvailable2Show ()) return;
        sRenderer.sprite = sprites[5];
        anim.SetTrigger ("show");
    }

    private bool IsAvailable2Show()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        return info.IsName("Idle");
    }
}
