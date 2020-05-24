using System.Reflection;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;

namespace BetterSkullCavernFalling
{
    internal class SkullCavernFallMessageIntercepter
    {
        private static readonly MethodInfo MINESHAFT_AFTERFALL_METHOD = typeof(MineShaft).GetMethod("afterFall", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly int mineLevel;
        private readonly int levelsDownFallen;

        private void AfterFall()
        {
            // string message = Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9578", levelsDownFallen) + (levelsDownFallen > 7 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9580") : "");
            string message = Game1.content.LoadString(levelsDownFallen > 7 ? "Strings\\Locations:Mines_FallenFar" : "Strings\\Locations:Mines_Fallen", levelsDownFallen);

            Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });
            Game1.enterMine(mineLevel + levelsDownFallen);
            Game1.player.faceDirection(2);
            Game1.player.showFrame(5, false);
            Game1.player.forceCanMove();
            Game1.messagePause = true;
            Game1.fadeToBlackAlpha = 1f;
            Game1.globalFadeToClear(null, 0.1f);
        }

        private SkullCavernFallMessageIntercepter(MineShaft mineShaft, IReflectionHelper reflection)
        {
            this.mineLevel = mineShaft.mineLevel;
            this.levelsDownFallen = reflection.GetField<int>(mineShaft, "lastLevelsDownFallen").GetValue();
        }

        internal static void Intercept(IReflectionHelper reflection)
        {
            Game1.afterFade = Intercept(Game1.afterFade, reflection);

            FieldInfo screenFadeFieldInfo = typeof(Game1).GetField("screenFade", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo afterFadeFieldInfo = typeof(ScreenFade).GetField("afterFade", BindingFlags.Instance | BindingFlags.NonPublic);

            ScreenFade game1ScreenFade = (screenFadeFieldInfo.GetValue(null) as ScreenFade);
            afterFadeFieldInfo.SetValue(game1ScreenFade, Intercept(afterFadeFieldInfo.GetValue(game1ScreenFade) as Game1.afterFadeFunction, reflection));
        }

        private static Game1.afterFadeFunction Intercept(Game1.afterFadeFunction afterFade, IReflectionHelper reflection)
        {
            if (afterFade != null && afterFade.Target is MineShaft mineShaft && afterFade.Method == MINESHAFT_AFTERFALL_METHOD)
            {
                return new SkullCavernFallMessageIntercepter(mineShaft, reflection).AfterFall;
            }
            return afterFade;
        }
    }
}
