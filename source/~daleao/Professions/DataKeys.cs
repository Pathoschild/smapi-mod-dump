/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions;

internal sealed class DataKeys
{
    // farmer keys
    internal const string EcologistVarietiesForaged = "EcologistVarietiesForaged";
    internal const string PrestigedEcologistBuffLookup = "PrestigedEcologistBuffLookup";
    internal const string GemologistMineralsStudied = "GemologistMineralsStudied";
    internal const string ProspectorHuntStreak = "ProspectorHuntStreak";
    internal const string ScavengerHuntStreak = "ScavengerHuntStreak";
    internal const string ConservationistTrashCollectedThisSeason = "ConservationistTrashCollectedThisSeason";
    internal const string ConservationistActiveTaxDeduction = "ConservationistActiveTaxDeduction";
    internal const string ForgottenRecipesDict = "ForgottenRecipesDict";
    internal const string LimitBreakId = "LimitBreakId";
    internal const string OrderedProfessions = "OrderedProfessions";

    // object keys
    internal const string FirstMemorizedTackle = "FirstMemorizedTackle";
    internal const string FirstMemorizedTackleUses = "FirstMemorizedTackleUses";
    internal const string SecondMemorizedTackle = "SecondMemorizedTackle";
    internal const string SecondMemorizedTackleUses = "SecondMemorizedTackleUses";
    internal const string MuskUses = "MuskTimesUsed";
    internal const string PersistedCoals = "PersistedCoals";

    // terrain feature and crop keys
    internal const string PlantedByArborist = "PlantedByArborist";
    internal const string PlantedByPrestigedAgriculturist = "PlantedByAgriculturist";
    internal const string DatePlanted = "DatePlanted";
    internal const string Fertilized = "Fertilized";

    // farm animal keys
    internal const string BredByPrestigedBreeder = "BredByBreeder";

    // building keys
    internal const string FamilyLivingHere = "FamilyLivingHere";
}
