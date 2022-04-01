/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Almanac.Pages {
	public interface IAlmanacPage {

		// Id
		string Id { get; }

		// Type
		PageType Type { get; }
		bool IsMagic { get; }

		// State
		object GetState();
		void LoadState(object state);

		// Events
		void Refresh();

		void ThemeChanged();
		void Activate();
		void Deactivate();
		void DateChanged(WorldDate oldDate, WorldDate newDate);

		void UpdateComponents();
		List<ClickableComponent> GetComponents();

		bool ReceiveGamePadButton(Buttons b);
		bool ReceiveKeyPress(Keys key);
		bool ReceiveScroll(int x, int y, int direction);
		bool ReceiveLeftClick(int x, int y, bool playSound);
		bool ReceiveRightClick(int x, int y, bool playSound);
		void PerformHover(int x, int y);

		void Draw(SpriteBatch b);
	}

	public enum PageType {
		Blank,
		Seasonal,
		Calendar,
		Cover
	}
}
