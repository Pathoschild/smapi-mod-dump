/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Locations;
using System;
using System.Reflection;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewRoguelike.Extensions;
using HarmonyLib;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), nameof(MineShaft.GetMine))]
    internal class GetMinePatch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "The property is read-only.")]
        public static bool Prefix(ref MineShaft __result, string name)
        {
            // requestedFloor/requestedLevel
            string format = name["UndergroundMine".Length..];
            string floorString = format.Split("/")[0];
            string levelString = format.Split("/")[1];

            int requestedFloor = Convert.ToInt32(floorString);
            int requestedLevel = Convert.ToInt32(levelString);

            foreach (MineShaft mine in MineShaft.activeMines)
            {
                if (MineShaftLevel.get_MineShaftLevel(mine).Value == requestedLevel)
                {
                    __result = mine;
                    return false;
                }
            }

            MineShaft newMine = new(0);
            newMine.GetType().GetField("mineRandom", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(newMine, Roguelike.FloorRng);
            newMine.get_MineShaftLevel().Value = requestedLevel;
            newMine.get_MineShaftEntryTime().Value = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            if (Merchant.IsMerchantFloor(requestedLevel))
                Merchant.Initialize(newMine);
            else if (BossFloor.IsBossFloor(requestedLevel))
                BossFloor.Setup(newMine);
            else if (ForgeFloor.ShouldDoForgeFloor(requestedLevel))
                newMine.get_MineShaftForgeFloor().Value = true;
            else if (ChestFloor.ShouldDoChestFloor(requestedLevel))
            {
                newMine.get_MineShaftChestFloor().Value = true;
                ChestFloor.SpawnChest(newMine);
            }
            else if (ChallengeFloor.ShouldDoChallengeFloor(requestedLevel))
            {
                newMine.get_MineShaftIsChallengeFloor().Value = true;
                newMine.get_MineShaftChallengeFloor().Value = ChallengeFloor.GetRandomChallenge(requestedLevel);
            }

            double darkChance;
            if (DebugCommands.ForcedDarkChance > 0f)
                darkChance = DebugCommands.ForcedDarkChance;
            else
                darkChance = 0.2;

            if (Roguelike.HardMode && Roguelike.FloorRng.NextDouble() < darkChance && newMine.IsNormalFloor())
                newMine.set_MineShaftIsDarkArea(true);

            int depth = Roguelike.GetFloorDepth(newMine, requestedFloor);

            newMine.mineLevel = depth;
            newMine.name.Value = name;

            MineShaft.activeMines.Add(newMine);

            newMine.GetType().GetMethod("generateContents", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(newMine, null);

			__result = newMine;

            Game1.netWorldState.Value.MinesDifficulty = 0;
            Game1.netWorldState.Value.SkullCavesDifficulty = 0;

            if (newMine.get_MineShaftIsChallengeFloor().Value)
                newMine.get_MineShaftChallengeFloor().Value.Initialize(newMine);

            return false;
        }
    }
}
