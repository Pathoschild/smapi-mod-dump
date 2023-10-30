/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chudders1231/SDV-FishingProgression
**
*************************************************/

using System;
using System.Collections.Generic;
using FishingProgression.framework;
using FishingProgression.reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace FishingProgression
{
    internal class ModEntry : Mod
    {
        private readonly PerScreen<Bobber> bobber = new();

        /// <summary>Whether the player is in the fishing minigame.</summary>
        private readonly PerScreen<bool> BeganFishingGame = new();

        /// <summary>The number of ticks since the player opened the fishing minigame.</summary>
        private readonly PerScreen<int> UpdateIndex = new();

        public override void Entry(IModHelper helper)
        {
            Globals.InitializeGlobals(this);
            Globals.InitializeConfig();

            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(Globals.Manifest.UniqueID);

            // example patch, you'll need to edit this for your patch
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Tools.FishingRod), nameof(StardewValley.Tools.FishingRod.pullFishFromWater)),
               prefix: new HarmonyMethod(typeof(PullFishFromWater), nameof(PullFishFromWater.pullFishFromWater_Prefix))
            );
        }

        private void OnGameLaunched(object sender, EventArgs e)
        {
            Globals.RegisterConfigMenu();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            this.bobber.Value = e.NewMenu is BobberBar menu ? new Bobber(menu, this.Helper.Reflection) : null;
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            // Checks to see if the world is ready.
            if (!Context.IsWorldReady)
                return;
            // Gets the player
            Farmer player = Game1.player;

            // Checks is the player exists or is not the local player.
            if (player == null || !player.IsLocalPlayer)
            {
                return;
            }

            // Checks if the menu is the fishing minigame and that we have received the minigame instance
            if (Game1.activeClickableMenu is BobberBar && this.bobber.Value !=null && Globals.Config.EnableDifficultyModifier)
            {
                // Allows us to interact with the minigame instance
                Bobber newBobber = this.bobber.Value;

                // Checks if we have applied our buffs or not, and makes sure this is delayed enough so that the minigame has definitely started
                if(!this.BeganFishingGame.Value && this.UpdateIndex.Value > 15)
                {

                    // Added roughly 1/4 of a second after it updates, once per fishing game
                    // Difficulty modifier is calculated based on the player's level
                    float difficultyModifier = Math.Clamp(1 - (player.FishingLevel * (Globals.Config.DifficultyModifier / 100)), 0, 0.75f);

                    // Apply the buffs
                    newBobber.Difficulty *= difficultyModifier;

                    if (player.CurrentTool is FishingRod rod)
                    {

                        if (rod.attachments[1] != null)
                        {
                            var tackle = rod.attachments[1];
                            if (tackle.uses.Value >= 0 && Game1.random.Next(0, 100) <= Math.Clamp(player.FishingLevel * Globals.Config.TackleRestorationChance, 1, 100))
                            {
                                rod.attachments[1].uses.Value -= 1;
                            }
                        }
                    }
                    // Internal stopper to make sure we only apply the buff once per minigame
                    this.BeganFishingGame.Value = true;
                }

                // The timer to make sure that the buffs applied are delayed

                if (this.UpdateIndex.Value < 20)
                {
                    this.UpdateIndex.Value++;
                }

            } else {

                // Resets internal indicators
                this.BeganFishingGame.Value = false;
                this.UpdateIndex.Value = 0;

            }
        }
    }
}
