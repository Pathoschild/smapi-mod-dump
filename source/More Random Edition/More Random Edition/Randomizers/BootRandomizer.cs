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
		public readonly static Dictionary<int, BootItem> Boots = new();

        /// <summary>
        /// The data from Data/Boots.xnb
        /// </summary>
        public static Dictionary<int, string> BootData { get; private set; }

        /// <summary>
        /// Randomizes boots - currently only changes defense and immunity
        /// </summary>
        /// <returns />
        public static Dictionary<int, string> Randomize()
		{
            // Initialize boot data here so that it's reloaded in case of a locale change
            BootData = Globals.ModRef.Helper.GameContent
                .Load<Dictionary<int, string>>("Data/Boots");
			Boots.Clear();

			WeaponAndArmorNameRandomizer nameRandomizer = new();
			List<string> descriptions = 
				NameAndDescriptionRandomizer.GenerateBootDescriptions(BootData.Count);

			Dictionary<int, string> bootReplacements = new();
			List<BootItem> bootsToUse = new();

			int index = 0;
			foreach (KeyValuePair<int, string> bootData in BootData)
			{
				string[] bootStringData = bootData.Value.Split("/");
				int originalDefense = int.Parse(bootStringData[(int)BootIndexes.Defense]);
                int originalImmunity = int.Parse(bootStringData[(int)BootIndexes.Immunity]);

                int statPool = Globals.RNGGetIntWithinPercentage(originalDefense + originalImmunity, 30);
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

				BootItem newBootItem = new(
					bootData.Key,
					nameRandomizer.GenerateRandomBootName(),
					descriptions[index],
					defense,
					immunity);

				bootsToUse.Add(newBootItem);
				Boots.Add(newBootItem.Id, newBootItem);

				index++;
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
