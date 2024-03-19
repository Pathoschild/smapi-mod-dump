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
using StardewValley.Buffs;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

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

    internal class Patcher
    {
        private static RingOverhaul mod;

        public static readonly string JukeBoxRingTrackKey = $"{mod?.ModManifest?.UniqueID}JukeBoxRingTrackKey";
        public static readonly string JukeBoxRingHasAddedKey = $"{mod?.ModManifest?.UniqueID}JukeBoxRingHasAddedKey";
        public static readonly string PrecisionSlingshotBuffKey = $"{mod?.ModManifest?.UniqueID}.PrecisionSlingshotDamage";

        private static readonly List<string> explorerIds = new() { "(O)520", "(O)859", "(O)888", "(O)528" }; // 516, 517, 518, 519,
        private static readonly List<string> berserkerIds = new() { "(O)521", "(O)522", "(O)523", "(O)526", "(O)811", "(O)860", "(O)862" };
        private static readonly List<string> iridiumBandIds = new() { "(O)529", "(O)530", "(O)531", "(O)532", "(O)533", "(O)534" }; // 527
        private static readonly List<string> paladinIds = new() { "(O)524", "(O)525", "(O)810", "(O)839", "(O)861", "(O)863", "(O)887" };

        public const string IridiumBandQualifiedID = "(O)527";
        public const string JukeBoxRingQualifiedID = "(O)528";

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
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.AddEquipmentEffects)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(AddEquipmentEffects_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onEquip)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(OnEquip_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onUnequip)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(OnUnequip_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onNewLocation)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(OnNewLocation_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), nameof(Ring.onLeaveLocation)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(OnLeaveLocation_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsMiniJukeboxPlaying)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(IsMiniJukeboxPlaying_Post)));
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
                        result[key] = val + 1;
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
                if (ring.QualifiedItemId is IridiumBandQualifiedID or "(O)516" or "(O)517" or "(O)518" or "(O)519")
                {
                    return RingClass.different;
                }
                else if (explorerIds.Contains(ring.QualifiedItemId))
                {
                    return RingClass.explorer;
                }
                else if (berserkerIds.Contains(ring.QualifiedItemId))
                {
                    return RingClass.berserker;
                }
                else if (iridiumBandIds.Contains(ring.QualifiedItemId))
                {
                    return RingClass.iridiumBand;
                }
                else if (paladinIds.Contains(ring.QualifiedItemId))
                {
                    return RingClass.paladin;
                }

                return RingClass.none;
            }
        }

        public static void LoadDisplayFields_Postfix(CombinedRing __instance)
        {
            RingClass ringClass = GetRingClass(__instance);

            switch (ringClass)
            {
                case RingClass.explorer:
                    __instance.displayName = mod.Helper.Translation.Get("ExplorerRingName");
                    break;

                case RingClass.berserker:
                    __instance.displayName = mod.Helper.Translation.Get("BerserkerRingName");
                    break;

                case RingClass.iridiumBand:
                    __instance.displayName = mod.Helper.Translation.Get("GemBandName");
                    break;

                case RingClass.paladin:
                    __instance.displayName = mod.Helper.Translation.Get("PaladinRingName");
                    break;
            }

            if (mod.Config.RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing || mod.Config.RemoveLuckyTooltipFromCombinedRing)
            {
                __instance.description = "";
                foreach (Ring ring in __instance.combinedRings)
                {
                    if ((ring.QualifiedItemId is "(O)810" or "(O)887") && mod.Config.RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing)
                    {
                        continue;
                    }

                    if ((ring.QualifiedItemId is "(O)859") && mod.Config.RemoveLuckyTooltipFromCombinedRing)
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

        public static bool CanCombine_Prefix(Ring __instance, Ring ring, ref bool __result)
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
            // intentional use of ParentSheetIndex (as the base game method does the same)
            // do not change to ItemId or QualifiedItemId unless it's changed in the same way in the base game
            else if (__instance.ParentSheetIndex == ring.ParentSheetIndex)
            {
                __result = false;
            }

            return false; // don't run original logic
        }

        public static bool DrawInMenu_Prefix(CombinedRing __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
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

        public static void PerformFire_Pre(Farmer who)
        {
            if (!mod.Config.PrecisionBuffsSlingshotDamage)
            {
                return;
            }

            var precisionSlingShotBuff = new Buff(
                id: PrecisionSlingshotBuffKey,
                duration: Buff.ENDLESS,
                effects: new BuffEffects()
                {
                    AttackMultiplier = { who.buffs.WeaponPrecisionMultiplier }
                }
            )
            {
                visible = false
            };

            who.applyBuff(precisionSlingShotBuff);
        }

        public static void PerformFire_Post(Farmer who)
        {
            if (!mod.Config.PrecisionBuffsSlingshotDamage)
            {
                return;
            }

            who.buffs.Remove(PrecisionSlingshotBuffKey);
        }

        public static void AddEquipmentEffects_Post(Ring __instance, ref BuffEffects effects)
        {
            if (!mod.Config.IridiumBandChangesEnabled)
            {
                return;
            }

            if (__instance.QualifiedItemId == IridiumBandQualifiedID)
            {
                //effects.AttackMultiplier.Value += 0.1f; // base is still called, so we don't need to add this
                effects.KnockbackMultiplier.Value += 0.1f;
                effects.WeaponPrecisionMultiplier.Value += 0.1f;
                effects.CriticalChanceMultiplier.Value += 0.1f;
                effects.CriticalPowerMultiplier.Value += 0.1f;
                effects.WeaponSpeedMultiplier.Value += 0.1f;
            }
        }

        public static void OnEquip_Post(Ring __instance, Farmer who)
        {
            GameLocation location = who.currentLocation;
            if (mod.Config.JukeboxRingEnabled && __instance.QualifiedItemId == JukeBoxRingQualifiedID)
            {
                TryAddJukeBoxRing(__instance, location);

                OnEquipJukeBoxRing(who, __instance);
                return;
            }
            else if (__instance.QualifiedItemId == IridiumBandQualifiedID)
            {
                RemoveIridiumBandLight(location, __instance);
                return;
            }
        }

        public static void OnUnequip_Post(Ring __instance, Farmer who)
        {
            GameLocation location = who.currentLocation;
            if (mod.Config.JukeboxRingEnabled && __instance.QualifiedItemId == JukeBoxRingQualifiedID)
            {
                TryRemoveJukeBoxRing(__instance, location);
            }
        }

        public static void OnNewLocation_Post(Ring __instance, GameLocation environment)
        {
            if (mod.Config.JukeboxRingEnabled && __instance.QualifiedItemId == JukeBoxRingQualifiedID)
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

                return;
            }
            else if (__instance.QualifiedItemId == IridiumBandQualifiedID)
            {
                RemoveIridiumBandLight(environment, __instance);
                return;
            }
        }

        public static void OnLeaveLocation_Post(Ring __instance, GameLocation environment)
        {
            if (mod.Config.JukeboxRingEnabled && __instance.QualifiedItemId == JukeBoxRingQualifiedID)
            {
                TryRemoveJukeBoxRing(__instance, environment);
            }
        }

        public static void IsMiniJukeboxPlaying_Post(GameLocation __instance, ref bool __result)
        {
            // we only try to override an 'anti rain check', so if the config is off, we don't need to do anything
            if (__result || !mod.Config.JukeboxRingWorksInRain || !mod.Config.JukeboxRingEnabled)
            {
                return;
            }

            bool isRaining = __instance.IsOutdoors && __instance.IsRainingHere();
            bool couldPlayMiniJukeBox = __instance.miniJukeboxCount.Value > 0 && __instance.miniJukeboxTrack.Value != string.Empty;

            bool wasOnlyCancelledDueToRain = isRaining && couldPlayMiniJukeBox;

            if (wasOnlyCancelledDueToRain)
            {
                // check if one of the 'miniJukeboxCount' was a jukebox ring
                foreach (var player in Game1.getOnlineFarmers())
                {
                    if (player.currentLocation == __instance && player.isWearingRing(JukeBoxRingQualifiedID))
                    {
                        __result = true;
                        break;
                    }
                }
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

        // based on MiniJukebox.checkForAction
        public static void OnEquipJukeBoxRing(Farmer who, Ring ring)
        {
            if (who != Game1.player)
            {
                return;
            }

            GameLocation location = who.currentLocation;

            List<string> jukeboxTracks = Utility.GetJukeboxTracks(who, location);
            jukeboxTracks.Insert(0, "turn_off");
            jukeboxTracks.Add("random");
            Game1.activeClickableMenu = new ChooseFromListMenu(jukeboxTracks, (s) => OnSongChosen(s, location, ring), isJukebox: true, location.miniJukeboxTrack.Value);
        }

        public static void RemoveIridiumBandLight(GameLocation environment, Ring ring)
        {
            if (!mod.Config.IridiumBandChangesEnabled)
            {
                return;
            }

            var fieldInfo = AccessTools.Field(typeof(Ring), "_lightSourceID");

            if (fieldInfo.GetValue(ring) != null)
            {
                environment.removeLightSource(((int?)fieldInfo.GetValue(ring)).Value);
            }

            fieldInfo.SetValue(ring, null);
        }

        // based on MiniJukebox.OnSongChosen
        public static void OnSongChosen(string selection, GameLocation location, Ring ring)
        {
            if (location == null)
            {
                return;
            }

            ring.modData[JukeBoxRingTrackKey] = selection;

            if (selection == "turn_off")
            {
                location.miniJukeboxTrack.Value = string.Empty;
            }
            else
            {
                location.miniJukeboxTrack.Value = selection;

                if (selection == "random")
                {
                    location.SelectRandomMiniJukeboxTrack();
                }
            }
        }
    }
}