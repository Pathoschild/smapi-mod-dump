using System.Reflection;
using StardewModdingAPI;
using StardewValley;
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
            Game1.globalFadeToClear(null, 0.01f);
        }

        private SkullCavernFallMessageIntercepter(MineShaft mineShaft, IReflectionHelper reflection)
        {
            this.mineLevel = mineShaft.mineLevel;
            this.levelsDownFallen = reflection.GetField<int>(mineShaft, "lastLevelsDownFallen").GetValue();
        }

        internal static void Intercept(IReflectionHelper reflection)
        {
            if (Game1.afterFade != null
                && Game1.afterFade.Target is MineShaft mineShaft
                && Game1.afterFade.Method == MINESHAFT_AFTERFALL_METHOD)
            {
                Game1.afterFade = new SkullCavernFallMessageIntercepter(mineShaft, reflection).AfterFall;
            }
        }
    }
}
