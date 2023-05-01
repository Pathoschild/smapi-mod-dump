/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;
using HarmonyLib;
using System.Linq;
using System.Reflection.Emit;

namespace BNWCore
{
    public class Seed_Chance_Patches
    {
        public partial class EnumerablePatcher
        {
            [HarmonyPatch(typeof(Crop), nameof(Crop.harvest))]
            public class Crop_harvest_Patch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; i++)
                    {
                        if (codes[i].opcode == OpCodes.Ldloc_0 && codes[i + 1].opcode == OpCodes.Brfalse)
                        {
                            codes.Insert(i + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Seed_Chance_Patches), nameof(Seed_Chance_Patches.Harvest))));
                            codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_2));
                            codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_1));
                            codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_0));
                            break;
                        }
                    }
                    return codes.AsEnumerable();
                }
            }
        }
        public static void Harvest(Crop crop, int xTile, int yTile)
        {
            if (!(Game1.MasterPlayer.mailReceived.Contains("earth_farming_blessing")) || crop.dead.Value || crop.netSeedIndex.Value == crop.indexOfHarvest.Value || (!Seed_Config.RegrowableSeeds && crop.regrowAfterHarvest.Value > -1) || Game1.random.NextDouble() > Seed_Config.SeedChance / 100f)
                return;
            int index;
            if (crop.forageCrop.Value)
            {
                switch (Game1.currentSeason)
                {
                    case "summer":
                        index = 496;
                        break;
                    case "fall":
                        index = 497;
                        break;
                    case "winter":
                        index = 498;
                        break;
                    default:
                        index = 495;
                        break;
                }
            }
            else
            {
                index = crop.netSeedIndex.Value;
                if (index == -1)
                {
                    Dictionary<int, string> cropData = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
                    foreach (int key in cropData.Keys)
                    {
                        if (Convert.ToInt32(cropData[key].Split('/', StringSplitOptions.None)[3]) == crop.indexOfHarvest.Value)
                        {
                            index = key;
                        }
                    }
                }
            }
            if (index == -1)
                return;
            int amount = Game1.random.Next(Seed_Config.MinSeeds, Seed_Config.MaxSeeds + 1);
            if (amount <= 0)
                return;
            Game1.createItemDebris(new Object(index, amount), new Vector2((float)(xTile * 64 + 32), (float)(yTile * 64 + 32)), -1, null, -1);
        }
    }
}