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
using StardewArchipelago.GameModifications.CodeInjections.Modded;
using StardewArchipelago.Locations.CodeInjections.Modded;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.GingerIsland;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.Patcher
{
    public class ModLocationPatcher : ILocationPatcher
    {
        private readonly ArchipelagoClient _archipelago;
        private readonly Harmony _harmony;
        private readonly IModHelper _modHelper;
        private readonly GingerIslandPatcher _gingerIslandPatcher;
        private ModsManager _modsManager;

        public ModLocationPatcher(Harmony harmony, IModHelper modHelper, ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
            _harmony = harmony;
            _modHelper = modHelper;
            _modsManager = archipelago.SlotData.Mods;
        }

        public void ReplaceAllLocationsRewardsWithChecks()
        {
            AddModSkillInjections();
            AddDeepWoodsModInjections();
            AddMagicModInjections();
            AddSkullCavernElevatorModInjections();
        }

        private void AddModSkillInjections()
        {
            InjectSpaceCoreSkillsPage();

            if (!_modsManager.HasModdedSkill() || _archipelago.SlotData.SkillProgression == SkillsProgression.Vanilla)
            {
                return;
            }

            var spaceCoreType = AccessTools.TypeByName("SpaceCore.Skills");
            _harmony.Patch(
                original: AccessTools.Method(spaceCoreType, "AddExperience"),
                prefix: new HarmonyMethod(typeof(SkillInjections), nameof(SkillInjections.AddExperience_ArchipelagoModExperience_Prefix))
            );

            InjectSocializingExperienceMultiplier();
            InjectArchaeologyExperienceMultiplier();
        }

        private void InjectSpaceCoreSkillsPage()
        {
            if (!_modsManager.ModIsInstalledAndLoaded(_modHelper, "SpaceCore"))
            {
                return;
            }

            var spaceCoreSkillsPageType = AccessTools.TypeByName("SpaceCore.Interface.NewSkillsPage");
            var desiredNewSkillsPageCtorParameters = new[] { typeof(int), typeof(int), typeof(int), typeof(int) };
            _harmony.Patch(
                original: AccessTools.Constructor(spaceCoreSkillsPageType, desiredNewSkillsPageCtorParameters),
                prefix: new HarmonyMethod(typeof(NewSkillsPageInjections),
                    nameof(NewSkillsPageInjections.NewSkillsPageCtor_BearKnowledgeEvent_Prefix)),
                postfix: new HarmonyMethod(typeof(NewSkillsPageInjections),
                    nameof(NewSkillsPageInjections.NewSkillsPageCtor_BearKnowledgeEvent_Postfix))
            );
        }

        private void InjectSocializingExperienceMultiplier()
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.SOCIALIZING))
            {
                return;
            }

            var socializingConfigType = AccessTools.TypeByName("SocializingSkill.Config");
            _harmony.Patch(
                original: AccessTools.PropertyGetter(socializingConfigType, "ExperienceFromTalking"),
                postfix: new HarmonyMethod(typeof(SocializingConfigCodeInjections), nameof(SocializingConfigCodeInjections.ExperienceFromTalking_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(socializingConfigType, "ExperienceFromGifts"),
                postfix: new HarmonyMethod(typeof(SocializingConfigCodeInjections), nameof(SocializingConfigCodeInjections.ExperienceFromGifts_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(socializingConfigType, "ExperienceFromEvents"),
                postfix: new HarmonyMethod(typeof(SocializingConfigCodeInjections), nameof(SocializingConfigCodeInjections.ExperienceFromEvents_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(socializingConfigType, "ExperienceFromQuests"),
                postfix: new HarmonyMethod(typeof(SocializingConfigCodeInjections), nameof(SocializingConfigCodeInjections.ExperienceFromQuests_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(socializingConfigType, "LovedGiftExpMultiplier"),
                postfix: new HarmonyMethod(typeof(SocializingConfigCodeInjections), nameof(SocializingConfigCodeInjections.LovedGiftExpMultiplier_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(socializingConfigType, "BirthdayGiftExpMultiplier"),
                postfix: new HarmonyMethod(typeof(SocializingConfigCodeInjections), nameof(SocializingConfigCodeInjections.BirthdayGiftExpMultiplier_APMultiplier_Postfix))
            );
        }

        private void InjectArchaeologyExperienceMultiplier()
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.ARCHAEOLOGY))
            {
                return;
            }

            var excavationConfigType = AccessTools.TypeByName("ExcavationSkill.Config");
            _harmony.Patch(
                original: AccessTools.PropertyGetter(excavationConfigType, "ExperienceFromArtifactSpots"),
                postfix: new HarmonyMethod(typeof(ArchaeologyConfigCodeInjections), nameof(ArchaeologyConfigCodeInjections.ExperienceFromArtifactSpots_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(excavationConfigType, "ExperienceFromMinesDigging"),
                postfix: new HarmonyMethod(typeof(ArchaeologyConfigCodeInjections), nameof(ArchaeologyConfigCodeInjections.ExperienceFromBuriedAndPannedItem_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(excavationConfigType, "ExperienceFromBuriedAndPannedItem"),
                postfix: new HarmonyMethod(typeof(ArchaeologyConfigCodeInjections), nameof(ArchaeologyConfigCodeInjections.ExperienceFromMinesDigging_APMultiplier_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.PropertyGetter(excavationConfigType, "ExperienceFromWaterShifter"),
                postfix: new HarmonyMethod(typeof(ArchaeologyConfigCodeInjections), nameof(ArchaeologyConfigCodeInjections.ExperienceFromWaterShifter_APMultiplier_Postfix))
            );
        }

        private void AddDeepWoodsModInjections()
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.DEEP_WOODS))
            {
                return;
            }

            var _deepWoodsType = AccessTools.TypeByName("DeepWoodsMod.DeepWoods");
            var _enterDirectionType = AccessTools.TypeByName("DeepWoodsMod.DeepWoodsEnterExit+EnterDirection");
            var constructorParameterTypes = new[] { _deepWoodsType, typeof(int), _enterDirectionType };
            var _unicornType = AccessTools.TypeByName("DeepWoodsMod.Unicorn");
            var _gingerbreadType = AccessTools.TypeByName("DeepWoodsMod.GingerBreadHouse");
            var _iridiumtreeType = AccessTools.TypeByName("DeepWoodsMod.IridiumTree");
            var _treasureType = AccessTools.TypeByName("DeepWoodsMod.TreasureChest");
            var _fountainType = AccessTools.TypeByName("DeepWoodsMod.HealingFountain");
            var _infestedType = AccessTools.TypeByName("DeepWoodsMod.InfestedTree");

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
            _harmony.Patch(
                original: AccessTools.Method(_infestedType, "DeInfest"),
                postfix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.Deinfest_DeinfestLocation_Postfix))
            );
            if (_archipelago.SlotData.ElevatorProgression != ElevatorProgression.Vanilla)
            {
                _harmony.Patch(
                    original: AccessTools.Constructor(_deepWoodsType, constructorParameterTypes),
                    postfix: new HarmonyMethod(typeof(DeepWoodsModInjections), nameof(DeepWoodsModInjections.Constructor_WoodsDepthChecker_Postfix))
                );
            }
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

        private void AddSkullCavernElevatorModInjections()
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.SKULL_CAVERN_ELEVATOR))
            {
                return;
            }

            if (_archipelago.SlotData.ElevatorProgression == ElevatorProgression.Vanilla)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.enterMine)),
                postfix: new HarmonyMethod(typeof(SkullCavernInjections), nameof(SkullCavernInjections.EnterMine_SendSkullCavernElevatorCheck_PostFix))
            );

            var constructorParameterTypes = new[] { typeof(int), typeof(double), typeof(int) };
            var myElevatorMenuType = AccessTools.TypeByName("MyElevatorMenu");
            var myElevatorMenuConstructor = AccessTools.Constructor(myElevatorMenuType, constructorParameterTypes);
            _harmony.Patch(
                original: myElevatorMenuConstructor,
                prefix: new HarmonyMethod(typeof(SkullCavernInjections), nameof(SkullCavernInjections.MyElevatorMenuConstructor_SkullCavernElevator_Prefix)),
                postfix: new HarmonyMethod(typeof(SkullCavernInjections), nameof(SkullCavernInjections.MyElevatorMenuConstructor_SkullCavernElevator_Postfix))
            );

            var myElevatorMenuWithScrollBarType = AccessTools.TypeByName("MyElevatorMenuWithScrollbar");
            var myElevatorMenuWithScrollBarConstructor = AccessTools.Constructor(myElevatorMenuWithScrollBarType, constructorParameterTypes);
            _harmony.Patch(
                original: myElevatorMenuWithScrollBarConstructor,
                prefix: new HarmonyMethod(typeof(SkullCavernInjections), nameof(SkullCavernInjections.MyElevatorMenuConstructor_SkullCavernElevator_Prefix)),
                postfix: new HarmonyMethod(typeof(SkullCavernInjections), nameof(SkullCavernInjections.MyElevatorMenuConstructor_SkullCavernElevator_Postfix))
            );
        }
    }
}
