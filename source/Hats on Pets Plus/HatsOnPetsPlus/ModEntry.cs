/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SymaLoernn/Stardew_HatsOnPetsPlus
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace HatsOnPetsPlus
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public const string modContentPath = "Syma.HatsOnPetsPlus/CustomPetData";
        IModHelper helper;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            helper.Events.Content.AssetReady += this.OnAssetReady;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Characters.Pet), nameof(StardewValley.Characters.Pet.drawHat)),
                prefix: new HarmonyMethod(typeof(PetHatsPatch), nameof(PetHatsPatch.DrawHatPrefix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Characters.Pet), nameof(StardewValley.Characters.Pet.checkAction)),
                postfix: new HarmonyMethod(typeof(PetHatsPatch), nameof(PetHatsPatch.CheckActionPostfix)));

            HOPPHelperFunctions.Initialize(this.Monitor, helper);

        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            HOPPHelperFunctions.Content_AssetRequested(e);
        }

        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            HOPPHelperFunctions.Content_AssetReady(e);
        }
    }
}