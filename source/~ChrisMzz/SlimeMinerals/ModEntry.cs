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

namespace SlimeMinerals
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : StardewModdingAPI.Mod
    {
        private static readonly Random rng = new Random();

        public static ModConfig Config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            helper.Events.World.NpcListChanged += this.OnSlimeKill;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Config = helper.ReadConfig<ModConfig>();

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



        private void OnSlimeKill(object sender, NpcListChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet or if the location change isn't in the current location
            if (!Context.IsWorldReady || !e.IsCurrentLocation || !Context.IsMainPlayer)
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

                switch (color)
                { 
                    // tiger slimes seem to have the color (255,255,255,255)
                    case Netcode.NetColor rgb when (0 < rgb.R && rgb.R < 1) && (0 < rgb.G && rgb.G < 1) && (0 < rgb.B && rgb.B < 1):
                        loot.Add(new StardewValley.Object("168", 1)); // trash TEST -- never runs, change 1s to 255s to make always it run
                        break;

                    case Netcode.NetColor rgb when (160 < rgb.R && rgb.R < 211) && (209 < rgb.G && rgb.G < 232) && (204 < rgb.B && rgb.B < 247):
                        loot.Add(new StardewValley.Object("564", 1)); //  opal
                        break;

                    case Netcode.NetColor rgb when (38 < rgb.R && rgb.R < 71) && (30 < rgb.G && rgb.G < 64) && (92 < rgb.B && rgb.B < 117):
                        loot.Add(new StardewValley.Object("565", 1)); // fire opal
                        break;

                    case Netcode.NetColor rgb when (27 < rgb.R && rgb.R < 37) && (32 < rgb.G && rgb.G < 50) && (29 < rgb.B && rgb.B < 45):
                        loot.Add(new StardewValley.Object("539", 1)); // bixite
                        break;

                    case Netcode.NetColor rgb when (186 < rgb.R && rgb.R < 214) && (56 < rgb.G && rgb.G < 102) && (56 < rgb.B && rgb.B < 76):
                        loot.Add(new StardewValley.Object("540", 1)); // baryte
                        break;

                    case Netcode.NetColor rgb when (204 < rgb.R && rgb.R < 242) && (143 < rgb.G && rgb.G < 161) && (166 < rgb.B && rgb.B < 232):
                        loot.Add(new StardewValley.Object("543", 1)); // dolomite
                        break;

                    case Netcode.NetColor rgb when (231 < rgb.R && rgb.R < 255) && (196 < rgb.G && rgb.G < 232) && (64 < rgb.B && rgb.B < 140):
                        loot.Add(new StardewValley.Object("542", 1)); //  calcite
                        break;

                    case Netcode.NetColor rgb when (74 < rgb.R && rgb.R < 122) && (155 < rgb.G && rgb.G < 184) && (163 < rgb.B && rgb.B < 217):
                        loot.Add(new StardewValley.Object("541", 1)); //aerinite
                        break;


                    case Netcode.NetColor rgb when (148 < rgb.R && rgb.R < 178) && (155 < rgb.G && rgb.G < 178) && (176 < rgb.B && rgb.B < 211):
                        loot.Add(new StardewValley.Object("544", 1)); //esperite
                        break;



                    case Netcode.NetColor rgb when (132 < rgb.R && rgb.R < 163) && (127 < rgb.G && rgb.G < 158) && (178 < rgb.B && rgb.B < 219):
                        loot.Add(new StardewValley.Object("545", 1)); //fluoropatite
                        break;


                    case Netcode.NetColor rgb when (143 < rgb.R && rgb.R < 173) && (201 < rgb.G && rgb.G < 224) && (104 < rgb.B && rgb.B < 186):
                        loot.Add(new StardewValley.Object("546", 1)); //geminite
                        break;


                    case Netcode.NetColor rgb when (200 < rgb.R && rgb.R < 230) && (30 < rgb.G && rgb.G < 70) && (30 < rgb.B && rgb.B < 70):
                        loot.Add(new StardewValley.Object("547", 1)); //helvite
                        break;


                    case Netcode.NetColor rgb when (140 < rgb.R && rgb.R < 173) && (201 < rgb.G && rgb.G < 234) && (69 < rgb.B && rgb.B < 107):
                        loot.Add(new StardewValley.Object("548", 1)); //jamborite
                        break;


                    case Netcode.NetColor rgb when (181 < rgb.R && rgb.R < 209) && (181 < rgb.G && rgb.G < 242) && (13 < rgb.B && rgb.B < 48):
                        loot.Add(new StardewValley.Object("549", 1)); //jagoite
                        break;


                    case Netcode.NetColor rgb when (196 < rgb.R && rgb.R < 224) && (206 < rgb.G && rgb.G < 234) && (204 < rgb.B && rgb.B < 224):
                        loot.Add(new StardewValley.Object("571", 1)); //limestone
                        break;


                    case Netcode.NetColor rgb when (178 < rgb.R && rgb.R < 210) && (178 < rgb.G && rgb.G < 196) && (153 < rgb.B && rgb.B < 181):
                        loot.Add(new StardewValley.Object("572", 1)); //soapstone
                        break;


                    case Netcode.NetColor rgb when (51 < rgb.R && rgb.R < 94) && (18 < rgb.G && rgb.G < 46) && (0 < rgb.B && rgb.B < 18):
                        loot.Add(new StardewValley.Object("572", 1)); //mudstone
                        break;


                    case Netcode.NetColor rgb when (25 < rgb.R && rgb.R < 100) && (5 < rgb.G && rgb.G < 40) && (53 < rgb.B && rgb.B < 110):
                        loot.Add(new StardewValley.Object("575", 1)); //obsidian
                        break;


                    case Netcode.NetColor rgb when (107 < rgb.R && rgb.R < 132) && (191 < rgb.G && rgb.G < 227) && (160 < rgb.B && rgb.B < 191):
                        loot.Add(new StardewValley.Object("576", 1)); //slate
                        break;



                    case Netcode.NetColor rgb when (48 < rgb.R && rgb.R < 71) && (36 < rgb.G && rgb.G < 61) && (115 < rgb.B && rgb.B < 166):
                        loot.Add(new StardewValley.Object("577", 1)); //fairy stone
                        break;


                    case Netcode.NetColor rgb when (196 < rgb.R && rgb.R < 227) && (13 < rgb.G && rgb.G < 97) && (194 < rgb.B && rgb.B < 217):
                        loot.Add(new StardewValley.Object("578", 1)); //star shard 
                        break;


                    case Netcode.NetColor rgb when (86 < rgb.R && rgb.R < 125) && (191 < rgb.G && rgb.G < 209) && (87 < rgb.B && rgb.B < 153):
                        loot.Add(new StardewValley.Object("538", 1)); //alamite
                        break;

                    default:
                        break;
                }

                Point center = slime.GetBoundingBox().Center;
                Vector2 dropPos = new Vector2(center.X, center.Y);
                loot.ForEach(mineral => { if (rng.Next(100) <= 8) { Game1.createItemDebris(mineral, dropPos, rng.Next(4), slime.currentLocation); }});

            }


        }




    }
}