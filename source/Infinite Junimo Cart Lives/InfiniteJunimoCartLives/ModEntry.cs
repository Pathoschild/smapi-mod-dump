using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Minigames;
using StardewValley;

namespace InfiniteJunimoCartLives
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += Junimo_Lives;
        }

        private void Junimo_Lives(object sender, UpdateTickedEventArgs args)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (Game1.currentMinigame is MineCart game && args.IsMultipleOf(30))
            {
                IReflectedField<int> livesLeft = Helper.Reflection.GetField<int>(game, "livesLeft");
                if (livesLeft?.GetValue() < 3)
                {
                    livesLeft.SetValue(3);
                    //this.Monitor.Log("Set Cart Lives to 3");
                }
            }
        }
    }
}