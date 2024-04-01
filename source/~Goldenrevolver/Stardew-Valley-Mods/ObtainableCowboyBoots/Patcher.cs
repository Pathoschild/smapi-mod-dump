/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ObtainableCowboyBoots
{
    using HarmonyLib;
    using StardewValley;
    using StardewValley.Characters;
    using StardewValley.Objects;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;

    internal class Patcher
    {
        private static ObtainableCowboyBoots mod;

        public const string cowboyBootsQualifiedID = "(B)515";

        public static void PatchAll(ObtainableCowboyBoots mod)
        {
            Patcher.mod = mod;

            var harmony = new Harmony(Patcher.mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(AddCowboyBootsMovementSpeed)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Item), nameof(Item.onEquip)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(HatOnEquip)));
        }

        public static readonly HashSet<string> pastCowboyHats = new();

        public static void HatOnEquip(Item __instance, Farmer who)
        {
            if (who != Game1.player)
            {
                return;
            }

            if (__instance is not Hat hat)
            {
                return;
            }

            if (IsCowboyHat(hat))
            {
                if (!pastCowboyHats.Contains(hat.QualifiedItemId))
                {
                    pastCowboyHats.Add(hat.QualifiedItemId);
                }

                if (mod.Config.CanOnlyObtainOnce && (who?.modData?.ContainsKey(ObtainableCowboyBoots.obtainedCowboyBootsKey) == true))
                {
                    return;
                }

                bool wonCowboyGame = mod.Config.SkipHatCountIfBeatenCowboyArcadeGame && who.mailReceived.Contains("Beat_PK");
                bool hasEnoughHats = pastCowboyHats.Count >= mod.Config.RequiredCowboyHats;

                if (wonCowboyGame || hasEnoughHats)
                {
                    who.modData[ObtainableCowboyBoots.obtainedCowboyBootsKey] = "true";

                    who.playNearbySoundLocal("cowboy_gunload");
                    //who.playNearbySoundLocal("Cowboy_Secret");
                    //who.playNearbySoundLocal("Cowboy_gunshot");
                    //who.playNearbySoundLocal("cowboy_outlawsong");
                    //who.playNearbySoundLocal("cowboy_boss");
                    //who.playNearbySoundLocal("Cowboy_singing");
                    //who.playNearbySoundLocal("cowboy_powerup");

                    who.addItemByMenuIfNecessary(ItemRegistry.Create(cowboyBootsQualifiedID));
                }
            }
            else
            {
                pastCowboyHats.Clear();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Not a readability improvement")]
        public static bool IsCowboyHat(Hat hat)
        {
            if (hat == null)
            {
                return false;
            }

            switch (hat.QualifiedItemId)
            {
                case "(H)0":
                case "(H)33":
                case "(H)34":
                case "(H)37":
                case "(H)38":
                case "(H)60":
                case "(H)73":
                case "(H)81":
                case "(H)83":
                    return true;

                default:
                    return false;
            }
        }

        // old reliable code from HorseOverhaul
        public static void AddCowboyBootsMovementSpeed(ref Farmer __instance, ref float __result)
        {
            if (__instance.boots.Value?.QualifiedItemId != cowboyBootsQualifiedID)
            {
                return;
            }

            if (mod.Config.MovementSpeedWhileOnSandOrDirt && !Game1.eventUp && (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence) && !(__instance.hasBuff("19") && Game1.CurrentEvent == null))
            {
                if (IsTractor(__instance.mount))
                {
                    return;
                }

                var step_type = __instance.FarmerSprite.currentStep;

                if (__instance.currentLocation != null
                    && __instance.currentLocation.terrainFeatures.TryGetValue(__instance.Tile, out var terrainFeature)
                    && terrainFeature is Flooring flooring)
                {
                    step_type = flooring.getFootstepSound();
                }

                if (step_type != "sandyStep")
                {
                    return;
                }

                float addedMovementSpeed;

                if (__instance.mount != null)
                {
                    addedMovementSpeed = Math.Max(0f, mod.Config.BonusHorseMovementSpeed);
                }
                else
                {
                    addedMovementSpeed = Math.Max(0f, mod.Config.BonusOnFootMovementSpeed);
                }

                if (__instance.movementDirections.Count > 1)
                {
                    addedMovementSpeed *= 0.7f;
                }

                __result += addedMovementSpeed;
            }
        }

        public static bool IsTractor(Horse horse)
        {
            return horse != null && horse.modData.ContainsKey("Pathoschild.TractorMod");
        }
    }
}