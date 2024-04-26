/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WizardsLizards/CauldronOfChance
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronOfChance
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        public const string eventId = "10975001";
        public const string UniqueId = "Expl0.CauldronOfChance";

        public static List<long> userIds { get; set; }

        public override void Entry(IModHelper IHelper)
        {
            #region Setup
            var csHarmony = new Harmony(this.ModManifest.UniqueID);

            ObjectPatches.Initialize(this.Monitor, IHelper);

            userIds = new List<long>();
            #endregion Setup

            #region Harmony Patches
            csHarmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), "checkAction"),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.checkAction_Prefix))
            );
            #endregion Harmony Patches

            #region Events
            //IHelper.Events.Input.ButtonPressed += onButtonPressed;
            IHelper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            IHelper.Events.GameLoop.DayEnding += onDayEnding;
            #endregion Events
        }

        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //if (Game1.currentLocation != null && Game1.currentLocation.Name.Equals("WizardHouse"))
            //{
            //    var x = Game1.currentLocation;
            //}

            if (e.Button.IsActionButton())
            {
                //Game1.addHUDMessage(new HUDMessage("Tile: X: " + e.Cursor.Tile.X + "Y: " + e.Cursor.Tile.Y));
                //Game1.addHUDMessage(new HUDMessage("ScreenPixels: X: " + e.Cursor.ScreenPixels.X + "Y: " + e.Cursor.ScreenPixels.Y));
                //Game1.addHUDMessage(new HUDMessage("AbsolutePixels: X: " + e.Cursor.AbsolutePixels.X + "Y: " + e.Cursor.AbsolutePixels.Y));
                //Game1.addHUDMessage(new HUDMessage(Game1.player.DailyLuck.ToString()));
                //Game1.player.performPlayerEmote("sick");
            }
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                GameLocation WizardHouse = Game1.locations.Where(x => x.Name.Equals("WizardHouse")).First();
                WizardHouse.setTileProperty(2, 20, "Buildings", "Action", "CauldronOfChance");
                WizardHouse.setTileProperty(3, 20, "Buildings", "Action", "CauldronOfChance");
                WizardHouse.setTileProperty(4, 20, "Buildings", "Action", "CauldronOfChance");
                WizardHouse.setTileProperty(2, 21, "Buildings", "Action", "CauldronOfChance");
                WizardHouse.setTileProperty(3, 21, "Buildings", "Action", "CauldronOfChance");
                WizardHouse.setTileProperty(4, 21, "Buildings", "Action", "CauldronOfChance");
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Could not add TileProperties to the Wizards Cauldron:\n{ex}", LogLevel.Error);
            }
        }

        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            userIds = new List<long>();
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/WizardHouse"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, string>();
                    editor.Data[eventId + "/f Wizard 500/p Wizard"] = String.Join("/", new string[]
                    {
                        "WizardSong",
                        "-1000 -1000",
                        "farmer 8 24 0 Wizard 3 19 2",
                        "skippable",
                        "showFrame Wizard 20",
                        "viewport 8 18 true",
                        "playSound doorClose",
                        "move farmer 0 -2 0",
                        "playSound bubbles",
                        "emote Wizard 56",
                        "pause 1000",
                        "animate Wizard false false 100 20 21 22 0",
                        "move Wizard 0 0 2 false",
                        "pause 500",
                        "stopAnimation Wizard",
                        "emote Wizard 8",
                        "pause 1000",
                        "speak Wizard \"Young @...\"",
                        "speak Wizard \"Come in...\"",
                        "move Wizard 3 0 2 false",
                        "move Wizard 0 1 2 false",
                        "pause 1000",
                        "speak Wizard \"I was just brewing something in my Cauldron.\"",
                        "pause 1000",
                        "emote Wizard 40",
                        "pause 1500",
                        "speak Wizard \"Say, have you ever considered dabbling in the mystical arts?\"",
                        "pause 500",
                        "emote farmer 28",
                        "pause 500",
                        "speak Wizard \"Its really simple, on a basic level.\"",
                        "speak Wizard \"Just throw a few ingredients into the cauldron and see what happens.\"",
                        "pause 500",
                        "speak Wizard \"Experimenting is the key...\"",
                        "pause 500",
                        "quickQuestion #\"I'll try it out!\"#\"I don't think this kinda stuff is for me...\""
                        + "(break)emote Wizard 56\\pause 500\\emote farmer 56\\pause 500\\move Wizard 0 -1 0\\move Wizard -3 0 2\\speak Wizard \"I'm curious to see what you will discover...\""
                        //Add days-after conversation topic (What arcane discoveries did you make?)
                        + "(break)emote Wizard 28\\pause 500\\speak Wizard \"Well, if you ever change your mind...\"\\move Wizard 0 -1 0\\move Wizard -3 0 2",
                        //Add days-after conversation topic (So did you come around to try out the magic of the cauldron?)
                        "playSound bubbles",
                        "globalFade .008",
                        "viewport -1000 -1000",
                        "playMusic none",
                        "pause 2000",
                        "playSound reward",
                        "pause 300",
                        "message \"You can now use the wizards cauldron.\"",
                        "end dialogue Wizard \"Feel free to use my cauldron whenever you like.\"",
                    });
                });
            }
        }

        /*
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Events/WizardHouse"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Events/WizardHouse"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                data[eventId + "/f Wizard 500/p Wizard"] = String.Join("/", new string[]
                {
                    "WizardSong",
                    "-1000 -1000",
                    "farmer 8 24 0 Wizard 3 19 2",
                    "skippable",
                    "showFrame Wizard 20",
                    "viewport 8 18 true",
                    "playSound doorClose",
                    "move farmer 0 -2 0",
                    "playSound bubbles",
                    "emote Wizard 56",
                    "pause 1000",
                    "animate Wizard false false 100 20 21 22 0",
                    "move Wizard 0 0 2 false",
                    "pause 500",
                    "stopAnimation Wizard",
                    "emote Wizard 8",
                    "pause 1000",
                    "speak Wizard \"Young @...\"",
                    "speak Wizard \"Come in...\"",
                    "move Wizard 3 0 2 false",
                    "move Wizard 0 1 2 false",
                    "pause 1000",
                    "speak Wizard \"I was just brewing something in my Cauldron.\"",
                    "pause 1000",
                    "emote Wizard 40",
                    "pause 1500",
                    "speak Wizard \"Say, have you ever considered dabbling in the mystical arts?\"",
                    "pause 500",
                    "emote farmer 28",
                    "pause 500",
                    "speak Wizard \"Its really simple, on a basic level.\"",
                    "speak Wizard \"Just throw a few ingredients into the cauldron and see what happens.\"",
                    "pause 500",
                    "speak Wizard \"Experimenting is the key...\"",
                    "pause 500",
                    "quickQuestion #\"I'll try it out!\"#\"I don't think this kinda stuff is for me...\""
                    + "(break)emote Wizard 56\\pause 500\\emote farmer 56\\pause 500\\move Wizard 0 -1 0\\move Wizard -3 0 2\\speak Wizard \"I'm curious to see what you will discover...\""
                    //Add days-after conversation topic (What arcane discoveries did you make?)
                    + "(break)emote Wizard 28\\pause 500\\speak Wizard \"Well, if you ever change your mind...\"\\move Wizard 0 -1 0\\move Wizard -3 0 2",
                    //Add days-after conversation topic (So did you come around to try out the magic of the cauldron?)
                    "playSound bubbles",
                    "globalFade .008",
                    "viewport -1000 -1000",
                    "playMusic none",
                    "pause 2000",
                    "playSound reward",
                    "pause 300",
                    "message \"You can now use the wizards cauldron.\"",
                    "end dialogue Wizard \"Feel free to use my cauldron whenever you like.\"",
                });
            }
        }
        */
    }
}
