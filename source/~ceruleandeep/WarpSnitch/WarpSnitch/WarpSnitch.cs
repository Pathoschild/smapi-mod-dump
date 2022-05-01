/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Harmony;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Tools;
using xTile.ObjectModel;
using xTile.Tiles;

namespace WarpSnitch
{
    /// <summary>The mod entry point.</summary>
    public class WarpSnitch : Mod
    {
        public static IMonitor SMonitor;
        
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SMonitor = Monitor;

            HarmonyInstance harmony = HarmonyInstance.Create("ceruleandeep.warpsnitch");
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "loadEndOfRouteBehavior"),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(NPC_loadEndOfRouteBehavior_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "warpCharacter", parameters: new Type[] { typeof(NPC), typeof(GameLocation), typeof(Vector2) }),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(Game1_warpCharacter_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "warpCharacter", parameters: new Type[] { typeof(NPC), typeof(string), typeof(Vector2) }),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(Game1_warpCharacter_String_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), "draw"),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(FishingRod_Draw_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), "HasUnresolvedNeeds"),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(FishPond_HasUnresolvedNeeds_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(PathFindController), "isPositionImpassableForNPCSchedule"),
                prefix: new HarmonyMethod(typeof(WarpSnitch), nameof(PathFindController_isPositionImpassableForNPCSchedule_Prefix))
            );
        }
        
        // System.NullReferenceException: Object reference not set to an instance of an object
        //     at xTile.ObjectModel.PropertyValue.ToString () <0x124184800 + 0x00012> in <filename unknown>:0 
        // at StardewValley.PathFindController.isPositionImpassableForNPCSchedule (StardewValley.GameLocation loc, Int32 x, Int32 y) <0x13d750220 + 0x00157> in <filename unknown>:0 
        // at StardewValley.PathFindController.findPathForNPCSchedules (Point startPoint, Point endPoint, StardewValley.GameLocation location, Int32 limit) <0x13d74d340 + 0x0056d> in <filename unknown>:0 
        // at StardewValley.NPC.pathfindToNextScheduleLocation (System.String startingLocation, Int32 startingX, Int32 startingY, System.String endingLocation, Int32 endingX, Int32 endingY, Int32 finalFacingDirection, System.String endBehavior, System.String endMessage) <0x13d74ccd0 + 0x00359> in <filename unknown>:0 
        // at (wrapper dynamic-method) StardewValley.NPC:StardewValley.NPC.parseMasterSchedule_PatchedBy<SMAPI> (object,string)
        //     at (wrapper managed-to-native) System.Reflection.MonoMethod:InternalInvoke (System.Reflection.MonoMethod,object,object[],System.Exception&)
        // at System.Reflection.MonoMethod.Invoke (System.Object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) <0x1107b24e0 + 0x000bb> in <filename unknown>:0 

        [HarmonyPriority(Priority.High)]
        public static bool PathFindController_isPositionImpassableForNPCSchedule_Prefix(PathFindController __instance, GameLocation loc, int x, int y, ref bool __result)
        {
            SMonitor.Log($"isPositionImpassableForNPCSchedule: Location: {loc.name} [{x} {y}]", LogLevel.Trace);
            
            Tile tile = loc.Map.GetLayer("Buildings").Tiles[x, y];
            if (tile != null && tile.TileIndex != -1)
            {
                PropertyValue property = null;
                string value2 = null;
                tile.TileIndexProperties.TryGetValue("Action", out property);
                if (property == null)
                {
                    tile.Properties.TryGetValue("Action", out property);
                }

                if (property != null)
                {
                    try
                    {
                        value2 = property.ToString();
                    }
                    catch (Exception e)
                    {
                        SMonitor.Log($"isPositionImpassableForNPCSchedule: Location: {loc.name} [{x} {y}]",
                            LogLevel.Warn);
                        SMonitor.Log($"Tile's Action property cannot be converted to string", LogLevel.Warn);
                        SMonitor.Log($"{e}", LogLevel.Error);
                        SMonitor.Log($"trying to recover by telling SVE that this tile is impassable to NPCs", LogLevel.Warn);
                        __result = false;
                        return false;
                    }
                }
            }

            return true;
        }

        // [SMAPI] An error occurred in the overridden draw loop: System.NullReferenceException: Object reference not set to an instance of an object
        //   at StardewValley.Buildings.FishPond._GetNeededItemData () <0x14a7a5710 + 0x0008c> in <filename unknown>:0 
        //   at StardewValley.Buildings.FishPond.HasUnresolvedNeeds () <0x14a7a55c0 + 0x0003b> in <filename unknown>:0 
        //   at StardewValley.Buildings.FishPond.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x14a79f2b0 + 0x03c0e> in <filename unknown>:0 
        //   at StardewValley.Locations.BuildableGameLocation.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x14a714630 + 0x00447> in <filename unknown>:0 
        //   at StardewValley.Farm.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x14a712e70 + 0x0009e> in <filename unknown>:0 
        //   at StardewModdingAPI.Framework.SGame.DrawImpl (Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.RenderTarget2D target_screen) <0x1400d8000 + 0x07235> in <filename unknown>:0 
        //   at StardewModdingAPI.Framework.SGame._draw (Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.RenderTarget2D target_screen) <0x1400d07f0 + 0x00052> in <filename unknown>:0 

        [HarmonyPriority(Priority.High)]
        public static bool FishPond_HasUnresolvedNeeds_Prefix(FishPond __instance, ref bool __result)
        {
            if (__instance.currentOccupants.Value < (int) __instance.maxOccupants) return true;
            FishPondData f = __instance.GetFishPondData();
            if (f is null)
            {
                SMonitor.Log($"FishPond.HasUnresolvedNeeds: GetFishPondData did not return data", LogLevel.Warn);
                SMonitor.Log($"FishPond.HasUnresolvedNeeds: trying to recover by telling SDV the pond has no neededItem or unresolved needs", LogLevel.Warn);
                __instance.neededItem.Value = null;
                __result = false;
                return false;
            }

            return true;
        }
        
        // [game] An error occurred in the base update loop: System.NullReferenceException: Object reference not set to an instance of an object
        // at StardewValley.Buildings.FishPond.dayUpdate (Int32 dayOfMonth) <0x12d0facc0 + 0x00322> in <filename unknown>:0 
        // at StardewValley.Locations.BuildableGameLocation.DayUpdate (Int32 dayOfMonth) <0x12d0f9d70 + 0x00072> in <filename unknown>:0 
        // at StardewValley.Farm.DayUpdate (Int32 dayOfMonth) <0x12d0f6e70 + 0x001a3> in <filename unknown>:0 
        // at StardewValley.Game1+<_newDayAfterFade>d__715.MoveNext () <0x12d0ddd40 + 0x06e11> in <filename unknown>:0 
        // at StardewValley.Game1+<>c__DisplayClass713_0.<newDayAfterFade>b__0 () <0x12d0dce00 + 0x0004a> in <filename unknown>:0 
        // at StardewModdingAPI.Framework.SModHooks.OnGame1_NewDayAfterFade (System.Action action) <0x12d0daf10 + 0x00031> in <filename unknown>:0 
        // at StardewValley.Game1.newDayAfterFade (System.Action after) <0x12d0dab70 + 0x0034e> in <filename unknown>:0 
        // at StardewValley.Game1.onFadeToBlackComplete () <0x12b498810 + 0x01718> in <filename unknown>:0 
        // at StardewValley.BellsAndWhistles.ScreenFade.UpdateFade (Microsoft.Xna.Framework.GameTime time) <0x12778ecd0 + 0x000b0> in <filename unknown>:0 
        // at StardewValley.Game1.UpdateOther (Microsoft.Xna.Framework.GameTime time) <0x12778c7a0 + 0x000ab> in <filename unknown>:0 
        // at StardewValley.Game1._update (Microsoft.Xna.Framework.GameTime gameTime) <0x11d3f9670 + 0x046fd> in <filename unknown>:0 
        // at StardewValley.Game1.Update (Microsoft.Xna.Framework.GameTime gameTime) <0x11d3f82c0 + 0x00178> in <filename unknown>:0 
        // at StardewModdingAPI.Framework.SGame.<>n__0 (Microsoft.Xna.Framework.GameTime gameTime) <0x11d3f8290 + 0x00019> in <filename unknown>:0 
        // at StardewModdingAPI.Framework.SGame+<>c__DisplayClass40_0.<Update>b__0 () <0x11d3f8250 + 0x0001f> in <filename unknown>:0 
        // at StardewModdingAPI.Framework.SCore.OnPlayerInstanceUpdating (StardewModdingAPI.Framework.SGame instance, Microsoft.Xna.Framework.GameTime gameTime, System.Action runUpdate) <0x11d26f730 + 0x052f4> in <filename unknown>:0 

        [HarmonyPriority(Priority.High)]
        public static bool FishPond_dayUpdate_Prefix(FishPond __instance, ref int ___whichFish)
        {
            SMonitor.Log($"FishPond.dayUpdate: whichFish: {___whichFish}");
            
            Dictionary<string, string> animationDescriptions = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
            if (! Game1.objectInformation.ContainsKey(___whichFish))
            {
                SMonitor.Log($"FishingRod.draw: whichFish does not exist in objectInformation", LogLevel.Warn);
                ___whichFish = Game1.objectInformation.Keys.First();
                SMonitor.Log($"FishingRod.draw: trying to recover by setting whichFish to {___whichFish}", LogLevel.Warn);
            }
            return true;
        }
        
        
        // [SMAPI] An error occurred in the overridden draw loop: System.Collections.Generic.KeyNotFoundException: The given key was not present in the dictionary.
        // at System.Collections.Generic.Dictionary`2[TKey,TValue].get_Item (System.Collections.Generic.TKey key) <0x110c9e830 + 0x00069> in <filename unknown>:0 
        // at StardewValley.Tools.FishingRod.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x1311f1a90 + 0x0736a> in <filename unknown>:0 
        // at StardewValley.Game1.drawTool (StardewValley.Farmer f, Int32 currentToolIndex) <0x131400000 + 0x008be> in <filename unknown>:0 
        // at StardewValley.Game1.drawTool (StardewValley.Farmer f) <0x1311f1a30 + 0x00041> in <filename unknown>:0 
        // at StardewValley.Farmer.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x12d3c8d20 + 0x01aea> in <filename unknown>:0 
        // at StardewValley.GameLocation.drawFarmers (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x12d3c8b90 + 0x0010c> in <filename unknown>:0 
        // at StardewValley.GameLocation.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x12d3c76b0 + 0x00731> in <filename unknown>:0 
        // at StardewValley.Locations.BuildableGameLocation.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x12e9e3bb0 + 0x0006e> in <filename unknown>:0 
        // at StardewValley.Farm.draw (Microsoft.Xna.Framework.Graphics.SpriteBatch b) <0x12e9e23e0 + 0x0009e> in <filename unknown>:0 
        // at StardewModdingAPI.Framework.SGame.DrawImpl (Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.RenderTarget2D target_screen) <0x121861000 + 0x07235> in <filename unknown>:0 
        // at StardewModdingAPI.Framework.SGame._draw (Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.RenderTarget2D target_screen) <0x121854a30 + 0x00052> in <filename unknown>:0 

        [HarmonyPriority(Priority.High)]
        public static bool FishingRod_Draw_Prefix(FishingRod __instance, ref string ___itemCategory, ref int ___whichFish)
        {
            if (___itemCategory == "Object" && ! Game1.objectInformation.ContainsKey(___whichFish))
            {
                SMonitor.Log($"FishingRod.draw: whichFish {___whichFish} does not exist in objectInformation", LogLevel.Warn);
                ___whichFish = Game1.objectInformation.Keys.First();
                SMonitor.Log($"FishingRod.draw: trying to recover by setting whichFish to {___whichFish}", LogLevel.Warn);
            }
            return true;
        }

        
        // An error occurred in the base update loop: System.FormatException: Input string was not in a correct format.
        //     at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)
        // at System.Number.ParseInt32(String s, NumberStyles style, NumberFormatInfo info)
        // at StardewValley.Utility.parseStringToIntArray(String s, Char delimiter)
        // at StardewValley.NPC.loadEndOfRouteBehavior(String name)
        // at StardewValley.NPC.getRouteEndBehaviorFunction(String behaviorName, String endMessage)
        // at StardewValley.NPC.checkSchedule(Int32 timeOfDay)
        
        [HarmonyPriority(Priority.High)]
        public static bool NPC_loadEndOfRouteBehavior_Prefix(NPC __instance, string name)
        {
            SMonitor.Log($"loadEndOfRouteBehavior: NPC: {__instance.displayName}, route name {name}");

            if (name is null)
            {
                SMonitor.Log($"loadEndOfRouteBehavior: route name is null [NPC: {__instance.displayName}]", LogLevel.Warn);
                return false;
            }

            Dictionary<string, string> animationDescriptions = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
            if (animationDescriptions.ContainsKey(name))
            {
                SMonitor.Log($"loadEndOfRouteBehavior: animationDescription {animationDescriptions[name]} [NPC: {__instance.displayName}]", LogLevel.Trace);

                string[] rawData = animationDescriptions[name].Split('/');

                SMonitor.Log($"loadEndOfRouteBehavior: rawdata length {rawData.Count()} [NPC: {__instance.displayName}] (needs to be at least 3)", LogLevel.Trace);

                try
                {
                    Utility.parseStringToIntArray(rawData[0]);
                } catch
                {
                    SMonitor.Log($"loadEndOfRouteBehavior: could not parse routeEndIntro [NPC: {__instance.displayName}]", LogLevel.Warn);
                    SMonitor.Log($"the faulty int array is: '{rawData[0]}'", LogLevel.Warn);
                    return false;
                }
                try
                {
                    Utility.parseStringToIntArray(rawData[1]);
                } catch
                {
                    SMonitor.Log($"loadEndOfRouteBehavior: could not parse routeEndAnimation [NPC: {__instance.displayName}]", LogLevel.Warn);
                    SMonitor.Log($"the faulty int array is: '{rawData[1]}'", LogLevel.Warn);
                    return false;
                }
                try
                {
                    Utility.parseStringToIntArray(rawData[2]);
                } catch
                {
                    SMonitor.Log($"loadEndOfRouteBehavior: could not parse routeEndOutro [NPC: {__instance.displayName}]", LogLevel.Warn);
                    SMonitor.Log($"the faulty int array is: '{rawData[2]}'", LogLevel.Warn);
                    return false;
                }
            }
            return true;
        }
        
        [HarmonyPriority(Priority.High)]
        public static bool Game1_warpCharacter_String_Prefix(Game1 __instance, NPC character, string targetLocationName)
        {
            if (character is null)
            {
                SMonitor.Log($"warpCharacter: character is null", LogLevel.Warn);
                return false;
            }

            if (targetLocationName is null)
            {
                SMonitor.Log($"warpCharacter: {character.displayName}'s targetLocationName is null", LogLevel.Warn);
                return false;
            }

            if (Game1.getLocationFromName(targetLocationName) is null)
            {
                SMonitor.Log($"warpCharacter: getLocationFromName returns null for {targetLocationName} [NPC: {character.displayName}]", LogLevel.Warn);
                return false;
            }
            return true;
        }
        
        [HarmonyPriority(Priority.High)]
        public static bool Game1_warpCharacter_Prefix(Game1 __instance, NPC character, GameLocation targetLocation)
        {
            if (character is null)
            {
                SMonitor.Log($"warpCharacter: character is null", LogLevel.Warn);
                return false;
            }

            if (targetLocation is null)
            {
                SMonitor.Log($"warpCharacter: {character.displayName}'s targetLocation is null", LogLevel.Warn);
                return false;
            }
            
            SMonitor.Log($"Warping {character.displayName} to {targetLocation.Name}", LogLevel.Trace);
            return true;
        }
    }
}