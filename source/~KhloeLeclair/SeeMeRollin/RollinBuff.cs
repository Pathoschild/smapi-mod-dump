/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using StardewValley;

namespace Leclair.Stardew.SeeMeRollin {
	public class RollinBuff : Buff {

		public int Speed;

		public RollinBuff(int speed) : base(I18n.Buff_Name(), 1000, null, 9) {
			which = ModEntry.BUFF;

			Speed = speed;
			buffAttributes[Buff.speed] = Speed;
		}

	}
}
