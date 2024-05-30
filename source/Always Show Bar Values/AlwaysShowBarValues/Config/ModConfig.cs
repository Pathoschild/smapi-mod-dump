/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using AlwaysShowBarValues.Integrations;
using AlwaysShowBarValues.StatBoxes;
using AlwaysShowBarValues.UIElements;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Audio;

namespace AlwaysShowBarValues.Config
{
    public sealed class ModConfig
    {
        private readonly Dictionary<string, StatBox> StatBoxes = new()
        {
            {"main", new MainBox() },
            {"Survivalistic", new SurvivalisticIntegration() }
        };

        public bool Enabled
        {
            get { return StatBoxes["main"].Enabled; }
            set { StatBoxes["main"].Enabled = value; }
        }

        public string BoxStyle
        {
            get { return StatBoxes["main"].BoxStyle; }
            set { StatBoxes["main"].BoxStyle = value; }
        }

        public string Position
        {
            get { return StatBoxes["main"].Position; }
            set { StatBoxes["main"].Position = value; }
        }
        public int X
        {
            get { return StatBoxes["main"].X; }
            set { StatBoxes["main"].X = value; }
        }
        public int Y
        {
            get { return StatBoxes["main"].Y; }
            set { StatBoxes["main"].Y = value; }
        }
        public string HealthColorMode
        {
            get { return StatBoxes["main"].TopValue.ColorMode; }
            set { StatBoxes["main"].TopValue.ColorMode = value; }
        }
        public string StaminaColorMode
        {
            get { return StatBoxes["main"].BottomValue.ColorMode; }
            set { StatBoxes["main"].BottomValue.ColorMode = value; }
        }
        public bool Above
        {
            get { return StatBoxes["main"].Above; }
            set { StatBoxes["main"].Above = value; }
        }
        public bool TextShadow
        {
            get { return StatBoxes["main"].TextShadow; }
            set { StatBoxes["main"].TextShadow = value; }
        }
        public KeybindList ToggleKey
        {
            get { return StatBoxes["main"].ToggleKey; }
            set { StatBoxes["main"].ToggleKey = value; }
        }

        public string MaxHealthHex
        {
            get { return StatBoxes["main"].TopValue.Colors["max"].HexCode; }
            set { StatBoxes["main"].TopValue.Colors["max"].HexCode = value; }
        }
        public string MiddleHealthHex
        {
            get { return StatBoxes["main"].TopValue.Colors["middle"].HexCode; }
            set { StatBoxes["main"].TopValue.Colors["middle"].HexCode = value; }
        }
        public string MinHealthHex
        {
            get { return StatBoxes["main"].TopValue.Colors["min"].HexCode; }
            set { StatBoxes["main"].TopValue.Colors["min"].HexCode = value; }
        }
        public string MaxStaminaHex
        {
            get { return StatBoxes["main"].BottomValue.Colors["max"].HexCode; }
            set { StatBoxes["main"].BottomValue.Colors["max"].HexCode = value; }
        }
        public string MiddleStaminaHex
        {
            get { return StatBoxes["main"].BottomValue.Colors["middle"].HexCode; }
            set { StatBoxes["main"].BottomValue.Colors["middle"].HexCode = value; }
        }
        public string MinStaminaHex
        {
            get { return StatBoxes["main"].BottomValue.Colors["min"].HexCode; }
            set { StatBoxes["main"].BottomValue.Colors["min"].HexCode = value; }
        }

        public bool LeftIcon
        {
            get { return StatBoxes["main"].IconsLeftOfString; }
            set { StatBoxes["main"].IconsLeftOfString = value; }
        }

        // Survivalistic
        public bool SurvivalisticEnabled
        {
            get { return StatBoxes["Survivalistic"].Enabled; }
            set { StatBoxes["Survivalistic"].Enabled = value; }
        }
        public string SurvivalisticBoxStyle
        {
            get { return StatBoxes["Survivalistic"]?.BoxStyle ?? "Round"; }
            set { StatBoxes["Survivalistic"].BoxStyle = value; }
        }
        public string SurvivalisticPosition
        {
            get { return StatBoxes["Survivalistic"]?.Position ?? "Top Left"; }
            set { StatBoxes["Survivalistic"].Position = value; }
        }
        public int SurvivalisticX
        {
            get { return StatBoxes["Survivalistic"]?.X ?? 0; }
            set { StatBoxes["Survivalistic"].X = value; }
        }
        public int SurvivalisticY
        {
            get { return StatBoxes["Survivalistic"]?.Y ?? 0; }
            set { StatBoxes["Survivalistic"].Y = value; }
        }
        public string SurvivalisticHealthColorMode
        {
            get { return StatBoxes["Survivalistic"].TopValue.ColorMode ?? "Black"; }
            set { StatBoxes["Survivalistic"].TopValue.ColorMode = value; }
        }
        public string SurvivalisticStaminaColorMode
        {
            get { return StatBoxes["Survivalistic"]?.BottomValue?.ColorMode ?? "Black"; }
            set { StatBoxes["Survivalistic"].BottomValue.ColorMode = value; }
        }
        public bool SurvivalisticAbove
        {
            get { return StatBoxes["Survivalistic"]?.Above ?? true; }
            set { StatBoxes["Survivalistic"].Above = value; }
        }
        public bool SurvivalisticTextShadow
        {
            get { return StatBoxes["Survivalistic"]?.TextShadow ?? true; }
            set { StatBoxes["Survivalistic"].TextShadow = value; }
        }
        public KeybindList SurvivalisticToggleKey
        {
            get { return StatBoxes["Survivalistic"]?.ToggleKey ?? KeybindList.Parse("L"); }
            set { StatBoxes["Survivalistic"].ToggleKey = value; }
        }

        public string SurvivalisticMaxHealthHex
        {
            get { return StatBoxes["Survivalistic"]?.TopValue.Colors["max"].HexCode ?? "#000000"; }
            set { StatBoxes["Survivalistic"].TopValue.Colors["max"].HexCode = value; }
        }
        public string SurvivalisticMiddleHealthHex
        {
            get { return StatBoxes["Survivalistic"]?.TopValue.Colors["middle"].HexCode ?? "#000000"; }
            set { StatBoxes["Survivalistic"].TopValue.Colors["middle"].HexCode = value; }
        }
        public string SurvivalisticMinHealthHex
        {
            get { return StatBoxes["Survivalistic"]?.TopValue.Colors["min"].HexCode ?? "#000000"; }
            set { StatBoxes["Survivalistic"].TopValue.Colors["min"].HexCode = value; }
        }
        public string SurvivalisticMaxStaminaHex
        {
            get { return StatBoxes["Survivalistic"]?.BottomValue.Colors["max"].HexCode ?? "#000000"; }
            set { StatBoxes["Survivalistic"].BottomValue.Colors["max"].HexCode = value; }
        }
        public string SurvivalisticMiddleStaminaHex
        {
            get { return StatBoxes["Survivalistic"]?.BottomValue.Colors["middle"].HexCode ?? "#000000"; }
            set { StatBoxes["Survivalistic"].BottomValue.Colors["middle"].HexCode = value; }
        }
        public string SurvivalisticMinStaminaHex
        {
            get { return StatBoxes["Survivalistic"]?.BottomValue.Colors["min"].HexCode ?? "#000000"; }
            set { StatBoxes["Survivalistic"].BottomValue.Colors["min"].HexCode = value; }
        }
        public bool SurvivalisticLeftIcon
        {
            get { return StatBoxes["Survivalistic"].IconsLeftOfString; }
            set { StatBoxes["Survivalistic"].IconsLeftOfString = value; }
        }


        internal void AddInstanceToMod(string name, Mod modInstance)
        {
            StatBoxes[name].AddModInstance(modInstance);
        }

        internal List<StatBox> GetStatBoxes()
        {
            List<StatBox> result = new();
            foreach (KeyValuePair<string, StatBox> entry in StatBoxes)
            {
                if (entry.Key == "main") result.Add(entry.Value);
                else if (entry.Value.IsValid) result.Add(entry.Value);
            }
            return result;
        }
    }
}
