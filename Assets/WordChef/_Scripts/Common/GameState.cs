public class GameState
{
    public static DailyType dialyType = DailyType.None;
    public static int pictureLevel=1;
    public static int crossyLevel = 1;
    public static int searchLevel = 1;
    public static int puzzleLevel = 1;

    public static int chapterLevelNum = 20;//一个章节里面关卡的最大数
    public static int chapterCount = 0;

    public static bool daySign = false;//今天是否签到了
    public static bool firstEnterMap = true;
    public static bool firstClickPlay= true;
    public static bool unlockNewLevel = false;//解锁新关卡
    public static bool unLockNewGrahp = false;

    public static bool isShowBanner = false;
    public static int passLevelCount = 0;//已经通过的关卡数
    public static int starLightCount = 0;//解锁关卡点亮的星星
    public static int starTotalCount = 0;//解锁关卡总共的星星
    public static int themeCount = 4;
    public static int freeTickeCount = 1;
    public static int buyTicketCount = 0;//已经购买的次数
    public static int bronzenCount = 0, silverCount = 0, goldCount = 0;//铜，银，金三种奖牌的数量
    public static int playVideoCount = 0;//播放视频的次数，一天只能播放10次
    public static int totalPlayVideo = 50;//每天只能弹出50次
    public static int currentWorldCup=1, currentWorldCupTheme=1, currentWorldCupLevel=0;
    public static int currentWorld, currentSubWorld, currentLevel;
    public static string currentSubWorldName = "Subworld name";
    public static int unlockedWorld = -1, unlockedSubWord = -1, unlockedLevel = -1;
}