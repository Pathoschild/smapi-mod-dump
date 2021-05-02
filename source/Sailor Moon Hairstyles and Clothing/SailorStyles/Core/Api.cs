/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

namespace SailorStyles
{
	public interface IApi
	{
		bool AreHairstylesEnabled();
		int GetHairstylesInitialIndex();
		string GetCharacterForHairstyle(int hairstyle);
	}

	public class Api : IApi
	{
		public bool AreHairstylesEnabled()
		{
			return ModEntry.Config.EnableHairstyles;
		}

		public int GetHairstylesInitialIndex()
		{
			return ModConsts.HairstylesInitialIndex;
		}

		public string GetCharacterForHairstyle(int hairstyle)
		{
			string chara = null;
			int index = ModConsts.HairstylesInitialIndex;
			hairstyle -= index;

			if (index < 0 || hairstyle < 0)
				return null;

			switch (hairstyle)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					chara = "moon";
					break;
				case 4:
				case 5:
				case 6:
				case 7:
					chara = "chibi";
					break;
				case 8:
				case 9:
					chara = "mercury";
					break;
				case 10:
				case 11:
					chara = "mars";
					break;
				case 12:
				case 13:
					chara = "jupiter";
					break;
				case 14:
				case 15:
					chara = "venus";
					break;
				case 16:
				case 17:
					chara = "uranus";
					break;
				case 18:
				case 19:
					chara = "neptune";
					break;
				case 20:
				case 21:
					chara = "pluto";
					break;
				case 22:
				case 23:
					chara = "saturn";
					break;
			}
			return chara;
		}
	}
}
