/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/CustomizeWeddingAttire
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace CustomizeWeddingAttire
{
    // Applies Harmony patches to Utility.cs to add a conversation topic for weddings.
    public class WeddingPatcher
    {
        private static IMonitor Monitor;
        private static ModConfig Config;
        private static ITranslationHelper I18n;
        private static IManifest Manifest;

        static string tuxShirt = "1010";
        static string tuxPants = "0";
        static Color tuxColor = new Color(49, 49, 49);

        static string dressShirt = "1265";
        static string dressPants = "2";
        static Color dressColor = new Color(255, 255, 255);

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config, ITranslationHelper translator, IManifest manifest)
        {
            Monitor = monitor;
            Config = config;
            I18n = translator;
            Manifest = manifest;
        }

        // Method to apply harmony patch
        public static void Apply(Harmony harmony)
        {
            try
            {
                Monitor.Log("Applying Harmony patch to postfix IsOverridingPants in Farmer.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.IsOverridingPants)),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(WeddingPatcher.Farmer_IsOverridingPants_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to after farmer pants override function with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Applying Harmony patch to postfix IsOverridingShirt in Farmer.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.IsOverridingShirt)),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(WeddingPatcher.Farmer_IsOverridingShirt_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to after farmer shirt override function with exception: {ex}", LogLevel.Error);
            }
        }

        private static void Farmer_IsOverridingPants_Postfix(Farmer __instance, ref string id, ref Color? color, ref bool __result)
        {
            // Return if not during a wedding
            if (!Context.IsWorldReady || Game1.CurrentEvent == null || !Game1.CurrentEvent.isWedding)
            {
                return;
            }

            // Get preference and change accordingly
            try
            {
                // Identify which farmer we're dealing with here
                long unqID = __instance.UniqueMultiplayerID;

                // Be more careful about changing current player's clothing
                if (unqID == Game1.player.UniqueMultiplayerID)
                {
                    string preference = getOutfitTypeCurrentFarmer();
                    if (preference == "tux")
                    {
                        id = tuxPants;
                        color = tuxColor;
                        __result = true;
                    }
                    else if (preference == "dress")
                    {
                        id = dressPants;
                        color = dressColor;
                        __result = true;
                    }
                    else if (preference == "custom")
                    {
                        if (isValidPants(Config.CustomPants) && isValidColor(Config.PantsR, Config.PantsG, Config.PantsB))
                        {
                            id = Config.CustomPants;
                            color = new Color(Config.PantsR, Config.PantsG, Config.PantsB);
                            __result = true;
                        }
                    }
                }
                // Fake event farmers can just directly get changed
                else
                {
                    string preference = getOutfitTypeOtherFarmers(unqID);
                    if (preference == "tux")
                    {
                        putInTux(__instance);
                    }
                    else if (preference == "dress")
                    {
                        putInDress(__instance);
                    }
                    else if (preference == "custom")
                    {
                        string[] outfit = getCustomOutfitOtherFarmers(unqID);
                        if (outfit != null && Int32.TryParse(outfit[2], out int pantsR) && Int32.TryParse(outfit[3], out int pantsG) && Int32.TryParse(outfit[4], out int pantsB))
                        {
                            putInCustom(__instance, outfit[0], outfit[1], pantsR, pantsG, pantsB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to make player pants change with exception: {ex}", LogLevel.Error);
            }
        }

        private static void Farmer_IsOverridingShirt_Postfix(Farmer __instance, ref string id, ref bool __result)
        {
            // Return if not during a wedding
            if (!Context.IsWorldReady || Game1.CurrentEvent == null || !Game1.CurrentEvent.isWedding)
            {
                return;
            }

            // Get preference and change accordingly
            try
            {
                // Identify which farmer we're dealing with here
                long unqID = __instance.UniqueMultiplayerID;

                // Be more careful about changing current player's clothing
                if (unqID == Game1.player.UniqueMultiplayerID)
                {
                    string preference = getOutfitTypeCurrentFarmer();
                    if (preference == "tux")
                    {
                        id = tuxShirt;
                        __result = true;
                    }
                    else if (preference == "dress")
                    {
                        id = dressShirt;
                        __result = true;
                    }
                    else if (preference == "custom")
                    {
                        if (isValidShirt(Config.CustomShirt))
                        {
                            id = Config.CustomShirt;
                            __result = true;
                        }
                    }
                }
                // Fake event farmers can just directly get changed
                else
                {
                    string preference = getOutfitTypeOtherFarmers(unqID);
                    if (preference == "tux")
                    {
                        putInTux(__instance);
                    }
                    else if (preference == "dress")
                    {
                        putInDress(__instance);
                    }
                    else if (preference == "custom")
                    {
                        string[] outfit = getCustomOutfitOtherFarmers(unqID);
                        if (outfit != null && Int32.TryParse(outfit[2], out int pantsR) && Int32.TryParse(outfit[3], out int pantsG) && Int32.TryParse(outfit[4], out int pantsB))
                        {
                            putInCustom(__instance, outfit[0], outfit[1], pantsR, pantsG, pantsB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to make player pants change with exception: {ex}", LogLevel.Error);
            }
        }

        private static string getOutfitTypeCurrentFarmer()
        {
            // Put the player in a tux if desired
            if (Config.WeddingAttire == ModEntry.tuxOption || (Game1.player.IsMale && Config.WeddingAttire == ModEntry.defaultOption))
            {
                return "tux";
            }

            // Put the player in a dress if desired
            if (Config.WeddingAttire == ModEntry.dressOption)
            {
                return "dress";
            }

            if (Config.WeddingAttire == ModEntry.customOption)
            {
                return "custom";
            }
            return "none";
        }

        // Respect other players' preferences if possible
        private static string getOutfitTypeOtherFarmers(long unqID)
        {
            // Get the copy of the other farmer in the event
            Farmer realFarmerActor = Game1.getFarmerMaybeOffline(unqID);

            // Check for the preference of the farmer in modData and do nothing if no preference found
            if (!realFarmerActor.modData.TryGetValue($"{Manifest.UniqueID}/weddingAttirePref", out string farmerPreference))
            {
                return "none";
            }
            // Tuxedo if preferred
            else if (farmerPreference == ModEntry.tuxOption || farmerPreference == ModEntry.defaultOption && realFarmerActor.IsMale)
            {
                return "tux";
            }
            // Dress if preferred
            else if (farmerPreference == ModEntry.dressOption)
            {
                return "dress";
            }
            else if (farmerPreference == ModEntry.customOption)
            {
                return "custom";
            }
            
            return "none";
        }

        private static string[] getCustomOutfitOtherFarmers(long unqID)
        {
            // Get the copy of the other farmer in the event
            Farmer realFarmerActor = Game1.getFarmerMaybeOffline(unqID);

            // Check for the preference of the farmer in modData and do nothing if no preference found
            if (!realFarmerActor.modData.TryGetValue($"{Manifest.UniqueID}/customAttirePref", out string customOutfit))
            {
                return null;
            }
            else
            {
                string[] outfit = customOutfit.Split("|");
                if (outfit.Length != 5)
                {
                    return null;
                }
                else
                {
                    return outfit;
                }
            }
        }

        public static void putInTux(Farmer farmer)
        {
            // This is identical to the game's wedding tuxedo
            farmer.changeShirt(tuxShirt);
            farmer.changePantStyle(tuxPants);
            farmer.changePantsColor(tuxColor);
        }

        public static void putInDress(Farmer farmer)
        {
            // Bridal top and long skirt, both in white
            farmer.changeShirt(dressShirt);
            farmer.changePantStyle(dressPants);
            farmer.changePantsColor(dressColor);
        }

        public static void putInCustom(Farmer farmer, string customShirt, string customPants, int pantsR, int pantsG, int pantsB)
        {
            // Bridal top and long skirt, both in white
            if (isValidShirt(customShirt))
            {
                farmer.changeShirt(customShirt);
            }
            if (isValidPants(customPants))
            {
                farmer.changePantStyle(customPants);
            }
            if (isValidColor(pantsR, pantsG, pantsB))
            {
                Color pantsColor = new Color(pantsR, pantsG, pantsB);
                farmer.changePantsColor(pantsColor);
            }
        }

        private static bool isValidPants(string pantsId)
        {
            if (ItemRegistry.GetDataOrErrorItem("(P)" + pantsId).IsErrorItem)
            {
                return false;
            }
            return true;
        }

        private static bool isValidShirt(string shirtId)
        {
            if (ItemRegistry.GetDataOrErrorItem("(S)" + shirtId).IsErrorItem)
            {
                return false;
            }
            return true;
        }

        private static bool isValidColor(int R, int G, int B)
        {
            if (R < 0 || G < 0 || B < 0)
            {
                return false;
            }
            if (R > 255 || G > 255 || B > 255)
            {
                return false;
            }
            return true;
        }
    }
}