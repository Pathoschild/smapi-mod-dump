/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using StardewValley.GameData.Objects;

namespace AnimalHusbandryMod.meats
{
    public static class MeatExtension
    {
        public static string GetDescription(this Meat value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static ObjectData GetObjectData(this Meat value)
        {
            var meatItem = DataLoader.MeatData.getMeatItem(value);
            var i18n = DataLoader.i18n;
            return new ObjectData()
            {
                SpriteIndex = (int) value,
                Name = value.GetDescription(),
                Price = meatItem.Price,
                Edibility = meatItem.Edibility,
                DisplayName = i18n.Get($"Meat.{value}.Name"),
                Description = i18n.Get($"Meat.{value}.Description"),
                Category = -14,
                Type = "Basic"
            };
        }        
    }
}
