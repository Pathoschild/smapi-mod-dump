/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Objects;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace BetterJunimos.Utils {
    public class Util {
        private const int UnpaidRadius = 3;
        public const int CoffeeId = 433;

        private const int GemCategory = -2;
        private const int MineralCategory = -12;

        public const int ForageCategory = -81;
        public const int FlowerCategory = -80;
        public const int FruitCategory = -79;
        public const int WineCategory = -26;
        
        internal static IReflectionHelper Reflection;
        internal static JunimoAbilities Abilities;
        internal static JunimoPayments Payments;
        internal static JunimoProgression Progression;
        internal static JunimoGreenhouse Greenhouse;

        public static List<GameLocation> GetAllFarms() {
            return Game1.locations
                //.Where(loc => loc.IsFarm && loc.IsOutdoors)
                .ToList();
        }

        public static int CurrentWorkingRadius {
            get {
                if (!BetterJunimos.Config.JunimoPayment.WorkForWages) return BetterJunimos.Config.JunimoHuts.MaxRadius;
                if (Payments.WereJunimosPaidToday) return BetterJunimos.Config.JunimoHuts.MaxRadius;
                return UnpaidRadius;
            }
        }

        public static List<JunimoHut> GetAllHuts() {
            return GetAllFarms().SelectMany(farm => farm.buildings.OfType<JunimoHut>().ToList()).ToList();
        }

        public static Guid GetHutIdFromHut(JunimoHut hut) {
            return GetAllFarms().Select(farm => farm.buildings.GuidOf(hut)).ToList().Find(guid => guid != Guid.Empty);
        }

        public static JunimoHut GetHutFromId(Guid id) {
            foreach (var farm in GetAllFarms()) {
                if (farm.buildings.TryGetValue(id, out var hut)) {
                    return hut as JunimoHut;
                }
            }
            
            BetterJunimos.SMonitor.Log($"Could not get hut from id ${id}", LogLevel.Error);
            return null;
        }

        public static void AddItemToChest(GameLocation farm, Chest chest, SObject item) {
            Item obj = chest.addItem(item);
            if (obj == null)
                return;
            Vector2 pos = chest.TileLocation;
            for (int index = 0; index < obj.Stack; ++index)
                Game1.createObjectDebris(item.ItemId, (int) pos.X + 1, (int) pos.Y + 1, -1, item.Quality, 1f,
                    farm);
        }

        public static void RemoveItemFromChest(Chest chest, Item item, int count = 1) {
            if (BetterJunimos.Config.FunChanges.InfiniteJunimoInventory) {
                return;
            }

            item.Stack -= count;
            if (item.Stack <= 0) {
                chest.Items.Remove(item);
            }
        }

        public static void SpawnJunimoAtHut(JunimoHut hut) {
            // I don't know why we're multiplying by 64 here
            var pos = new Vector2((float) hut.tileX.Value + 1, (float) hut.tileY.Value + 1) * 64f + new Vector2(0.0f, 32f);
            SpawnJunimoAtPosition(Game1.player.currentLocation, pos, hut, hut.getUnusedJunimoNumber());
        }

        public static void SpawnJunimoAtPosition(GameLocation location, Vector2 pos, JunimoHut hut, int junimoNumber) {
            if (hut == null) {
                // BetterJunimos.SMonitor.Log($"SpawnJunimoAtPosition: hut is null", LogLevel.Warn);    
                return;
            }
            
            /*
             * Added by Mizzion. This will set the color of the junimos based on what gem is inside the hut.
             */
            var isPrismatic = false;
            var gemColor = GetGemColor(ref isPrismatic, hut); 
            /*
             * End added By Mizzion
             */

            // BetterJunimos.SMonitor.Log($"SpawnJunimoAtPosition: spawning #{junimoNumber} in {location.Name} at [{pos.X} {pos.Y}]", LogLevel.Debug);

            var junimoHarvester = new JunimoHarvester(location, pos, hut, junimoNumber, gemColor);

            // the JunimoHarvester constructor sets the location to Farm and calls pathfindToRandomSpotAroundHut immediately
            // so we have to set the location explicitly then re-do pathfinding
            if (!location.Equals(Game1.getFarm())) {
                // BetterJunimos.SMonitor.Log($"SpawnJunimoAtPosition: forcing #{junimoNumber} to {location.Name} at [{pos.X} {pos.Y}]", LogLevel.Trace);

                Reflection.GetField<bool>(junimoHarvester, "destroy").SetValue(false);
                junimoHarvester.currentLocation = location;
                junimoHarvester.Position = pos;
                junimoHarvester.pathfindToRandomSpotAroundHut();
            }
            
            
            // BetterJunimos.SMonitor.Log($"SpawnJunimoAtPosition: spawned #{junimoNumber} " +
            //                            $"in {junimoHarvester.currentLocation.Name} " +
            //                            $"at [{junimoHarvester.getTileX()} {junimoHarvester.getTileX()}]", LogLevel.Debug);

            junimoHarvester.isPrismatic.Value = isPrismatic; //Added by Mizzion, Fixes the Prismatic Junimos.
            location.characters.Add(junimoHarvester);
            hut.myJunimos.Add(junimoHarvester);
            
            if (Game1.isRaining) {
                var alpha = Reflection.GetField<float>(junimoHarvester, "alpha");
                alpha.SetValue(BetterJunimos.Config.FunChanges.RainyJunimoSpiritFactor);
            }

            // var destroy = Reflection.GetField<bool>(junimoHarvester, "destroy").GetValue();
            // var onscreen = Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, location);
            //
            // BetterJunimos.SMonitor.Log($"SpawnJunimoAtPosition: #{junimoNumber} general situation " +
            //                            $"destroy: {destroy} " +
            //                            $"isOnScreen: {onscreen} " +
            //                            $"controller: {junimoHarvester.controller is not null} " +
            //                            $"pathToEndPoint: {junimoHarvester.controller?.pathToEndPoint is not null} " +
            //                            $"at [{junimoHarvester.getTileX()} {junimoHarvester.getTileX()}]", LogLevel.Debug);

            if (!Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, location)) return;
            location.playSound("junimoMeep1");
        }

/*
 * Added by Mizzion. This method is used to get the gem color, so the junimos can be colored
 * I ripped this from SDV and edited it to work with this mod.
*/
        private static Color? GetGemColor(ref bool isPrismatic, JunimoHut hut) {
            var colorList = new List<Color>();
            var chest = hut.GetOutputChest();
            foreach (Item dyeObject in chest.Items) {
                if (dyeObject != null &&
                    (dyeObject.Category == MineralCategory || dyeObject.Category == GemCategory)) {
                    Color? dyeColor = TailoringMenu.GetDyeColor(dyeObject);
                    if (dyeObject.Name == "Prismatic Shard")
                        isPrismatic = true;
                    if (dyeColor.HasValue)
                        colorList.Add(dyeColor.Value);
                }
            }

            if (colorList.Count > 0)
                return colorList[Game1.random.Next(colorList.Count)];
            return new Color?();
        }

        public static void SendMessage(string msg) {
            if (!BetterJunimos.Config.Other.ReceiveMessages) return;

            Game1.addHUDMessage(new HUDMessage(msg, 3) {
                noIcon = true,
                timeLeft = HUDMessage.defaultTime
            });

            // try {
            //     var multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            //     multiplayer.broadcastGlobalMessage("Strings\\StringsFromCSFiles:"+msg);
            // }
            // catch (InvalidOperationException) {
            //     BetterJunimos.SMonitor.Log($"SendMessage: multiplayer unavailable", LogLevel.Error);
            // }
        }

        public static void SpawnParticles(Vector2 pos) {
            Multiplayer multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[] {
                new TemporaryAnimatedSprite(17, new Vector2(pos.X * 64f, pos.Y * 64f), Color.White, 7,
                    Game1.random.NextDouble() < 0.5, 125f)
            });
            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[] {
                new TemporaryAnimatedSprite(14, new Vector2(pos.X * 64f, pos.Y * 64f), Color.White, 7,
                    Game1.random.NextDouble() < 0.5, 50f)
            });
        }

        internal static int ExperienceForCrop(Crop crop) {
            if (crop.forageCrop.Value) {
                return 3;
            }

            var ioh = crop.indexOfHarvest.Value;
            var oi = Game1.objectData[ioh];
            var num = Math.Round((float)(16.0 * Math.Log(0.018 * (double) oi.Price + 1.0, Math.E)));
            var int32 = Convert.ToInt32(num);

            return int32;
        }
    }
}