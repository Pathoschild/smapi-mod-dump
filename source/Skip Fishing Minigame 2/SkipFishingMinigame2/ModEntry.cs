/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/philosquare/SDVSkipFishingMinigame2
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Tools;

namespace SkipFishingMinigame2
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private int oldTimeInterval = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.Display_MenuChanged;
            helper.Events.Display.Rendered += Display_AutoHook;
            helper.Events.GameLoop.UpdateTicked += GameLoop_PauseFishTime;
        }

        private void GameLoop_PauseFishTime(object? sender, UpdateTickedEventArgs e)
        {
            if (!(Game1.player.CurrentTool is FishingRod fishingRod) || !fishingRod.isFishing)
            {
                oldTimeInterval = Game1.gameTimeInterval;
                return;
            }
            Game1.gameTimeInterval = oldTimeInterval;
        }

        private void Display_AutoHook(object? sender, RenderedEventArgs e)
        {
            if (!(Game1.player.CurrentTool is FishingRod fishingRod) || !fishingRod.isNibbling ||
                !fishingRod.isFishing || fishingRod.hit || fishingRod.isReeling)
            {
                return;
            }
            Farmer who = Game1.player;
            fishingRod.timeUntilFishingBite = -1f;
            fishingRod.DoFunction(who.currentLocation, (int)fishingRod.bobber.X, (int)fishingRod.bobber.Y, 1, who);
            this.Monitor.Log($"{who.Name} fishingRod DoFunction fishingRod.bobber.X {fishingRod.bobber.X} fishingRod.bobber.Y {fishingRod.bobber.Y} ", LogLevel.Debug);
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is BobberBar bobberBar) || !(Game1.player.CurrentTool is FishingRod fishingRod))
            {
                return;
            }
            string whichFish = ((Mod)this).Helper.Reflection.GetField<string>((object)bobberBar, "whichFish", true).GetValue();
            int fishSize = ((Mod)this).Helper.Reflection.GetField<int>((object)bobberBar, "fishSize", true).GetValue();
            int fishQuality = ((Mod)this).Helper.Reflection.GetField<int>((object)bobberBar, "fishQuality", true).GetValue();
            float difficulty = ((Mod)this).Helper.Reflection.GetField<float>((object)bobberBar, "difficulty", true).GetValue();
            bool treasureCaught = ((Mod)this).Helper.Reflection.GetField<bool>((object)bobberBar, "treasure", true).GetValue();
            bool perfect = ((Mod)this).Helper.Reflection.GetField<bool>((object)bobberBar, "perfect", true).GetValue();
            bool fromFishPond = ((Mod)this).Helper.Reflection.GetField<bool>((object)bobberBar, "fromFishPond", true).GetValue();
            Vector2 fishShake = ((Mod)this).Helper.Reflection.GetField<Vector2>((object)bobberBar, "fishShake", true).GetValue();
            int perfections = ((!(Game1.currentMinigame is FishingGame)) ? (-1) : ((Mod)this).Helper.Reflection.GetField<int>((object)Game1.currentMinigame, "perfections", true).GetValue());
            string setFlagOnCatch = ((Mod)this).Helper.Reflection.GetField<string>((object)bobberBar, "setFlagOnCatch", true).GetValue();
            bool bossFish = ((Mod)this).Helper.Reflection.GetField<bool>((object)bobberBar, "bossFish", true).GetValue();
            string baitId = fishingRod?.GetBait()?.QualifiedItemId;
            int numCaught = ((bossFish || !(baitId == "(O)774") || !(Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0)) ? 1 : 2);
            if (baitId == "(O)ChallengeBait")
            {
                numCaught = 3;
            }
            if (perfect && Game1.currentMinigame is FishingGame)
            {
                Game1.CurrentEvent.perfectFishing();
            }
            this.Monitor.Log($"whichFish {whichFish} fishSize {fishSize} fishQuality {fishQuality} difficulty {difficulty} treasureCaught {treasureCaught} ", LogLevel.Debug);
            this.Monitor.Log($"perfect {perfect} fromFishPond {fromFishPond} setFlagOnCatch {setFlagOnCatch} bossFish {bossFish} baitId {baitId} numCaught {numCaught}", LogLevel.Debug);

            fishingRod.pullFishFromWater(whichFish, fishSize, fishQuality, (int)difficulty, treasureCaught, perfect, fromFishPond, setFlagOnCatch, bossFish, numCaught);
            Game1.exitActiveMenu();
            Game1.setRichPresence("location", Game1.currentLocation.Name);

        }

    }
}