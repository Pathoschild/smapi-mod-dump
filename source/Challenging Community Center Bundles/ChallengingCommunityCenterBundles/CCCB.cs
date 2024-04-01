/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/ChallengingCommunityCenterBundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace CCCB
{
    public class CCCB : Mod
    {
        private ModConfig Config;
        private Dictionary<string, string> CustomBundleData;
        private Dictionary<string, string> OldBundleData = new Dictionary<string, string>();
        private Dictionary<string, string> NewBundleData = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> BundlePacks = new Dictionary<string, Dictionary<string, string>>();

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            string BundleVariant = this.Config.BundleVariant;

            // Load all the Content Packs
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                if (this.BundlePacks.Count == 0)
                {
                    this.BundlePacks = contentPack.ReadJsonFile<Dictionary<string, Dictionary<string, string>>>("content.json");
                }
                else
                {
                    Dictionary<string, Dictionary<string, string>> BundlePacksTemp = contentPack.ReadJsonFile<Dictionary<string, Dictionary<string, string>>>("content.json");
                    foreach (KeyValuePair<string, Dictionary<string, string>> entry in BundlePacksTemp)
                    {
                        this.BundlePacks.Add(entry.Key, entry.Value);
                    }
                }

            }

            // Check which Content Packs are loaded
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in this.BundlePacks)
            {
                this.Monitor.Log($"Loaded Bundle: " + entry.Key, LogLevel.Info);
            }

            if (BundlePacks.ContainsKey(this.Config.BundleVariant))
            {
                this.CustomBundleData = BundlePacks[this.Config.BundleVariant];
                this.Monitor.Log($"Game will be using " + this.Config.BundleVariant + " Bundles", LogLevel.Info);
            }
            else
            {
                this.Monitor.Log($"Your Config Settings are invalid.", LogLevel.Error);
                if (BundlePacks.ContainsKey("Pure_Vanilla"))
                {
                    this.CustomBundleData = BundlePacks["Pure_Vanilla"];
                    this.Monitor.Log($"Game will be using pure Vanilla Bundles instead.", LogLevel.Info);
                }
                else
                {
                    return;
                }
            }

            helper.Events.GameLoop.DayStarted += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.DayEnding += this.OnSaving;
        }

        // Set up the new bundles
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            this.OldBundleData.Clear();
            this.NewBundleData.Clear();

            if (Context.IsWorldReady)
            {
                Dictionary<string, string> BundleData = Game1.netWorldState.Value.BundleData;

                for (int index = 0; index < BundleData.Count; index++)
                {
                    string[] BundleName = BundleData.ElementAt(index).Value.Split('/'); // The Name of the Game Bundles
                    string Key = BundleData.ElementAt(index).Key;

                    // For named bundles
                    if (this.CustomBundleData.ContainsKey(BundleName[0]))
                    {
                        // Save the old bundle data to secure the savegame
                        this.OldBundleData.Add(Key, BundleData[Key]);
                        // Get the new bundle data
                        this.NewBundleData.Add(Key, this.CustomBundleData[BundleName[0]]);
                    }

                    if (this.CustomBundleData.ContainsKey(Key))
                    {
                        // Save the old bundle data to secure the savegame
                        this.OldBundleData.Add(Key, BundleData[Key]);
                        // Get the new bundle data
                        this.NewBundleData.Add(Key, this.CustomBundleData[Key]);
                    }
                }
                Game1.netWorldState.Value.SetBundleData(this.NewBundleData);
            }
        }

        //Change the data back to the vanilla bundles for saving (and hopefully saving all savegames)
        private void OnSaving(object sender, EventArgs e)
        {
            //If using Pure_Vanilla the bundles will be overwritten to reset the save game
            //Note: this will also reset all kinds of remixed bundles to the old vanilla presets
            if (this.Config.BundleVariant == "Pure_Vanilla" && BundlePacks.ContainsKey("Pure_Vanilla"))
            {
                return;
            }
            Game1.netWorldState.Value.SetBundleData(this.OldBundleData);
        }
    }
}
