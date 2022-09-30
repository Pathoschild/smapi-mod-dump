/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaddUtil;
using StardewModdingAPI;


namespace ChestPreview.Framework
{
    public static class Conversor
    {
        public static Size GetSizeFromConfigInt(int value)
        {
            switch (value)
            {
                case 1:
                    return Size.Small;
                case 2:
                    return Size.Medium;
                case 3:
                    return Size.Big;
                case 4:
                    return Size.Huge;
                default:
                    return Size.Big;
            }
        }

        public static string GetSizeStringFromEnum(Size size)
        {
            if (size == Size.Small)
            {
                return "Small";
            }
            else if (size == Size.Medium)
            {
                return "Medium";
            }
            else if (size == Size.Big)
            {
                return "Big";
            }
            else if (size == Size.Huge)
            {
                return "Huge";
            }
            else
            {
                return "Medium";
            }
        }

        public static Size GetSizeEnumFromString(string size)
        {
            if (size.Equals("Small"))
            {
                return Size.Small;
            }
            else if (size.Equals("Medium"))
            {
                return Size.Medium;
            }
            else if (size.Equals("Big"))
            {
                return Size.Big;
            }
            else if (size.Equals("Huge"))
            {
                return Size.Huge;
            }
            else
            {
                return Size.Medium;
            }
        }

        public static float GetCurrentSizeValue()
        {
            if (ModEntry.CurrentSize == Size.Small)
            {
                return 0.4f;
            }
            else if (ModEntry.CurrentSize == Size.Medium)
            {
                return 0.5f;
            }
            else if (ModEntry.CurrentSize == Size.Big)
            {
                return 0.6f;
            }
            else if (ModEntry.CurrentSize == Size.Huge)
            {
                return 0.7f;
            }
            else
            {
                return 0.5f;
            }
        }
        public static string GetSizeName(int value)
        {
            string name = "";
            switch (value)
            {
                case 1:
                    name = "Small";
                    break;
                case 2:
                    name = "Medium";
                    break;
                case 3:
                    name = "Big";
                    break;
                case 4:
                    name = "Huge";
                    break;
            }
            return name;
        }

        public static string GetSizeTrasnlationName(int value)
        {
            string name = "";
            switch (value)
            {
                case 1:
                    name = GetTranslationSize("Small");
                    break;
                case 2:
                    name = GetTranslationSize("Medium");
                    break;
                case 3:
                    name = GetTranslationSize("Big");
                    break;
                case 4:
                    name = GetTranslationSize("Huge");
                    break;
            }
            return name;
        }

        public static SButton GetMouseButton(string value)
        {
            SButton button = SButton.A;
            if (value.Equals("MouseLeft"))
            {
                button = SButton.MouseLeft;
            }
            else if (value.Equals("MouseRight"))
            {
                button = SButton.MouseRight;
            }
            else if (value.Equals("MouseMiddle"))
            {
                button = SButton.MouseMiddle;
            }
            else if (value.Equals("MouseX1"))
            {
                button = SButton.MouseX1;
            }
            else if (value.Equals("MouseX2"))
            {
                button = SButton.MouseX2;
            }
            return button;
        }

        public static string GetTranslationMouse(string value)
        {
            string translated;
            if (value.Equals("MouseLeft"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.mouse.left");
            }
            else if (value.Equals("MouseRight"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.mouse.right");
            }
            else if (value.Equals("MouseMiddle"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.mouse.middle");
            }
            else if (value.Equals("MouseX1"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.mouse.x1");
            }
            else
            {
                translated = Helpers.GetTranslationHelper().Get("config.mouse.x2");
            }
            return translated;
        }

        public static string GetTranslationSize(string value)
        {
            string translated;
            if (value.Equals("Small"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.size.small");
            }
            else if (value.Equals("Medium"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.size.medium");
            }
            else if (value.Equals("Big"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.size.big");
            }
            else if (value.Equals("Huge"))
            {
                translated = Helpers.GetTranslationHelper().Get("config.size.huge");
            }
            else
            {
                translated = Helpers.GetTranslationHelper().Get("config.size.medium");
            }
            return translated;
        }
    }
}
