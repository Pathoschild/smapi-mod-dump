/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/


namespace NermNermNerm.Stardew.QuestableTractor
{
    public enum RestorationState
    {
        TalkToLewis,
        TalkToSebastian,
        TalkToLewisAgain,
        WaitingForMailFromRobinDay1,
        WaitingForMailFromRobinDay2,
        BuildTractorGarage,
        WaitingForSebastianDay1,
        WaitingForSebastianDay2,
        TalkToWizard,
        BringStuffToForest,
        BringEngineToSebastian,
        BringEngineToMaru,
        WaitForEngineInstall,
        Complete,
    }

    public static class RestorationStateExtensions
    {
        public static bool IsDerelictInTheFields(this RestorationState _this)
            => _this <= RestorationState.BuildTractorGarage;

        public static bool IsDerelictInTheGarage(this RestorationState _this)
            => _this > RestorationState.BuildTractorGarage && _this < RestorationState.Complete;

        public static bool CanBuildGarage(this RestorationState _this)
            => _this >= RestorationState.BuildTractorGarage;

    }
}
