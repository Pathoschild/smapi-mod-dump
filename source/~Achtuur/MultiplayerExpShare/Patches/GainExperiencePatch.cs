/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
namespace MultiplayerExpShare.Patches
{
    public class GainExperiencePatch : GenericPatcher
    {
        private static IMonitor Monitor;
        /// <summary>
        /// Experience that each farmer has gained. Keys represent farmer id, float accumulated exp.
        /// </summary>
        private static IDictionary<int, float> FarmerExperience;


        public void Apply(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            FarmerExperience = new IDictionary<int, float>();
            harmony.Apply(
                original: this.getOriginalMethod<Farmer>(nameof(Farmer.gainExperience),
                prefix: this.getHarmonyMethod(nameof(Prefix_GainExperience))
            );
        }

        private static void Prefix_GainExperience(int which, ref int howMuch, Farmer __instance)
        {
            // Skip execution if world isnt loaded or farmer is not local player
            if (!Context.isWorldReady || !__instance.isLocalPlayer)
                return;

            Farmer[] nearbyFarmers = ModEntry.GetNearbyFarmers();

            float main_xp = (float)howMuch * ModConfig.ExpPercentageToActor();
            float nearby_xp = ((float)howMuch - main_xp) / nearbyFarmers.Length;

            foreach (Farmer farmer in nearbyFarmers)
            {
                int exp = GetIntExpToAdd(farmer, nearby_xp);
                farmer.gainExperience(which, exp);
            }

            howMuch = GetIntExpToAdd(Game1.player, main_xp);
        }

        /// <summary>
        /// Adds decimal part of <paramref name="exp_amount"/> to <see cref="FarmerExperience"/> and returns integer part
        /// </summary>
        /// <param name="farmer">Farmer that experience should be added to</param>
        /// <param name="exp_amount">Exp Amount to add</param>
        private int GetIntExpToAdd(Farmer farmer, float exp_amount)
        {
            // Create entry in dict if it doesnt exist yet
            if (!FarmerExperience.containsKey(farmer.uniqueMultiplayerID))
                FarmerExperience[farmer.uniqueMultiplayerID] = 0f;

            // add exp as float
            FarmerExperience[farmer.id] += exp_amount;

            // get xp rounded down to whole integer
            int xp_rounded_down = Math.Floor(FarmerExperience[farmer.id]);

            // subtract whole part of exp
            FarmerExperience[farmer.id] -= xp_rounded_down;

            return xp_rounded_down;
        }
    }
}
