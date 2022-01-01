/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace JustRelax
{
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;

    public class JustRelax : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.UpdateTicked += RelaxHeal;
        }

        private void RelaxHeal(object sender, UpdateTickedEventArgs args)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            // if we are in bed and in singleplayer (because multiplayer already has healing) or we are sitting
            if ((Game1.player.isInBed.Value && !Game1.IsMultiplayer) || Game1.player.isSitting.Value)
            {
                // 'pause when inactive' only works when you are in true singleplayer, not when you are alone in multiplayer (unlike menu pause)
                bool outOfFocusPaused = Game1.multiplayerMode == Game1.singlePlayer && !Game1.game1.IsActive && Game1.options.pauseWhenOutOfFocus;

                // if the game is not paused in singleplayer and half a second passed (same regen rate as bed in multiplayer)
                if (!(Game1.player.hasMenuOpen.Value && !Game1.IsMultiplayer) && !outOfFocusPaused && !Game1.paused && args.Ticks % 30 == 0)
                {
                    // upper case stamina property has an inbuilt check that it can't go over maxStamina so we can simply add one
                    if (Game1.player.Stamina < Game1.player.maxStamina.Value)
                    {
                        Game1.player.Stamina++;
                    }

                    // health is an int so we can simply add one
                    if (Game1.player.health < Game1.player.maxHealth)
                    {
                        Game1.player.health++;
                    }
                }
            }
        }
    }
}