/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drachenkaetzchen/AdvancedKeyBindings
**
*************************************************/

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