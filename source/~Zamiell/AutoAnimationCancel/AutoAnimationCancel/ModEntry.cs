/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace AutoAnimationCancel
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (ShouldAnimationCancel())
            {
                AnimationCancel();
            }
        }

        public bool ShouldAnimationCancel()
        {
            if (Game1.player.CurrentItem is FishingRod)
            {
                return false;
            }
            
            if (Game1.player.CurrentItem is MeleeWeapon w)
            {
                switch (Game1.player.FarmerSprite.CurrentSingleAnimation)
                {
                    case 24: // Swing down
                    case 30: // Swing left/right
                    case 36: // Swing up
                        return Game1.player.FarmerSprite.currentAnimationIndex >= 4;
                }
            }
            else
            {
                // From: https://github.com/Underscore76/SDVTASMod/blob/main/TASMod/Automation/AnimationCancel.cs
                switch (Game1.player.FarmerSprite.CurrentSingleAnimation)
                {
                    case 66: // Axe/Pickaxe/Hoe down
                    case 48: // Axe/Pickaxe/Hoe left/right
                    case 36: // Axe/Pickaxe/Hoe down
                        return Game1.player.FarmerSprite.currentAnimationIndex >= 2;

                    case 54: // Watering Can down
                    case 58: // Watering Can left/right
                    case 62: // Watering Can up
                        return Game1.player.FarmerSprite.currentAnimationIndex >= 3;
                }
            }

            return false;
        }

        // This is the animation cancel code from: Game1::checkForEscapeKeys
        private void AnimationCancel()
        {
            Game1.freezeControls = false;
            Game1.player.forceCanMove();
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.player.UsingTool = false;
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
