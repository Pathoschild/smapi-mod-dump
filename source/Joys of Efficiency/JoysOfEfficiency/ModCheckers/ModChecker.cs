using StardewModdingAPI;

namespace JoysOfEfficiency.ModCheckers
{
    public class ModChecker
    {
        public static bool IsCoGLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("punyo.CasksOnGround");
        public static bool IsCaLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("CasksAnywhere");
        public static bool IsCcLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
    }
}
