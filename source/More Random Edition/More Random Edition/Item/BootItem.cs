/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Represents a boot - doesn't use the same IDs as normal items, but we'll treat it like an item
    /// </summary>
    public class BootItem : Item
	{
		public string Description { get; set; }
		public int Defense { get; set; }
		public int Immunity { get; set; }

        public BootItem(
			int id,
			string name,
			int defense,
			int immunity) : base(id)
        {
            DifficultyToObtain = ObtainingDifficulties.NonCraftingItem;
            CanStack = false;
            ShouldBeForagable = false;

            OverrideName = name;
            Description = BootRandomizer.BootData[id].Split("/")[(int)BootIndexes.Description];
            Defense = defense;
            Immunity = immunity;
        }

        public BootItem(
			int id,
			string name,
			string description,
			int defense,
			int immunity) : this(id, name, defense, immunity)
		{
			if (Globals.ModRef.Helper.Translation.LocaleEnum == LocalizedContentManager.LanguageCode.en)
			{
				Description = description;
			}
		}

		/// <summary>
		/// Gets the sale price of the boot
		/// Equal to the sell price * 8
		/// </summary>
		/// <returns />
		public int GetBuyPrice()
		{
			return Math.Max(50 * 8 * (Defense + Immunity), 100);
		}

		/// <summary>
		/// The string to use for asset replacement
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			List<string> originalData = BootRandomizer.BootData[Id].Split("/").ToList();
            originalData[(int)BootIndexes.Name] = OverrideName;
            originalData[(int)BootIndexes.Description] = Description;
			originalData[(int)BootIndexes.Price] = GetBuyPrice().ToString();
            originalData[(int)BootIndexes.Defense] = Defense.ToString();
            originalData[(int)BootIndexes.Immunity] = Immunity.ToString();

			// The display name field does not exist in English
			int displayNameIndex = (int)BootIndexes.DisplayName;
            if (originalData.Count > (int)BootIndexes.DisplayName)
			{
				originalData[displayNameIndex] = OverrideName;
			} 

            return string.Join("/", originalData);
		}
	}
}
