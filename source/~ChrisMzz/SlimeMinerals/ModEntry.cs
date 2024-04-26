/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChrisMzz/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using StardewValley;
using GenericModConfigMenu;
using SlimeMinerals.Compatibility;
using SlimeMinerals.MineralsData;

namespace SlimeMinerals
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : StardewModdingAPI.Mod
    {
        private static readonly Random rng = new Random();

        public static ModConfig Config { get; set; }

        public static Minerals minerals { get; set; }


        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            helper.Events.World.NpcListChanged += this.OnSlimeKill;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Config = helper.ReadConfig<ModConfig>();
            minerals = new Minerals();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Initialize mod(s)
            ModInitializer modInitializer = new ModInitializer(ModManifest, Helper);
            
            // Get Generic Mod Config Menu API (if it's installed) thanks to ConvenientInventory as a template for this
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                modInitializer.Initialize(api, Config);
            }
        }

        private bool InIntervalNum(int c, int center)
        {
            return (System.Math.Max(0, center - Config.Range) < c) && (c < System.Math.Min(255, center + Config.Range));
        }

        private bool InInterval(Netcode.NetColor color, Color target)
        {
            return (InIntervalNum(color.R, target.R) && InIntervalNum(color.G, target.G) && InIntervalNum(color.B, target.B));
        }



        private void OnSlimeKill(object sender, NpcListChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet or if the location change isn't in the current location
            if (!Context.IsWorldReady || !e.IsCurrentLocation)
                return;

            
            //Game1.addHUDMessage(new HUDMessage(Game1.player.currentLocation.Name, 3));
            // ignore if player is in a location toggled off from Generic Config
            if (!Config.Anywhere) // works fine
            {
                if (Game1.player.currentLocation.Name != "Slime Hutch" && Game1.player.currentLocation.Name != "Woods") { return; }
                if (Game1.player.currentLocation.Name == "Slime Hutch" && !Config.InSlimeHutch) { return; }
                if (Game1.player.currentLocation.Name == "Woods" && !Config.InWoods) { return; }
            }

            foreach (GreenSlime slime in e.Removed.OfType<GreenSlime>())
            {
                Netcode.NetColor color = slime.color; // RGBA (0-255)
                List<Item> loot = new List<Item>();

                //Game1.addHUDMessage(new HUDMessage(color.ToString(), 3));
                // the line of code above runs fine

                //slime.color.Value = new Color(180,220,205); // testing if it works on Opals

                foreach (Mineral mineral in minerals)
                {
                    if (InInterval(color, mineral.targetColor)) { loot.Add(new StardewValley.Object(mineral.iD, 1)); }
                }

                Point center = slime.GetBoundingBox().Center;
                Vector2 dropPos = new Vector2(center.X, center.Y);
                loot.ForEach(mineral => { if (rng.Next(100) <= System.Math.Min((0.11+Game1.player.DailyLuck)*50, 15)) { Game1.createItemDebris(mineral, dropPos, rng.Next(4), slime.currentLocation); }});
                // Computes a percentage based on daily luck, from 0.5% to 10.5%.
                // if luck is somehow increased through other means (possibly other mods), this is capped to 15%
            }


        }




    }
}