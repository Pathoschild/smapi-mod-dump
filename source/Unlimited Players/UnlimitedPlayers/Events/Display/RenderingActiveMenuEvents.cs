/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Armitxes/StardewValley_UnlimitedPlayers
**
*************************************************/

using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace UnlimitedPlayers.Events.Display
{
	public class RenderingActiveMenuEvents
	{
		public void RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
		{
			if (Game1.activeClickableMenu is CharacterCustomization)
				this.RenderingActiveMenu_CharacterCustomization(sender, e);
		}

        public void RenderingActiveMenu_CharacterCustomization(object sender, RenderingActiveMenuEventArgs e)
		{
			Type type = typeof(CharacterCustomization);
			CharacterCustomization activeMenu = Game1.activeClickableMenu as CharacterCustomization;

			TextBox nameBox = LazyHelper.GetInstanceField(type, activeMenu, "nameBox") as TextBox;
			TextBox farmNameBox = LazyHelper.GetInstanceField(type, activeMenu, "farmnameBox") as TextBox;
			TextBox favThingBox = LazyHelper.GetInstanceField(type, activeMenu, "favThingBox") as TextBox;

			if (!nameBox.Selected && String.IsNullOrWhiteSpace(nameBox.Text))
				nameBox.Text = "Player";
				Game1.player.Name = nameBox.Text;

			if (!farmNameBox.Selected && String.IsNullOrWhiteSpace(farmNameBox.Text))
				farmNameBox.Text = "Farm";

			if (!favThingBox.Selected && String.IsNullOrWhiteSpace(favThingBox.Text))
				favThingBox.Text = "Bananas";
		}
	}
}
