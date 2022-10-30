/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zeldela/sdv-mods
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.GameData.FishPond;
using GenericModConfigMenu;

namespace FastFishPond
{
    public class ModEntry : Mod
    {
        ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        // testing modconfigmenu
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config),
                titleScreenOnly: true
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Spawn time (days) (CHANGE REQUIRES RESTART)",
                getValue: () => this.Config.spawnTime,
                setValue: value => this.Config.spawnTime = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Vanilla ponds (CHANGE REQUIRES RESTART)",
                tooltip: () => "Turn off for minor rebalancing of fish pond outputs.",
                getValue: () => this.Config.vanilla,
                setValue: value => this.Config.vanilla = value
            );


        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/FishPondData"))
            {
                e.Edit(asset =>
                {
                    var data = (List<FishPondData>)asset.Data;
                    foreach (FishPondData f in data.ToArray())
                    {

                        if (f.SpawnTime < 9999)
                        {
                            f.SpawnTime = this.Config.spawnTime;
                        }

                        if (!this.Config.vanilla)
                        {
                            int minGate = -1;
                            if (f.PopulationGates != null)
                            {
                                int[] gates = f.PopulationGates.Keys.ToArray();
                                f.PopulationGates[gates.Min()] = new List<string> {
                                "685 5", "153", "766"
                            };
                                // minimum number of fish for extra buffs to be applied
                                if (gates.Count() > 1)
                                {
                                    minGate = 3;
                                }
                            }
                            foreach (FishPondReward r in f.ProducedItems)
                            {
                                r.MaxQuantity = Math.Max(2, r.MaxQuantity);
                                if (r.RequiredPopulation > minGate)
                                {
                                    r.MinQuantity = Math.Max(2, r.MinQuantity);
                                    if (r.ItemID != 812)
                                    {
                                        r.Chance = r.Chance < 1 ? Math.Min(r.Chance * this.Config.multiplier, 0.8f) : r.Chance;
                                        r.MaxQuantity = Math.Max(5, r.MaxQuantity);
                                    }
                                }

                            }

                        }
                    }
                });
            }
        }


    }

}

