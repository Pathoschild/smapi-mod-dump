using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceCore;

namespace EquivalentExchange
{
    class ProfessionHelper
    {
        public const int OldShaperId = 61;
        public const int OldSageId = 62;
        public const int OldTransmuterId = 63;
        public const int OldAdeptId = 64;
        public const int OldAurumancerId = 65;
        public const int OldConduitId = 66;

        public static List<int> DeprecatedProfessionIntegers = new List<int> { OldShaperId, OldSageId, OldTransmuterId, OldAdeptId, OldAurumancerId, OldConduitId };

        public static void CleanDeprecatedProfessions()
        {
            foreach(var deprecatedProfession in DeprecatedProfessionIntegers)
            {
                if (EquivalentExchange.HasProfession(deprecatedProfession))
                {
                    Game1.player.professions.Remove(deprecatedProfession);
                    switch (deprecatedProfession)
                    {
                        case OldShaperId:
                            Game1.player.professions.Add(AlchemySkill.ProfessionShaper.GetVanillaId());
                            break;
                        case OldSageId:
                            Game1.player.professions.Add(AlchemySkill.ProfessionSage.GetVanillaId());
                            break;
                        case OldTransmuterId:
                            Game1.player.professions.Add(AlchemySkill.ProfessionTransmuter.GetVanillaId());
                            break;
                        case OldAdeptId:
                            Game1.player.professions.Add(AlchemySkill.ProfessionAdept.GetVanillaId());
                            break;
                        case OldAurumancerId:
                            Game1.player.professions.Add(AlchemySkill.ProfessionAurumancer.GetVanillaId());
                            break;
                        case OldConduitId:
                            Game1.player.professions.Add(AlchemySkill.ProfessionConduit.GetVanillaId());
                            break;
                    }
                }
            }
        }
        
        //return first match on professions for levels 5 and 10
        public static List<int> GetProfessionsForSkillLevel(int whichLevel)
        {
            List<int> obtainedProfessionsAtLevel = new List<int>();
            switch (whichLevel)
            {
                case 5:
                    if (Game1.player.professions.Contains(ProfessionHelper.OldSageId))
                        obtainedProfessionsAtLevel.Add(ProfessionHelper.OldSageId);
                    if (Game1.player.professions.Contains(ProfessionHelper.OldShaperId))
                        obtainedProfessionsAtLevel.Add(ProfessionHelper.OldShaperId);
                    break;
                default:
                    if (Game1.player.professions.Contains(ProfessionHelper.OldTransmuterId))
                        obtainedProfessionsAtLevel.Add(ProfessionHelper.OldTransmuterId);
                    if (Game1.player.professions.Contains(ProfessionHelper.OldAdeptId))
                        obtainedProfessionsAtLevel.Add(ProfessionHelper.OldAdeptId);
                    if (Game1.player.professions.Contains(ProfessionHelper.OldAurumancerId))
                        obtainedProfessionsAtLevel.Add(ProfessionHelper.OldAurumancerId);
                    if (Game1.player.professions.Contains(ProfessionHelper.OldConduitId))
                        obtainedProfessionsAtLevel.Add(ProfessionHelper.OldConduitId);
                    break;
            }
            return obtainedProfessionsAtLevel;
        }


        public static List<int> firstRankProfessions = new List<int> { ProfessionHelper.OldShaperId, ProfessionHelper.OldSageId };
        public static List<int> secondRankProfessions = new List<int> { ProfessionHelper.OldTransmuterId, ProfessionHelper.OldAdeptId, ProfessionHelper.OldAurumancerId, ProfessionHelper.OldConduitId };

        public static int getProfessionForSkill(int level)
        {
            if (level != 5 && level != 10)
                return -1;

            List<int> list = (level == 5 ? firstRankProfessions : secondRankProfessions);
            foreach (int prof in list)
            {
                if (Game1.player.professions.Contains(prof))
                    return prof;
            }

            return -1;
        }

        //called when selecting a profession from the level up menu, sets the chosen profession for icon purposes.
        internal static void EnableAlchemistProfession(int profession)
        {
            switch (profession)
            {
                case ProfessionHelper.OldShaperId:
                    Game1.player.professions.Add(ProfessionHelper.OldShaperId);
                    break;
                case ProfessionHelper.OldSageId:
                    Game1.player.professions.Add(ProfessionHelper.OldSageId);
                    break;
                case ProfessionHelper.OldTransmuterId:
                    Game1.player.professions.Add(ProfessionHelper.OldTransmuterId);
                    break;
                case ProfessionHelper.OldAdeptId:
                    Game1.player.professions.Add(ProfessionHelper.OldAdeptId);
                    break;
                case ProfessionHelper.OldAurumancerId:
                    Game1.player.professions.Add(ProfessionHelper.OldAurumancerId);
                    break;
                case ProfessionHelper.OldConduitId:
                    Game1.player.professions.Add(ProfessionHelper.OldConduitId);
                    break;
            }
        }

        public static string GetProfessionInternalName(int whichProfession)
        {
            switch (whichProfession)
            {
                case ProfessionHelper.OldShaperId:
                    return "Shaper";
                case ProfessionHelper.OldSageId:
                    return "Sage";
                case ProfessionHelper.OldTransmuterId:
                    return "Transmuter";
                case ProfessionHelper.OldAdeptId:
                    return "Adept";
                case ProfessionHelper.OldAurumancerId:
                    return "Aurumancer";
                case ProfessionHelper.OldConduitId:
                    return "Conduit";
                default:
                    return null;
            }
        }

        public static string GetProfessionTitleFromNumber (int whichProfession)
        {
            switch (whichProfession)
            {
                case ProfessionHelper.OldShaperId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.Shaper) }";
                case ProfessionHelper.OldSageId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.Sage) }";
                case ProfessionHelper.OldTransmuterId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.Transmuter) }";
                case ProfessionHelper.OldAdeptId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.Adept) }";
                case ProfessionHelper.OldAurumancerId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.Aurumancer) }";
                case ProfessionHelper.OldConduitId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.Conduit) }";
                default:
                    return null;
            }
        }

        public static string GetProfessionDescription(int whichProfession)
        {
            switch (whichProfession)
            {
                case ProfessionHelper.OldShaperId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.ShaperDescription) }";
                case ProfessionHelper.OldSageId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.SageDescription) }";
                case ProfessionHelper.OldTransmuterId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.TransmuterDescription) }";
                case ProfessionHelper.OldAdeptId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.AdeptDescription) }";
                case ProfessionHelper.OldAurumancerId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.AurumancerDescription) }";
                case ProfessionHelper.OldConduitId:
                    return $"{ EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.ConduitDescription) }";
            }
            return "";
        }
    }
}
