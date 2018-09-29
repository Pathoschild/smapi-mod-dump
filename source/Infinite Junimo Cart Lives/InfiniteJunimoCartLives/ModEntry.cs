using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Minigames;
using StardewValley;
using System;

namespace InfiniteJunimoCartLives
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            GameEvents.HalfSecondTick += this.Junimo_Lives;

        }

        private void Junimo_Lives(object sender, EventArgs args)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (Game1.currentMinigame is MineCart game)
            {
                IReflectedField<int> livesLeft = this.Helper.Reflection.GetField<int>(game, "livesLeft");
                if (livesLeft != null)
                {
                    if (livesLeft.GetValue() < 3)
                    {
                        livesLeft.SetValue(3);
                        //this.Monitor.Log("Set Cart Lives to 3");
                    }
                    else { return; }
                }
            }
            else
            {
                return;
            }
        }
    }
}