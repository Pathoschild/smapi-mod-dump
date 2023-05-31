/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Locations.CodeInjections.Modded;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.GingerIsland;

namespace StardewArchipelago.Locations.Patcher
{
    public class ModLocationPatcher : ILocationPatcher
    {
        private readonly ArchipelagoClient _archipelago;
        private readonly Harmony _harmony;
        private readonly GingerIslandPatcher _gingerIslandPatcher;
        private ModsManager _modsManager;

        public ModLocationPatcher(Harmony harmony, ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
            _harmony = harmony;
            _modsManager = archipelago.SlotData.Mods;
        }

        public void ReplaceAllLocationsRewardsWithChecks()
        {
            AddModSkillInjections();
            AddDeepWoodsModInjections();
            AddMagicModInjections();
        }

        private void AddModSkillInjections()
        {
            if ((!_modsManager.HasModdedSkill() || _archipelago.SlotData.SkillProgression == SkillsProgression.Vanilla))
            {
                return;
            }
            var _spaceCoreType = AccessTools.TypeByName("SpaceCore.Skills");
                _harmony.Patch(
                    original: AccessTools.Method(_spaceCoreType, "AddExperience"),
                    prefix: new HarmonyMethod(typeof(SkillInjections), nameof(SkillInjections.AddExperience_ArchipelagoModExperience_Prefix))
                );
        }

        private void AddDeepWoodsModInjections()
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.DEEP_WOODS))
            {
                return;
            }
            var _deepWoodsType = AccessTools.TypeByName("DeepWoodsMod.DeepWoods");
            var _unicornType = AccessTools.TypeByName("DeepWoodsMod.Unicorn");
            var _gingerbreadType = AccessTools.TypeByName("DeepWoodsMod.GingerBreadHouse");
            var _iridiumtreeType = AccessTools.TypeByName("DeepWoodsMod.IridiumTree");
            var _treasureType = AccessTools.TypeByName("DeepWoodsMod.TreasureChest");
            var _fountainType = AccessTools.TypeByName("DeepWoodsMod.HealingFountain");

            _harmony.Patch(
                original: AccessTools.Method(_unicornType, "checkAction"),
                prefix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.CheckAction_PetUnicornLocation_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(_unicornType, "CheckScared"),
                prefix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.CheckScared_MakeUnicornLessScared_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(_treasureType, "checkForAction"),
                prefix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.CheckForAction_TreasureChestLocation_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(_gingerbreadType, "PlayDestroyedSounds"),
                postfix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.PlayDestroyedSounds_GingerbreadLocation_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(_iridiumtreeType, "PlayDestroyedSounds"),
                postfix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.PlayDestroyedSounds_IridiumTreeLocation_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(_fountainType, "performUseAction"),
                prefix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.PerformUseAction_HealingFountainLocation_Prefix))
            );
        }

        private void AddMagicModInjections()
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.MAGIC))
            {
                return;
            }

            var _analyzeSpellType = AccessTools.TypeByName("AnalyzeSpell");
            _harmony.Patch(
                original: AccessTools.Method(_analyzeSpellType, "OnCast"),
                prefix: new HarmonyMethod(typeof(MagicModInjections),
                    nameof(MagicModInjections.OnCast_AnalyzeGivesLocations_Prefix))
            );
        }
    }
}
