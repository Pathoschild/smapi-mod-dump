using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;

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

        public static string GetObjectString(this Meat value)
        {
            var meatItem = DataLoader.MeatData.getMeatItem(value);
            var i18n = DataLoader.i18n;
            return String.Format("{0}/{1}/{2}/Basic -14/{3}/{4}", value.GetDescription(), meatItem.Price, meatItem.Edibility, i18n.Get($"Meat.{value}.Name"), i18n.Get($"Meat.{value}.Description"));
        }        
    }
}
