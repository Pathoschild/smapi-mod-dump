/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace CustomFurniture
{
    class CustomFurniturePack
    {
        public string useid { get; set; } = "none";
        public string folderName { get; set; }
        public string fileName { get; set; }
        public string author { get; set; } = "none";
        public string version { get; set; } = "1.0.0";
        public string name { get; set; } = "Furniture Pack";
        public CustomFurnitureData[] furniture { get; set; }
        
        public CustomFurniturePack()
        {
            furniture = new CustomFurnitureData[] { new CustomFurnitureData(), new CustomFurnitureData(), new CustomFurnitureData(), new CustomFurnitureData(), new CustomFurnitureData() };
        }
    }
}
