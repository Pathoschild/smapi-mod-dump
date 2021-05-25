/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nofilenamed/AnimalObserver
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AnimalObserver
{
    internal class ModConfig
    {
        public bool PetToo { get; set; } = true;

        public bool ShowIsHarvestable { get; set; } = true;

        public bool ShowTruffle { get; set; } = true;

        public byte ShowIsHarvestableTime { get; set; } = 5;


        

        public KeySetting Keys { get; set; } = new KeySetting();      

        public EntitiesSetting Entities { get; set; } = new EntitiesSetting();

        public OffsetSetting Offsets { get; set; } = new OffsetSetting();        
    }

    internal class EntitiesSetting
    {
        public EntitiesConfig Bubble { get; set; } = new EntitiesConfig()
        {
            X = 141,
            Y = 465,
            Width = 20,
            Height = 24,
            Scale = 2f,
            Color = Color.White * 0.65f
        };

        public EntitiesConfig Heart { get; set; } = new EntitiesConfig()
        {
            X = 172,
            Y = 514,
            Width = 9,
            Height = 10,
            Scale = 3f,
            Offset = new Vector2(7f, 7f)
        };

        public EntitiesConfig CowMilk { get; set; } = new EntitiesConfig()
        {
            X = 256,
            Y = 112,
            Width = 16,
            Height = 16,
            Scale = 2f,
            Offset = new Vector2(4f, 4f)
        };

        public EntitiesConfig GoatMilk { get; set; } = new EntitiesConfig()
        {
            X = 64,
            Y = 288,
            Width = 16,
            Height = 16,
            Scale = 2f,
            Offset = new Vector2(4f, 4f)
        };

        public EntitiesConfig Truffle { get; set; } = new EntitiesConfig()
        {
            X = 352,
            Y = 272,
            Width = 16,
            Height = 16,
            Scale = 2f,
            Offset = new Vector2(4f, 4f)
        };

        public EntitiesConfig Wool { get; set; } = new EntitiesConfig()
        {
            X = 128,
            Y = 288,
            Width = 16,
            Height = 16,
            Scale = 2f,
            Offset = new Vector2(4f, 4f)
        };
    }

    internal class OffsetSetting
    {
        public Vector2 BarnAnimal { get; set; } = new Vector2(32f, -64f);

        public Vector2 CoopAnimal { get; set; } = new Vector2(0f, -64f);

        public Vector2 Pet { get; set; } = new Vector2(32f, -64f);
    }


    internal class KeySetting
    {
        public KeybindList Reload { get; set; } = new KeybindList(SButton.F9);

        public KeybindList Toggle { get; set; } = new KeybindList(SButton.F10);
    }
}
