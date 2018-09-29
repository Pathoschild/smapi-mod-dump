using StardewModdingAPI;

namespace JoysOfEfficiency.ModCheckers
{
    public class ModChecker
    {
        public static bool IsCoGLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("punyo.CasksOnGround");
        public static bool IsCCLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("aEnigma.ConvenientChests");
    }
}
