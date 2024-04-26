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
			string id,
			string name,
			int defense,
			int immunity) : base(id)
        {
            DifficultyToObtain = ObtainingDifficulties.NonCraftingItem;
            CanStack = false;
            ShouldBeForagable = false;

            OverrideName = name;
            Description = BootRandomizer.BootData[id.ToString()].Split("/")[(int)BootIndexes.Description];
            Defense = defense;
            Immunity = immunity;
        }

        public BootItem(
			string id,
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
			string[] originalData = BootRandomizer.BootData[Id.ToString()].Split("/");
            originalData[(int)BootIndexes.Name] = OverrideName;
            originalData[(int)BootIndexes.Description] = Description;
			originalData[(int)BootIndexes.Price] = GetBuyPrice().ToString();
            originalData[(int)BootIndexes.Defense] = Defense.ToString();
            originalData[(int)BootIndexes.Immunity] = Immunity.ToString();
            originalData[(int)BootIndexes.DisplayName] = OverrideName;

            return string.Join("/", originalData);
		}
	}
}
