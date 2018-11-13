using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.IO;

namespace DeepWoodsMod
{
    /// <summary>
    /// Credit to https://www.reddit.com/user/sinceriouslyyy for their pixel art fountain: https://www.reddit.com/r/PixelArt/comments/6h8wld/oc_pixel_art_fountain/
    /// Credit to https://forums.nexusmods.com/index.php?/user/21105194-mostlyreal/ for their ginger bread house.
    /// </summary>
    class Textures
    {
        public static Texture2D woodsObelisk;
        public static Texture2D healingFountain;
        public static Texture2D iridiumTree;
        public static Texture2D gingerbreadHouse;
        public static Texture2D bushThorns;
        public static Texture2D unicorn;
        public static Texture2D excaliburStone;
        public static Texture2D lakeTilesheet;
        public static Texture2D festivals;

        public static void LoadAll()
        {
            woodsObelisk = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "woods_obelisk_mostlyreal.png"), ContentSource.ModFolder);
            healingFountain = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "sinceriouslyyy_fountain.png"), ContentSource.ModFolder);
            iridiumTree = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "iridium_tree.png"), ContentSource.ModFolder);
            gingerbreadHouse = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "gingerbread_house_mostlyreal.png"), ContentSource.ModFolder);
            bushThorns = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "bush_thorns.png"), ContentSource.ModFolder);
            unicorn = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "unicorn_zhuria.png"), ContentSource.ModFolder);
            excaliburStone = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "excalibur_stone.png"), ContentSource.ModFolder);
            lakeTilesheet = ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "lake_tilesheet.png"), ContentSource.ModFolder);
            festivals = Game1.content.Load<Texture2D>("Maps\\Festivals");
        }
    }
}
