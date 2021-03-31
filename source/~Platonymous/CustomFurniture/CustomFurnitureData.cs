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
    public class CustomFurnitureData
    {
        public int id { get; set; }
        public string texture { get; set; }

        public string textureOverlay { get; set; }

        public string textureUnderlay { get; set; }


        public bool fromContent { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int price { get; set; }
        public int index { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int rotatedHeight { get; set; }
        public int rotatedWidth { get; set; }
        public int rotatedBoxHeight { get; set; }
        public int rotatedBoxWidth { get; set; }
        public int boxWidth { get; set; }
        public int boxHeight { get; set; }
        public int rotations { get; set; }
        public int setWidth { get; set; }
        public int animationFrames { get; set; }
        public int fps { get; set; }
        public string folderName { get; set; }
        public string shopkeeper { get; set; }
        public bool sellAtShop { get; set; }
        public string conditions { get; set; }
        public string instantGift { get; set; }

        public CustomFurnitureData()
        {
            id = 0;
            instantGift = "none";
            shopkeeper = "Robin";
            conditions = "none";
            sellAtShop = true;
            texture = "example.png";
            textureOverlay = null;
            textureUnderlay = null;
            name = "Furniture";
            description = "Furniture";
            type = "decor";
            price = 100;
            index = 0;
            width = 1;
            height = 2;
            rotatedHeight = -1;
            rotatedWidth = -1;
            rotatedBoxHeight = -1;
            rotatedBoxWidth = -1;
            boxWidth = 1;
            boxHeight = 1;
            rotations = 1;
            setWidth = -1;
            animationFrames = 0;
            fps = 6;
            folderName = "Example";
            fromContent = false;
        }
    }
}
