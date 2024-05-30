/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Interfaces;
using CustomCompanions.Framework.Models;
using CustomCompanions.Framework.Models.Companion;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Managers
{
    internal static class RingManager
    {
        internal static List<RingModel> rings;

        internal static bool IsSummoningRing(Ring ring)
        {
            if (ring != null && rings.Any(r => r.Name == ring.Name))
            {
                return true;
            }
            else if (HandleContentPatcherRing(ring))
            {
                return true;
            }

            return false;
        }

        internal static bool HasSummoningRingEquipped(Farmer who)
        {
            return GetWornRings().Where(r => IsSummoningRing(r)).Any();
        }

        internal static IEnumerable<Ring> GetWornRings(bool filterVanillaRings = false)
        {
            var stack = new Stack<Ring>();
            stack.Push(Game1.player.leftRing.Value);
            stack.Push(Game1.player.rightRing.Value);
            while (stack.Count > 0) 
            {
                var ring = stack.Pop();
                if (ring is CombinedRing) 
                {
                    foreach (var cr in ((CombinedRing)ring).combinedRings) 
                    {
                        stack.Push(cr);
                    }
                }
                else if (ring != null) 
                {
                    if (!filterVanillaRings || IsSummoningRing(ring)) 
                    { 
                        yield return ring;
                    }
                }
            }
        }

        internal static void LoadWornRings()
        {
            foreach (Ring ring in GetWornRings(true))
            {
                HandleEquip(Game1.player, Game1.currentLocation, ring);
            }
        }

        private static bool HandleContentPatcherRing(Ring ring)
        {
            if (rings.Any(r => r.Name == ring.Name))
            {
                return true;
            }

            if (Game1.objectData.TryGetValue(ring.ItemId, out var data) is false || data is null || data.CustomFields is null)
            {
                return false;
            }

            if (data.CustomFields.TryGetValue("PeacefulEnd.CustomCompanions/Companions/Variety", out string rawVarietyCount) && int.TryParse(rawVarietyCount, out int actualVarietyCount))
            {
                Dictionary<string, CompanionData> companions = new Dictionary<string, CompanionData>();
                for (int i = 0; i < actualVarietyCount; i++)
                {
                    if (data.CustomFields.TryGetValue($"PeacefulEnd.CustomCompanions/Companions/{i}/Id", out string companionId))
                    {
                        if (data.CustomFields.TryGetValue($"PeacefulEnd.CustomCompanions/Companions/{i}/NumberToSummon", out string rawNumberToSummon) && int.TryParse(rawNumberToSummon, out int actualNumberToSummon))
                        {
                            companions[companionId] = new CompanionData() { NumberToSummon = actualNumberToSummon };
                        }
                    }
                }

                if (companions.Count > 0)
                {
                    rings.Add(new RingModel()
                    {
                        Name = ring.Name,
                        Companions = companions,
                        AddedViaContentPatcher = true
                    });

                    return true;
                }
            }

            return false;
        }

        internal static void HandleEquip(Farmer who, GameLocation location, Ring ring)
        {
            var summoningRing = rings.FirstOrDefault(r => r.Name == ring.Name);
            if (summoningRing is null)
            {
                CustomCompanions.monitor.Log($"Failed to find a summoning ring match to [{ring.Name}]");
                return;
            }
            var selectedCompanionData = summoningRing.Companions.ElementAt(Game1.random.Next(summoningRing.Companions.Count));

            CompanionModel companion;
            if (summoningRing.AddedViaContentPatcher is true)
            {
                companion = CompanionManager.companionModels.FirstOrDefault(c => c.GetId() == selectedCompanionData.Key);
            }
            else
            {
                companion = CompanionManager.companionModels.FirstOrDefault(c => c.Name == selectedCompanionData.Key && c.Owner == summoningRing.Owner);
            }

            if (companion is null)
            {
                CustomCompanions.monitor.Log($"Failed to find a companion match to [{selectedCompanionData}] for the summoning ring [{ring.Name}]");
                return;
            }

            // Create a new Companion and add it to the player's location
            CustomCompanions.monitor.Log($"Spawning [{selectedCompanionData}] x{selectedCompanionData.Value.NumberToSummon} via the summoning ring [{ring.Name}]");
            CompanionManager.SummonCompanions(companion, selectedCompanionData.Value.NumberToSummon, summoningRing, who, location);
        }

        internal static void HandleUnequip(Farmer who, GameLocation location, Ring ring)
        {
            var summoningRing = rings.FirstOrDefault(r => r.Name == ring.Name);
            if (summoningRing is null)
            {
                CustomCompanions.monitor.Log($"Failed to find a summoning ring match to [{ring.Name}]");
                return;
            }

            // Despawn the summoned companion(s) bound to this ring
            CustomCompanions.monitor.Log($"Despawning companions bound to the summoning ring [{ring.Name}]");
            CompanionManager.RemoveCompanions(summoningRing, location);
        }

        internal static void HandleNewLocation(Farmer who, GameLocation location, Ring ring)
        {
            var summoningRing = rings.FirstOrDefault(r => r.Name == ring.Name);
            if (summoningRing is null)
            {
                CustomCompanions.monitor.Log($"Failed to find a summoning ring match to [{ring.Name}]");
                return;
            }

            // Create a new Companion and add it to the player's location
            CustomCompanions.monitor.Log($"Respawning companions bound to the summoning ring [{ring.Name}]");
            CompanionManager.RespawnCompanions(summoningRing, who, location);
        }

        internal static void HandleLeaveLocation(Farmer who, GameLocation location, Ring ring)
        {
            var summoningRing = rings.FirstOrDefault(r => r.Name == ring.Name);
            if (summoningRing is null)
            {
                CustomCompanions.monitor.Log($"Failed to find a summoning ring match to [{ring.Name}]");
                return;
            }

            // Despawn the summoned companion(s) bound to this ring
            CustomCompanions.monitor.Log($"Despawning companions bound to the summoning ring [{ring.Name}]");
            CompanionManager.RemoveCompanions(summoningRing, location, false);
        }
    }
}
