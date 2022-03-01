/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ExpertModeBeachMap
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Locations;
    using StardewValley.TerrainFeatures;
    using System;
    using xTile.Dimensions;
    using StardewObject = StardewValley.Object;

    public class ExpertModeBeachMap : Mod
    {
        private static ExpertModeBeachMap mod;

        public override void Entry(IModHelper helper)
        {
            mod = this;

            //config = Helper.ReadConfig<ExpertModeBeachMapConfig>();
            //ExpertModeBeachMapXPConfig.VerifyConfigValues(config, this);
            //Helper.Events.GameLoop.GameLaunched += delegate { ExpertModeBeachMapConfig.SetUpModConfigMenu(config, this); };

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
               prefix: new HarmonyMethod(typeof(ExpertModeBeachMap), nameof(ExpertModeBeachMap.DontGrowWithoutFertilizerInSand)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.ApplySprinkler)),
               prefix: new HarmonyMethod(typeof(ExpertModeBeachMap), nameof(ExpertModeBeachMap.ApplySprinkler_Pre)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.ApplySprinkler)),
               postfix: new HarmonyMethod(typeof(ExpertModeBeachMap), nameof(ExpertModeBeachMap.ApplySprinkler_Post)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.placementAction)),
               prefix: new HarmonyMethod(typeof(ExpertModeBeachMap), nameof(ExpertModeBeachMap.PlacementAction_Pre)));
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.placementAction)),
               postfix: new HarmonyMethod(typeof(ExpertModeBeachMap), nameof(ExpertModeBeachMap.PlacementAction_Post)));
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        // Pirates4
        // base.setTileProperty(23, 8, "Buildings", "Action", "MessageSpeech Pirates4");

        // TODO mussel rocks
        // TODO destructable driftwood piles

        //"FarmImprovement_Guy_1": "Pssst... hey, mainlander. Listen up. If you've got coin... me and my boys can improve your farm. Interested?",
        //"FarmImprovement_Guy_2": "Tell me more...",
        //"FarmImprovement_Guy_3": "No, thanks.",
        //"FarmImprovement_Guy_4": "I can get my boys to add a little extra space on your farm. It'll just be... 500,000g. What do you say?",
        //"FarmImprovement_Guy_5": "I can get my boys to improve the sand, allowing you to use sprinklers.  I'll just need 100,000g. What do you say?",
        //"FarmImprovement_Guy_6": "Heh heh heh... perfect. You can expect the changes tomorrow morning.",

        public static bool HasUnlockedSprinklersInSand
        {
            get
            {
                return Game1.MasterPlayer.modData.ContainsKey($"{mod.ModManifest.UniqueID}/HasUnlockedSprinklersInSand");
            }
            set
            {
                Game1.MasterPlayer.modData[$"{mod.ModManifest.UniqueID}/HasUnlockedSprinklersInSand"] = "true";
            }
        }

        // TODO
        public static bool performAction(IslandSouthEastCave __instance, string action, Farmer who, Location tileLocation)
        {
            if (action != null && who.IsLocalPlayer)
            {
                string a = action.Split(' ', StringSplitOptions.None)[0];
                string b = action.Split(' ', StringSplitOptions.None)[1];
                if (a == "MessageSpeech" && b == "Pirates6")
                {
                    var question_prompt = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_0");

                    __instance.createQuestionDialogue(question_prompt, __instance.createYesNoResponses(), "GoldenBeachFarmImprovement");
                }
            }

            return true;
        }

        public static bool ApplySprinkler_Pre(StardewObject __instance, GameLocation location, Vector2 tile, ref bool __state)
        {
            try
            {
                __state = false;

                DisableSprinklerFlag_Pre(__instance, location, (int)tile.X, (int)tile.Y, ref __state);

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void ApplySprinkler_Post(GameLocation location, Vector2 tile, ref bool __state)
        {
            try
            {
                if (__state)
                {
                    ReenableSprinklerFlag_Post(location, (int)tile.X, (int)tile.Y);
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool PlacementAction_Pre(StardewObject __instance, GameLocation location, int x, int y, ref bool __state)
        {
            try
            {
                __state = false;
                var placementTile = new Vector2(x / 64, y / 64);

                DisableSprinklerFlag_Pre(__instance, location, (int)placementTile.X, (int)placementTile.Y, ref __state);

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void PlacementAction_Post(GameLocation location, int x, int y, ref bool __state)
        {
            try
            {
                var placementTile = new Vector2(x / 64, y / 64);

                if (__state)
                {
                    ReenableSprinklerFlag_Post(location, (int)placementTile.X, (int)placementTile.Y);
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        private static void DisableSprinklerFlag_Pre(StardewObject __instance, GameLocation location, int x, int y, ref bool __state)
        {
            if (location is Farm && Game1.whichFarm == Farm.beach_layout && HasUnlockedSprinklersInSand)
            {
                if (__instance.IsSprinkler() && location.doesTileHavePropertyNoNull(x, y, "NoSprinklers", "Back") == "T")
                {
                    location.map.GetLayer("Back").PickTile(new Location(x * 64, y * 64), Game1.viewport.Size).Properties["NoSprinklers"] = "F";
                    __state = true;
                }
            }
        }

        private static void ReenableSprinklerFlag_Post(GameLocation location, int x, int y)
        {
            if (location is Farm && Game1.whichFarm == Farm.beach_layout && HasUnlockedSprinklersInSand)
            {
                location.map.GetLayer("Back").PickTile(new Location(x * 64, y * 64), Game1.viewport.Size).Properties["NoSprinklers"] = "T";
            }
        }

        public static bool DontGrowWithoutFertilizerInSand(ref int state, int fertilizer, int xTile, int yTile, GameLocation environment)
        {
            try
            {
                if (environment is Farm && Game1.whichFarm == Farm.beach_layout)
                {
                    if (environment.doesTileHavePropertyNoNull(xTile, yTile, "NoSprinklers", "Back") == "T")
                    {
                        if (state == HoeDirt.watered && fertilizer == HoeDirt.noFertilizer)
                        {
                            state = HoeDirt.dry;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }
    }
}