/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using AlwaysShowBarValues.UIElements;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Reflection;

namespace AlwaysShowBarValues.Integrations
{
    internal class SurvivalisticIntegration : StatBox
    {

        private object? Data;
        private Type? DataType { 
            get
            {
                IsValid = false;
                if (ModInstance is null || ModInstance.GetType() is not Type modType) return null;
                if (modType.GetProperty("Data") is not PropertyInfo dataProperty) return null;
                if (dataProperty.GetValue(ModInstance) is not object survivalisticData) return null;
                Data = survivalisticData;
                if (survivalisticData.GetType() is not Type dataType) return null;
                IsValid = true;
                return dataType;
            }
        }

        public SurvivalisticIntegration() : base(KeybindList.Parse("L"), "Survivalistic")
        {
            TopValue = new("Hunger", new Rectangle(10, 428, 10, 10), new Vector2(2f, 4f));
            BottomValue = new("Thirst", new Rectangle(372, 362, 9, 9), new Vector2(2f, 5f));
        }

        public override bool UpdateCurrentStats()
        {
            if (TopValue is null || BottomValue is null) return false;
            if (DataType is null || !IsValid) return false;
            if (!(DataType.GetProperty("ActualHunger") is PropertyInfo currentHungerInfo
                && DataType.GetProperty("MaxHunger") is PropertyInfo maxHungerInfo
                && DataType.GetProperty("ActualThirst") is PropertyInfo currentThirstInfo
                && DataType.GetProperty("MaxThirst") is PropertyInfo maxThirstInfo))
                return false;
            if (!(currentHungerInfo.GetValue(Data) is float currentHunger
                && maxHungerInfo.GetValue(Data) is float maxHunger
                && currentThirstInfo.GetValue(Data) is float currentThirst
                && maxThirstInfo.GetValue(Data) is float maxThirst))
                return false;
            TopValue.CurrentValue = currentHunger;
            TopValue.MaxValue = maxHunger;
            BottomValue.CurrentValue = currentThirst;
            BottomValue.MaxValue = maxThirst;
            return true;
        }
    }
}
