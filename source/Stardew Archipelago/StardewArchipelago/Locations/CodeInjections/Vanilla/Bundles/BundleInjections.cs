/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Bundles;
using StardewArchipelago.Constants;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using Bundle = StardewValley.Menus.Bundle;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Bundles
{
    public static class BundleInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static ArchipelagoStateDto _state;
        private static LocationChecker _locationChecker;
        private static BundlesManager _bundlesManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, ArchipelagoStateDto state, LocationChecker locationChecker, BundlesManager bundlesManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _state = state;
            _locationChecker = locationChecker;
            _bundlesManager = bundlesManager;
        }

        // public Bundle(int bundleIndex, string rawBundleInfo, bool[] completedIngredientsList, Point position, string textureName, JunimoNoteMenu menu)
        public static void BundleConstructor_GenerateBundleIngredients_Postfix(Bundle __instance, int bundleIndex, string rawBundleInfo,
            bool[] completedIngredientsList, Point position, string textureName, JunimoNoteMenu menu)
        {
            try
            {
                InitializeBundle(__instance, bundleIndex, rawBundleInfo, completedIngredientsList, textureName, menu);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(BundleConstructor_GenerateBundleIngredients_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void InitializeBundle(Bundle bundle, int bundleIndex, string rawBundleInfo, bool[] completedIngredientsList, string textureName, JunimoNoteMenu menu)
        {
            var rawBundleParts = rawBundleInfo.Split('/');
            var bundleName = rawBundleParts[0];
            var bundleDisplayName = $"{bundleName} Bundle";
            var bundleFromArchipelago = _bundlesManager.BundleRooms.BundlesByName[bundleDisplayName];
            if (bundleFromArchipelago is not ItemBundle itemBundle)
            {
                return;
            }
            
            bundle.name = bundleName;
            bundle.label = bundleName;
            bundle.rewardDescription = string.Empty;
            bundle.complete = true;

            var numberAlreadyDonated = 0;
            var ingredients = new List<BundleIngredientDescription>();
            for (var i = 0; i < itemBundle.Items.Count; i++)
            {
                var bundleItem = itemBundle.Items[i];
                var alreadyCompleted = completedIngredientsList[i];
                var ingredient = bundleItem.CreateBundleIngredientDescription(alreadyCompleted);
                ingredients.Add(ingredient);
                if (alreadyCompleted)
                {
                    ++numberAlreadyDonated;
                }
                else
                {
                    bundle.complete = false;
                }
            }

            bundle.ingredients = ingredients;
            bundle.bundleColor = itemBundle.ColorIndex;
            bundle.numberOfIngredientSlots = itemBundle.NumberRequired;

            if (numberAlreadyDonated >= bundle.numberOfIngredientSlots)
            {
                bundle.complete = true;
            }
        }
    }
}
