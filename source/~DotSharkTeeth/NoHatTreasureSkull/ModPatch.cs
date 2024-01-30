/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System.Reflection;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using HarmonyLib;
using Netcode;
using System.Collections.Generic;

namespace NoHatTreasureSkull
{
    public partial class ModEntry
    {
        public enum ConfigItem {
            Bomb,
            Machine,
            Medicine,
            Sapling,
            Seed
        }
        public static void MineShaftGetTreasureRoomItem_postfix(ref Item __result)
        {
            
            if (!Config.Enable || __result.Category != -95)
                return;

            List<ConfigItem> configItems = GetConfigItems;
            if (configItems.Count == 0)
                return;

            StardewValley.Object newItem = null;

            switch(configItems[Game1.random.Next(configItems.Count)]) {
                case ConfigItem.Bomb:
                    newItem = new StardewValley.Object(Game1.random.Next(286, 288), Game1.random.Next(1, 5) * 5);
                    break;
                case ConfigItem.Machine:
                    (Vector2, int)[] itemObjects = new[]
                    {
                        (Vector2.Zero, 21), // Crystalarium
                        (Vector2.Zero, 25), // Seed Maker
                        (Vector2.Zero, 165), // Auto Grabber
                        (Vector2.Zero, 272) // Auto Petter
                    };
                    int randomIndex = Game1.random.Next(itemObjects.Length);
                    newItem = new StardewValley.Object(itemObjects[randomIndex].Item1, itemObjects[randomIndex].Item2);
                    break;
                case ConfigItem.Medicine:
                    int[] medicine = { 773, 349 };
                    newItem = new StardewValley.Object(medicine[Game1.random.Next(medicine.Length)], Game1.random.Next(2, 5));
                    break;
                case ConfigItem.Sapling:
                    newItem = new StardewValley.Object(Game1.random.Next(628, 634), 1);
                    break;
                case ConfigItem.Seed:
                    newItem = new StardewValley.Object(Game1.random.Next(472, 499), Game1.random.Next(1, 5) * 5);
                    break;
            }
            __result = newItem;

        }

       
        public static List<ConfigItem> GetConfigItems {
            get {
                List<ConfigItem> list = new List<ConfigItem>();
                if (Config.EnableBomb)
                    list.Add(ConfigItem.Bomb);
                if (Config.EnableMachine)
                    list.Add(ConfigItem.Machine);
                if (Config.EnableMedicine)
                    list.Add(ConfigItem.Medicine);
                if (Config.EnableSapling)
                    list.Add(ConfigItem.Sapling);
                if (Config.EnableSeed)
                    list.Add(ConfigItem.Seed);
                return list;
            }
        }

        // Always Treasure room
        public static void MineShaftaddLevelChests_prefix(MineShaft __instance)
        {
            SHelper.Reflection.GetField<NetBool>(__instance, "netIsTreasureRoom").GetValue().Value = true;
        }

    }
}
