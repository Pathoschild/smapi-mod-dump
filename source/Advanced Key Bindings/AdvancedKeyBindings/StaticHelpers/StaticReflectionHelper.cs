using StardewModdingAPI;

namespace AdvancedKeyBindings.StaticHelpers
{
    public class StaticReflectionHelper
    {
        private static StaticReflectionHelper _staticReflectionHelper;
        private IReflectionHelper _reflectionHelper;
        
        private StaticReflectionHelper(IModHelper modHelper)
        {
            _reflectionHelper = modHelper.Reflection;
        }

        public IReflectionHelper GetReflector()
        {
            return _reflectionHelper;
        }

        public static StaticReflectionHelper GetInstance()
        {
            return _staticReflectionHelper;
        }
        
        public static void Initialize(IModHelper modHelper)
        {
            _staticReflectionHelper = new StaticReflectionHelper(modHelper);
        }
    }
}