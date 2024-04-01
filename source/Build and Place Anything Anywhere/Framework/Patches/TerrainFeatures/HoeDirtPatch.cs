/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace AnythingAnywhere.Framework.Patches.TerrainFeatures
{
    internal class HoeDirtPatch : PatchTemplate
    {
        private readonly Type _object = typeof(HoeDirt);

        internal HoeDirtPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }
        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(HoeDirt.plant),  new[] { typeof(string), typeof(Farmer), typeof(bool) } ), prefix: new HarmonyMethod(GetType(), nameof(PlantPrefix)));
        }

        public static bool PlantPrefix(HoeDirt __instance, string itemId, Farmer who, bool isFertilizer, ref bool __result)
        {
            if (!ModEntry.modConfig.EnablePlanting)
                return true;


            GameLocation location = __instance.Location;
            if (isFertilizer)
            {
                if (!__instance.CanApplyFertilizer(itemId))
                {
                    __result = false;
                    return false;
                }
                __instance.fertilizer.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
                __instance.applySpeedIncreases(who);
                location.playSound("dirtyHit");
                __result = true;
                return false;
            }
            Season season = location.GetSeason();
            Point tilePos = Utility.Vector2ToPoint(__instance.Tile);
            itemId = Crop.ResolveSeedId(itemId, location);
            if (!Crop.TryGetData(itemId, out var cropData) || cropData.Seasons.Count == 0)
            {
                __result = false;
                return false;
            }
            StardewValley.Object obj;
            bool isGardenPot = location.objects.TryGetValue(__instance.Tile, out obj) && obj is IndoorPot;
            bool isIndoorPot = isGardenPot && !location.IsOutdoors;
            location.GetData().CanPlantHere = true;
            if (!who.currentLocation.CheckItemPlantRules(itemId, isGardenPot, isIndoorPot || (location.GetData()?.CanPlantHere ?? location.IsFarm), out var deniedMessage))
            {
                if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
                {
                    if (deniedMessage == null && location.NameOrUniqueName != "Farm")
                    {
                        Farm farm = Game1.getFarm();
                        if (farm.CheckItemPlantRules(itemId, isGardenPot, farm.GetData()?.CanPlantHere ?? true, out var _))
                        {
                            deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919");
                        }
                    }
                    if (deniedMessage == null)
                    {
                        deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13925");
                    }
                    Game1.showRedMessage(deniedMessage);
                }
                __result = false;
                return false;
            }
            if (!isIndoorPot && !who.currentLocation.CanPlantSeedsHere(itemId, tilePos.X, tilePos.Y, isGardenPot, out deniedMessage))
            {
                if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
                {
                    if (deniedMessage == null)
                    {
                        deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13925");
                    }
                    Game1.showRedMessage(deniedMessage);
                }
                __result = false;
                return false;
            }
            if (isIndoorPot || location.SeedsIgnoreSeasonsHere() || !((!(cropData.Seasons?.Contains(season))) ?? true))
            {
                __instance.crop = new Crop(itemId, tilePos.X, tilePos.Y, __instance.Location);
                if ((bool)__instance.crop.raisedSeeds.Value)
                {
                    location.playSound("stoneStep");
                }
                location.playSound("dirtyHit");
                Game1.stats.SeedsSown++;
                __instance.applySpeedIncreases(who);
                __instance.nearWaterForPaddy.Value = -1;
                if (__instance.hasPaddyCrop() && __instance.paddyWaterCheck())
                {
                    __instance.state.Value = 1;
                    __instance.updateNeighbors();
                }
                __result = true;
                return false;
            }
            if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
            {
                string errorKey = (((!(cropData.Seasons?.Contains(season))) ?? false) ? "Strings\\StringsFromCSFiles:HoeDirt.cs.13924" : "Strings\\StringsFromCSFiles:HoeDirt.cs.13925");
                Game1.showRedMessage(Game1.content.LoadString(errorKey));
            }
            __result = false;
            return false;
        }
    }
}
