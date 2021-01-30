/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
	public class BootRandomizer
	{
		public static Dictionary<int, BootItem> Boots = new Dictionary<int, BootItem>();

		/// <summary>
		/// Randomizes boots - currently only changes defense and immunity
		/// </summary>
		/// <returns />
		public static Dictionary<int, string> Randomize()
		{
			Boots.Clear();
			WeaponAndArmorNameRandomizer nameRandomizer = new WeaponAndArmorNameRandomizer();
			List<string> descriptions = NameAndDescriptionRandomizer.GenerateBootDescriptions(BootData.AllBoots.Count);

			Dictionary<int, string> bootReplacements = new Dictionary<int, string>();
			List<BootItem> bootsToUse = new List<BootItem>();

			for (int i = 0; i < BootData.AllBoots.Count; i++)
			{
				BootItem originalBoot = BootData.AllBoots[i];
				int statPool = Globals.RNGGetIntWithinPercentage(originalBoot.Defense + originalBoot.Immunity, 30);
				int defense = Range.GetRandomValue(0, statPool);
				int immunity = statPool - defense;

				if ((defense + immunity) == 0)
				{
					if (Globals.RNGGetNextBoolean())
					{
						defense = 1;
					}

					else
					{
						immunity = 1;
					}
				}

				BootItem newBootItem = new BootItem(
					originalBoot.Id,
					nameRandomizer.GenerateRandomBootName(),
					descriptions[i],
					originalBoot.NotActuallyPrice,
					defense,
					immunity,
					originalBoot.ColorSheetIndex
				);

				bootsToUse.Add(newBootItem);
				Boots.Add(newBootItem.Id, newBootItem);
			}

			foreach (BootItem bootToAdd in bootsToUse)
			{
				bootReplacements.Add(bootToAdd.Id, bootToAdd.ToString());
			}

			WriteToSpoilerLog(bootsToUse);
			return bootReplacements;
		}

		/// <summary>
		/// Writes the boots to the spoiler log
		/// </summary>
		/// <param name="bootsToUse">The boot data that was used</param>
		public static void WriteToSpoilerLog(List<BootItem> bootsToUse)
		{
			if (!Globals.Config.Boots.Randomize) { return; }

			Globals.SpoilerWrite("==== BOOTS ====");
			foreach (BootItem bootToAdd in bootsToUse)
			{
				Globals.SpoilerWrite($"{bootToAdd.Name}: +{bootToAdd.Defense} defense; +{bootToAdd.Immunity} immunity");
			}
			Globals.SpoilerWrite("");
		}
	}
}
