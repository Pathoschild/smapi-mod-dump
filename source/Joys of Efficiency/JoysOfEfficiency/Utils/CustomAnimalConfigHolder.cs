/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using JoysOfEfficiency.Configs;

namespace JoysOfEfficiency.Utils
{
    internal class CustomAnimalConfigHolder : ConfigHolder<ConfigCustomAnimalTool>
    {
        public CustomAnimalConfigHolder(string filePath) : base(filePath)
        {
        }

        protected override ConfigCustomAnimalTool GetNewInstance()
        {
            return new ConfigCustomAnimalTool();
        }
    }
}
