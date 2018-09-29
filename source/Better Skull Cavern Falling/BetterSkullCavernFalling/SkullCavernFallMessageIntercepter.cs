using StardewValley;
using StardewValley.Locations;
using System;
using System.Reflection;

namespace BetterSkullCavernFalling
{
    internal class SkullCavernFallMessageIntercepter
    {
        private static readonly MethodInfo MINESHAFT_AFTERFALL_METHOD = typeof(MineShaft).GetMethod("afterFall", BindingFlags.Instance | BindingFlags.NonPublic);

        private int mineLevel;
        private int levelsDownFallen;

        private void AfterFall()
        {
            // string message = Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9578", levelsDownFallen) + (levelsDownFallen > 7 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9580") : "");
            string message = Game1.content.LoadString(levelsDownFallen > 7 ? "Strings\\Locations:Mines_FallenFar" : "Strings\\Locations:Mines_Fallen", levelsDownFallen);

            Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });
            Game1.enterMine(mineLevel + levelsDownFallen);
            Game1.player.faceDirection(2);
            Game1.player.showFrame(5, false);
            Game1.globalFadeToClear(null, 0.01f);
        }

        private SkullCavernFallMessageIntercepter(MineShaft mineShaft)
        {
            this.mineLevel = mineShaft.mineLevel;
            this.levelsDownFallen = (int)typeof(MineShaft).GetField("lastLevelsDownFallen", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mineShaft);
        }

        internal static void Intercept()
        {
            if (Game1.afterFade != null
                && Game1.afterFade.Target is MineShaft mineShaft
                && Game1.afterFade.Method == MINESHAFT_AFTERFALL_METHOD)
            {
                Game1.afterFade = new Game1.afterFadeFunction(new SkullCavernFallMessageIntercepter(mineShaft).AfterFall);
            }
        }
    }
}
