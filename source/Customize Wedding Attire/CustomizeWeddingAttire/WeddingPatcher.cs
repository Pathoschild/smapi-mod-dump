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
                Monitor.Log("Applying Harmony patch to prefix (and possibly skip) addSpecificTemporarySprite in Event.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Event), "addSpecificTemporarySprite"),
                    prefix: new HarmonyMethod(typeof(WeddingPatcher), nameof(WeddingPatcher.Event_addSpecificTemporarySprite_Prefix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add prefix to before wedding sprite function with exception: {ex}", LogLevel.Error);
            }
            try
            {
                Monitor.Log("Applying Harmony patch to postfix endBehaviors in Event.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Event),nameof(Event.endBehaviors)),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(WeddingPatcher.Event_endBehaviors_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to after wedding sprite function with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to prefix
        private static bool Event_addSpecificTemporarySprite_Prefix(string key, GameLocation location, Event __instance, ref int ___oldShirt, ref Color ___oldPants)
        {
            // If this is not a temporary sprite for a wedding, skip this prefix entirely
            if (key != "wedding")
            {
                return true;
            }

            // Put the player in a tux if desired
            try
            {
                if (Config.WeddingAttire == ModEntry.tuxOption || (Game1.player.IsMale && Config.WeddingAttire == ModEntry.defaultOption))
                {
                    ___oldShirt = __instance.farmer.shirt;
                    ___oldPants = __instance.farmer.pantsColor;
                    putInTux(__instance.farmer);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to make player wear tux with exception: {ex}", LogLevel.Error);
            }

            // Put the player in a dress if desired
            try
            {
                if (Config.WeddingAttire == ModEntry.dressOption)
                {
                    ___oldShirt = __instance.farmer.shirt;
                    ___oldPants = __instance.farmer.pantsColor;
                    putInDress(__instance.farmer);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to make player wear dress with exception: {ex}", LogLevel.Error);
            }

            // Figure out what behavior to take for each other farmer
            try
            {
                foreach (Farmer farmerActor in __instance.farmerActors)
                {
                    long unqID = farmerActor.UniqueMultiplayerID;

                    // If the farmer is the current player, we already handled this, so skip to the next farmer
                    if (unqID == Game1.player.UniqueMultiplayerID)
                    {
                        continue;
                    }

                    // Get the copy of the other farmer in the event
                    Farmer realFarmerActor = Game1.getFarmerMaybeOffline(unqID);

                    // Check for the preference of the farmer in modData
                    if (!realFarmerActor.modData.TryGetValue($"{Manifest.UniqueID}/weddingAttirePref", out string farmerPreference))
                    {
                        // If no preference is recorded, use the game default
                        if (farmerActor.IsMale)
                        {
                            putInTux(farmerActor);
                        }
                    }
                    // Use the game default if preferred
                    else if (farmerPreference == ModEntry.defaultOption) {
                        if (farmerActor.IsMale)
                        {
                            putInTux(farmerActor);
                        }
                    }
                    // Tuxedo if preferred
                    else if(farmerPreference == ModEntry.tuxOption)                        
                    {
                        putInTux(farmerActor);
                    }
                    // Dress if preferred
                    else if (farmerPreference == ModEntry.dressOption)
                    {
                        putInDress(farmerActor);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to change other player's sprite's clothes with exception: {ex}", LogLevel.Error);
            }

            // Do the sprite adding that needs to be done if the function is skipped
            try
            {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1196, 98, 54), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0f, -64f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1250, 98, 25), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0f, 54f) * 4f + new Vector2(0f, -64f), flicker: false, flipped: false, 0f, 0f, Color.White, 4f, 0f, 0f, 0f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(24f, 62f) * 64f, flicker: false, flipped: false, 0f, 0f, Color.White, 4f, 0f, 0f, 0f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(32f, 62f) * 64f, flicker: false, flipped: false, 0f, 0f, Color.White, 4f, 0f, 0f, 0f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(24f, 69f) * 64f, flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(32f, 69f) * 64f, flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f));
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to finish wedding scene setup with exception: {ex}", LogLevel.Error);
            }
            
            return false;
        }

        private static void Event_endBehaviors_Postfix(string[] split, Event __instance, ref int ___oldShirt, ref Color ___oldPants)
        {
            // After the wedding, make sure to change the player's clothes back to normal if needed
            try
            {
                // Process the input string the same way the game does to get the key
                if (split != null && split.Length > 1)
                {
                    string key = split[1];
                    // Only need to change clothes back if this was a wedding
                    if (key != "wedding")
                    {
                        return;
                    }
                    try
                    {
                        // Only need to change clothes back if preference was for changing clothes
                        if (Config.WeddingAttire == ModEntry.dressOption || Config.WeddingAttire == ModEntry.tuxOption || (Game1.player.IsMale && Config.WeddingAttire == ModEntry.defaultOption))
                        {
                            __instance.farmer.changeShirt(-1);
                            __instance.farmer.changePants(___oldPants);
                            __instance.farmer.changePantStyle(-1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Failed to reset clothes after wedding with exception: {ex}", LogLevel.Error);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to initiate resetting clothes after wedding with exception: {ex}", LogLevel.Error);
            }

        }

        private static void putInTux(Farmer farmerActor)
        {
            // This is identical to the game's wedding tuxedo
            farmerActor.changeShirt(10);
            farmerActor.changePants(new Color(49, 49, 49));
            farmerActor.changePantStyle(0);
        }

        private static void putInDress(Farmer farmerActor)
        {
            // Bridal top and long skirt, both in white
            farmerActor.changeShirt(265);
            farmerActor.changePantStyle(2);
            farmerActor.changePants(new Color(255, 255, 255));
        }
    }
}