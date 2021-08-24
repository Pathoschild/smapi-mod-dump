/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using HarmonyLib;
using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    public partial class ModEntry : Mod, IAssetLoader
    {
        /// <summary>A reference to this mod's current instance, allowing easier access to SMAPI utilites.</summary>
        internal static Mod Instance { get; set; } = null;

        /// <summary>The name/address of the asset used to store NPC exclusion settings.</summary>
        public static string AssetName { get; set; } = "Data/CustomNPCExclusions";

        /// <summary>Runs when SMAPI loads this mod.</summary>
        /// <param name="helper">This mod's API for most SMAPI features.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this; //set the reference to this mod's current instance
            Harmony harmony = new Harmony(this.ModManifest.UniqueID); //create a Harmony instance for this mod

            //apply all Harmony patches
            HarmonyPatch_ItemDeliveryQuest.ApplyPatch(harmony);
            HarmonyPatch_SocializeQuest.ApplyPatch(harmony);
            HarmonyPatch_WinterStarGifts.ApplyPatch(harmony);
            HarmonyPatch_ShopDialog.ApplyPatch(harmony);
            HarmonyPatch_IslandVisit.ApplyPatch(harmony, helper);
            HarmonyPatch_PerfectionFriendship.ApplyPatch(harmony);
            HarmonyPatch_MovieInvitation.ApplyPatch(harmony);
            HarmonyPatch_Greetings.ApplyPatch(harmony);
        }

        /// <summary>Get whether this mod can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(AssetName)) //if this asset's name matches
            {
                return true; //this mod can load this asset
            }

            return false; //this mod CANNOT load this asset
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(AssetName)) //if this asset's name matches
            {
                return (T)(object)new Dictionary<string, string>(); //return an empty string dictionary (i.e. create a new data file)
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}
