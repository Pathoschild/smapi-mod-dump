/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewObject = StardewValley.Object;

namespace RingOverhaul
{
    public enum RingClass
    {
        none = 0,
        different = 1,
        explorer = 2,
        berserker = 3,
        iridiumBand = 4,
        paladin = 5
    }

    public struct SimpleFurnaceRecipe
    {
        public SimpleFurnaceRecipe(List<int> input, int output, string recipe)
        {
            inputIds = input;
            outputId = output;
            requiredRecipe = recipe;
        }

        public List<int> inputIds;
        public int outputId;
        public string requiredRecipe;
    }

    internal class Patcher
    {
        private static RingOverhaul mod;

        public static readonly string JukeBoxRingTrackKey = $"{mod?.ModManifest?.UniqueID}JukeBoxRingTrackKey";
        public static readonly string JukeBoxRingHasAddedKey = $"{mod?.ModManifest?.UniqueID}JukeBoxRingHasAddedKey";

        private static readonly List<int> explorerIds = new() { 520, 859, 888, 528 }; // 516, 517, 518, 519,
        private static readonly List<int> berserkerIds = new() { 521, 522, 523, 526, 811, 860, 862 };
        private static readonly List<int> iridiumBandIds = new() { 529, 530, 531, 532, 533, 534 }; // 527
        private static readonly List<int> paladinIds = new() { 524, 525, 810, 839, 861, 863, 887 };

        public static void PatchAll(RingOverhaul horseOverhaul)
        {
            mod = horseOverhaul;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.CanCombine)),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(CanCombine_Prefix)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(CombinedRing), "loadDisplayFields"),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(LoadDisplayFields_Postfix)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(CombinedRing), nameof(CombinedRing.drawInMenu), new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(DrawInMenu_Prefix)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Slingshot), nameof(Slingshot.PerformFire)),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(PerformFire_Pre)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Slingshot), nameof(Slingshot.PerformFire)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(PerformFire_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onEquip)),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(OnEquip_Pre)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onUnequip)),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(OnUnequip_Pre)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onNewLocation)),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(OnNewLocation_Pre)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onLeaveLocation)),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(OnLeaveLocation_Pre)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsMiniJukeboxPlaying)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(IsMiniJukeboxPlaying)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.performObjectDropInAction), new[] { typeof(Item), typeof(bool), typeof(Farmer) }),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(UpdateFurnaceInput)));
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        public static int GetCombinedRingTotal(Ring ring)
        {
            if (ring is CombinedRing)
            {
                int count = 0;
                foreach (Ring r in (ring as CombinedRing).combinedRings)
                {
                    count += GetCombinedRingTotal(r);
                }
                return count;
            }
            else
            {
                return 1;
            }
        }

        public static SortedDictionary<string, int> GetCombinedRings(Ring ring)
        {
            var result = new SortedDictionary<string, int>();
            var to_process = new Queue<Ring>();
            to_process.Enqueue(ring);
            while (to_process.Count > 0)
            {
                Ring cur = to_process.Dequeue();
                if (cur is CombinedRing)
                {
                    foreach (Ring r in (cur as CombinedRing).combinedRings)
                    {
                        to_process.Enqueue(r);
                    }
                }
                else
                {
                    string key = cur.displayName;
                    if (result.TryGetValue(key, out int val))
                    {
                        result.Add(key, val + 1);
                    }
                    else
                    {
                        result.Add(key, 1);
                    }
                }
            }
            return result;
        }

        public static RingClass GetRingClass(Ring ring)
        {
            if (ring is CombinedRing combinedRing)
            {
                if (combinedRing.combinedRings.Count == 0)
                {
                    return RingClass.none;
                }

                RingClass currentClass = GetRingClass(combinedRing.combinedRings[0]);

                foreach (Ring part in combinedRing.combinedRings)
                {
                    var nClass = GetRingClass(part);

                    if (currentClass == nClass)
                    {
                        continue;
                    }
                    else if (nClass == RingClass.none)
                    {
                        continue;
                    }
                    else if (currentClass == RingClass.none)
                    {
                        currentClass = nClass;
                    }
                    else
                    {
                        return RingClass.different;
                    }
                }

                return currentClass;
            }
            else
            {
                // so they can't get combined with their own ingredients/ results
                if (ring.ParentSheetIndex is RingOverhaul.IridiumBandID or 516 or 517 or 518 or 519)
                {
                    return RingClass.different;
                }
                else if (explorerIds.Contains(ring.ParentSheetIndex))
                {
                    return RingClass.explorer;
                }
                else if (berserkerIds.Contains(ring.ParentSheetIndex))
                {
                    return RingClass.berserker;
                }
                else if (iridiumBandIds.Contains(ring.ParentSheetIndex))
                {
                    return RingClass.iridiumBand;
                }
                else if (paladinIds.Contains(ring.ParentSheetIndex))
                {
                    return RingClass.paladin;
                }

                return RingClass.none;
            }
        }

        public static void LoadDisplayFields_Postfix(CombinedRing __instance)
        {
            try
            {
                RingClass ringClass = GetRingClass(__instance);

                switch (ringClass)
                {
                    case RingClass.explorer:
                        __instance.DisplayName = mod.Helper.Translation.Get("ExplorerRingName");
                        break;

                    case RingClass.berserker:
                        __instance.DisplayName = mod.Helper.Translation.Get("BerserkerRingName");
                        break;

                    case RingClass.iridiumBand:
                        __instance.DisplayName = mod.Helper.Translation.Get("GemBandName");
                        break;

                    case RingClass.paladin:
                        __instance.DisplayName = mod.Helper.Translation.Get("PaladinRingName");
                        break;
                }

                if (mod.Config.RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing || mod.Config.RemoveLuckyTooltipFromCombinedRing)
                {
                    __instance.description = "";
                    foreach (Ring ring in __instance.combinedRings)
                    {
                        if ((ring.ParentSheetIndex is 810 or 887) && mod.Config.RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing)
                        {
                            continue;
                        }

                        if ((ring.ParentSheetIndex is 859) && mod.Config.RemoveLuckyTooltipFromCombinedRing)
                        {
                            continue;
                        }

                        ring.getDescription();
                        __instance.description += ring.description + "\n\n";
                    }

                    __instance.description = __instance.description.Trim();
                }

                if (GetCombinedRingTotal(__instance) >= 8)
                {
                    string description = "Many Rings forged into one:\n\n";
                    foreach (KeyValuePair<string, int> entry in GetCombinedRings(__instance))
                    {
                        description += String.Format("{1}x {0}\n", entry.Key, entry.Value);
                    }
                    __instance.description = description.Trim();
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool CanCombine_Prefix(Ring __instance, Ring ring, ref bool __result)
        {
            try
            {
                __result = true;

                var class1 = GetRingClass(__instance);
                var class2 = GetRingClass(ring);

                if (class1 == RingClass.different || class2 == RingClass.different)
                {
                    __result = false;
                    return false;
                }

                if (__instance is CombinedRing || ring is CombinedRing)
                {
                    if (class1 != RingClass.none && class2 != RingClass.none && class1 != class2)
                    {
                        __result = false;
                        return false;
                    }
                }

                if (ring is CombinedRing)
                {
                    foreach (Ring combinedRing in (ring as CombinedRing).combinedRings)
                    {
                        if (!__instance.CanCombine(combinedRing))
                        {
                            __result = false;
                            return false;
                        }
                    }
                }
                else if (__instance is CombinedRing)
                {
                    foreach (Ring combinedRing in (__instance as CombinedRing).combinedRings)
                    {
                        if (!combinedRing.CanCombine(ring))
                        {
                            __result = false;
                            return false;
                        }
                    }
                }
                else if (__instance.ParentSheetIndex == ring.ParentSheetIndex)
                {
                    __result = false;
                }
                return false; // don't run original logic
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true; // run original logic
            }
        }

        public static bool DrawInMenu_Prefix(CombinedRing __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            try
            {
                Texture2D texture = GetRingClass(__instance) switch
                {
                    RingClass.explorer => mod.ExplorerRingTexture,
                    RingClass.berserker => mod.BerserkerRingTexture,
                    RingClass.paladin => mod.PaladinRingTexture,
                    _ => null,
                };

                if (texture != null)
                {
                    spriteBatch.Draw(texture, location + new Vector2(32f, 32f) * scaleSize, null, color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
                    return false; // don't run original logic
                }

                if (__instance.combinedRings.Count >= 2)
                {
                    // always use base rings as the sprites to draw. The first pair that are combined on the left hand side get used as the sprite.
                    if (__instance.combinedRings[0] is CombinedRing)
                    {
                        __instance.combinedRings[0].drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
                        return false; // don't run original logic
                    }
                    else if (__instance.combinedRings[1] is CombinedRing)
                    {
                        __instance.combinedRings[1].drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
                        return false; // don't run original logic
                    }
                }

                return true; // run original logic
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true; // run original logic
            }
        }

        public static bool PerformFire_Pre(Farmer who)
        {
            try
            {
                who.attackIncreaseModifier += who.weaponPrecisionModifier;
                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void PerformFire_Post(Farmer who)
        {
            try
            {
                who.attackIncreaseModifier -= who.weaponPrecisionModifier;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool OnEquip_Pre(Ring __instance, Farmer who, GameLocation location)
        {
            try
            {
                if (mod.Config.JukeboxRingEnabled && __instance.indexInTileSheet.Value == RingOverhaul.JukeBoxRingID)
                {
                    TryAddJukeBoxRing(__instance, location);

                    OnEquipJukeBoxRing(who, __instance);
                    return false;
                }
                else if (__instance.indexInTileSheet.Value == RingOverhaul.IridiumBandID)
                {
                    var fieldInfo = AccessTools.Field(typeof(Ring), "_lightSourceID");

                    if (fieldInfo.GetValue(__instance) != null)
                    {
                        location.removeLightSource(((int?)fieldInfo.GetValue(__instance)).Value);
                    }

                    fieldInfo.SetValue(__instance, null);

                    who.attackIncreaseModifier += 0.1f; // base is not called, so we need to add this
                    who.knockbackModifier += 0.1f;
                    who.weaponPrecisionModifier += 0.1f;
                    who.critChanceModifier += 0.1f;
                    who.critPowerModifier += 0.1f;
                    who.weaponSpeedModifier += 0.1f;

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool OnUnequip_Pre(Ring __instance, Farmer who, GameLocation location)
        {
            try
            {
                if (mod.Config.JukeboxRingEnabled && __instance.indexInTileSheet.Value == RingOverhaul.JukeBoxRingID)
                {
                    TryRemoveJukeBoxRing(__instance, location);

                    return false;
                }
                else if (__instance.indexInTileSheet.Value == RingOverhaul.IridiumBandID)
                {
                    who.attackIncreaseModifier -= 0.1f;
                    who.knockbackModifier -= 0.1f;
                    who.weaponPrecisionModifier -= 0.1f;
                    who.critChanceModifier -= 0.1f;
                    who.critPowerModifier -= 0.1f;
                    who.weaponSpeedModifier -= 0.1f;

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool OnNewLocation_Pre(Ring __instance, GameLocation environment)
        {
            try
            {
                if (mod.Config.JukeboxRingEnabled && __instance.indexInTileSheet.Value == RingOverhaul.JukeBoxRingID)
                {
                    TryAddJukeBoxRing(__instance, environment);

                    // != true because it can be a null compare
                    if (environment?.currentEvent?.isFestival != true)
                    {
                        if (__instance.modData.ContainsKey(JukeBoxRingTrackKey))
                        {
                            environment.miniJukeboxTrack.Value = __instance.modData[JukeBoxRingTrackKey];

                            if (__instance.modData[JukeBoxRingTrackKey] == "random")
                            {
                                environment.SelectRandomMiniJukeboxTrack();
                            }
                        }
                    }

                    return false;
                }
                else if (__instance.indexInTileSheet.Value == RingOverhaul.IridiumBandID)
                {
                    var fieldInfo = AccessTools.Field(typeof(Ring), "_lightSourceID");

                    if (fieldInfo.GetValue(__instance) != null)
                    {
                        environment.removeLightSource(((int?)fieldInfo.GetValue(__instance)).Value);
                    }

                    fieldInfo.SetValue(__instance, null);

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool OnLeaveLocation_Pre(Ring __instance, GameLocation environment)
        {
            try
            {
                if (mod.Config.JukeboxRingEnabled && __instance.indexInTileSheet.Value == RingOverhaul.JukeBoxRingID)
                {
                    TryRemoveJukeBoxRing(__instance, environment);

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void IsMiniJukeboxPlaying(GameLocation __instance, ref bool __result)
        {
            try
            {
                // anti rain check
                if (mod.Config.JukeboxRingWorksInRain && !__result && __instance.miniJukeboxCount.Value > 0 && __instance.miniJukeboxTrack.Value != "")
                {
                    foreach (var player in Game1.getOnlineFarmers())
                    {
                        if (player.currentLocation == __instance && player.isWearingRing(RingOverhaul.JukeBoxRingID))
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static void TryAddJukeBoxRing(Ring ring, GameLocation location)
        {
            if (!ring.modData.ContainsKey(JukeBoxRingHasAddedKey))
            {
                location.OnMiniJukeboxAdded();
                ring.modData[JukeBoxRingHasAddedKey] = "true";
            }
        }

        public static void TryRemoveJukeBoxRing(Ring ring, GameLocation location)
        {
            if (ring.modData.ContainsKey(JukeBoxRingHasAddedKey))
            {
                location.OnMiniJukeboxRemoved();
                ring.modData.Remove(JukeBoxRingHasAddedKey);
            }
        }

        public static void OnEquipJukeBoxRing(Farmer who, Ring ring)
        {
            List<string> list = Game1.player.songsHeard.Distinct().ToList();
            list.Insert(0, "turn_off");
            list.Add("random");
            Game1.activeClickableMenu = new ChooseFromListMenu(list, new ChooseFromListMenu.actionOnChoosingListOption((s) => OnSongChosen(s, ring)), true, who.currentLocation.miniJukeboxTrack.Value);
        }

        public static void OnSongChosen(string selection, Ring ring)
        {
            if (Game1.player.currentLocation != null)
            {
                if (selection == "turn_off")
                {
                    Game1.player.currentLocation.miniJukeboxTrack.Value = "";
                    return;
                }

                Game1.player.currentLocation.miniJukeboxTrack.Value = selection;

                if (selection == "random")
                {
                    Game1.player.currentLocation.SelectRandomMiniJukeboxTrack();
                }

                ring.modData[JukeBoxRingTrackKey] = selection;
            }
        }

        public static bool UpdateFurnaceInput(ref StardewObject __instance, ref bool __result, ref Item dropInItem, ref bool probe, ref Farmer who)
        {
            try
            {
                if (!probe && __instance.name.Equals("Furnace") && __instance.heldObject.Value == null)
                {
                    if (dropInItem is StardewObject o && o.ParentSheetIndex == RingOverhaul.CoalID && who.IsLocalPlayer)
                    {
                        var inventory = who.Items;
                        SimpleFurnaceRecipe[] recipes = new SimpleFurnaceRecipe[]
                        {
                            new SimpleFurnaceRecipe(new List<int>{ 516 }, 517, "Glow Ring"), // + 5x 768
                            new SimpleFurnaceRecipe(new List<int>{ 518 }, 519, "Magnet Ring"), // + 5x 769
                            new SimpleFurnaceRecipe(new List<int>{ 517, 519 }, 888, "Glowstone Ring"),
                            new SimpleFurnaceRecipe(new List<int>{ 529, 530, 531, 532, 533, 534 }, RingOverhaul.IridiumBandID, "Iridium Band"), // + 2x 337
                        };

                        var foundIds = new List<int>();

                        for (int i = inventory.Count - 1; i >= 0; i--)
                        {
                            if (inventory[i] is Ring ring)
                            {
                                foundIds.Add(ring.ParentSheetIndex);
                            }
                        }

                        foreach (var recipe in recipes)
                        {
                            if (!who.craftingRecipes.ContainsKey(recipe.requiredRecipe))
                            {
                                continue;
                            }

                            // check if the recipe is applicable
                            if (foundIds.Intersect(recipe.inputIds).ToList().Count == recipe.inputIds.Count)
                            {
                                if (recipe.outputId == 517)
                                {
                                    if (who.getTallyOfObject(768, false) >= 5)
                                    {
                                        __instance.ConsumeInventoryItem(who, 768, 5);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                if (recipe.outputId == 519)
                                {
                                    if (who.getTallyOfObject(769, false) >= 5)
                                    {
                                        __instance.ConsumeInventoryItem(who, 769, 5);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                if (recipe.outputId == RingOverhaul.IridiumBandID)
                                {
                                    if (who.getTallyOfObject(337, false) >= 2)
                                    {
                                        __instance.ConsumeInventoryItem(who, 337, 2);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                for (int i = inventory.Count - 1; i >= 0; i--)
                                {
                                    if (inventory[i] is Ring ring && recipe.inputIds.Contains(ring.ParentSheetIndex))
                                    {
                                        recipe.inputIds.Remove(ring.ParentSheetIndex);
                                        inventory[i] = null;
                                    }
                                }

                                who.craftingRecipes[recipe.requiredRecipe]++;

                                __instance.ConsumeInventoryItem(who, RingOverhaul.CoalID, 1);

                                who.currentLocation.debris.Add(new Debris(new Ring(recipe.outputId), __instance.TileLocation * 64f));
                                who.currentLocation.playSound("furnace", NetAudio.SoundContext.Default);

                                var multiplayer = (Multiplayer)AccessTools.Field(typeof(Game1), "multiplayer").GetValue(null);

                                multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[]
                                {
                                    new TemporaryAnimatedSprite(30, __instance.TileLocation * 64f + new Vector2(0f, -16f), Color.White, 4, false, 50f, 10, 64, (__instance.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, -1, 0)
                                    {
                                        alphaFade = 0.005f
                                    }
                                });

                                __result = false;
                                return false;
                            }
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