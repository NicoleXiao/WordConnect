using UnityEngine;
using System.Collections.Generic;

namespace Superpow {
    public class Utils {
        //星座的名字
        public static string StarName(int id) {
           List<string> world_name= new List<string> () { "Aries", "Cancer", "Libra", "Leo", "Taurus", "Capricorn","Gemini","Aquarius", "Scorpio","Virgo","Pisces", "Sagittarius" };
            return world_name[id];
        }
       
        public static string ChapterBgUrl(int id) {
            List<string> urls= new List<string> () { "ChapterBg/chapterbg1.png", "ChapterBg/chapterbg2.png", "ChapterBg/chapterbg3.png", "ChapterBg/chapterbg4.png" };
            return urls[id];
        }
        public static int GetNumLevels(int world, int subWorld) {
            int[,] numLevels =
{
                { 12, 18, 18, 18, 18 },
                { 18, 18, 18, 18, 18 },
                { 18, 18, 18, 18, 18 },
                { 18, 18, 18, 18, 18 },
                { 18, 18, 18, 18, 18 },
                { 18, 18, 18, 18, 18 },
                { 21, 21, 21, 21, 21 },
                { 21, 21, 21, 21, 21 },
                { 21, 21, 21, 21, 21 },
                { 21, 21, 21, 21, 21 },
                { 21, 21, 21, 21, 21 }

            };
            return numLevels[world, subWorld];
        }

        public static int GetLeaderboardScore() {
            int levelInSub = Prefs.unlockedWorld == 0 && Prefs.unlockedSubWorld == 0 ? 12 : 18;
            int score = (Prefs.unlockedWorld * 5 + Prefs.unlockedSubWorld) * levelInSub + Prefs.unlockedLevel;

            if (levelInSub == 18) score -= 6;
            return score;
        }

        //public static GameLevel Load(int world, int subWorld, int level)
        //{
        //  // Debug.Log ("World_" + world + "/SubWorld_" + subWorld + "/Level_" + level);
        //  //  return Resources.Load<GameLevel>("World_" + world + "/SubWorld_" + subWorld + "/Level_" + level);
        //}
    }
}