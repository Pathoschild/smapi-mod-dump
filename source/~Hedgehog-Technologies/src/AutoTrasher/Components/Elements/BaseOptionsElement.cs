/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley.Menus;

namespace AutoTrasher.Components.Elements
{
	internal abstract class BaseOptionsElement : OptionsElement
	{
		protected BaseOptionsElement(string label)
			: base(label)
		{ }

		protected BaseOptionsElement(string label, int x, int y, int width, int height, int whichOption = -1)
			: base(label, x, y, width, height, whichOption)
		{ }

		protected int GetOffsetX()
		{
			return 0;
		}

		public virtual void ReceiveButtonPress(SButton button) { }

		public override void receiveKeyPress(Keys key)
		{
			ReceiveButtonPress(key.ToSButton());
			base.receiveKeyPress(key);
		}
	}
}
