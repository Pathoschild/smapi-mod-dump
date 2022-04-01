/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Util
{
    public class BlueprintInfo
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string BuildingType { get; }
        public int Cost { get; }

        public BlueprintInfo(string name, string displayName, string buildingType, int cost)
        {
            Name = name;
            DisplayName = displayName;
            BuildingType = buildingType;
            Cost = cost;
        }

        public bool IsAnimal()
        {
            return BuildingType == "Animal";
        }
    }
}
