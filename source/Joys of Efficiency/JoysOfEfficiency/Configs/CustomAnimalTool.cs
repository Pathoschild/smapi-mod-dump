/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

namespace JoysOfEfficiency.Configs
{
    class CustomAnimalTool
    {
        public string Name { get; set; }
        public ToolType ToolType { get; set; }

        public CustomAnimalTool(string name, ToolType toolType)
        {
            Name = name;
            ToolType = toolType;
        }
    }

    public enum ToolType
    {
        None,
        Bucket,
        Shears
    }
}
