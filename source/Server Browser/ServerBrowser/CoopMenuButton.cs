using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ServerBrowser
{
	internal class CoopMenuHolder
	{
		public static ClickableComponent OnlineTab { get; set; }
		public static Checkbox PublicCheckBox { get; set; } = new Checkbox(0, 0, "Public", false);
		public static bool IsPublicCheckBoxVisible(CoopMenu menu)
		{
			try
			{
				return ModEntry.ModHelper.Reflection.GetField<CoopMenu.Tab>(menu, "currentTab").GetValue() == CoopMenu.Tab.HOST_TAB;
			}
			catch(Exception)
			{
				return true;
			}
		}

		public static void CreateTab(CoopMenu coopMenu)
		{
			var label2 = "Online";
			var smallScreenFormat = true;
			var joinTab = coopMenu.joinTab;
			var hostTab = coopMenu.hostTab;
			var width2 = (int)Game1.dialogueFont.MeasureString(label2).X + 64;
			var pos2 = (smallScreenFormat ? new Vector2((float)(hostTab.bounds.Right), (float)coopMenu.yPositionOnScreen) : new Vector2((float)(joinTab.bounds.Right + 4), (float)(coopMenu.yPositionOnScreen - 64)));

			//height : smallScreenFormat ? 72 : 128
			OnlineTab = new ClickableComponent(new Rectangle((int)pos2.X, (int)pos2.Y, width2, joinTab.bounds.Height ), "", label2);
		}
	}

	class CoopMenuDrawTabs : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "drawTabs");

		
		public static void Postfix(SpriteBatch b, CoopMenu __instance)
		{
			//Online button
			bool smallScreenFormat = false;
			Color selectColor = smallScreenFormat ? Color.Orange : new Color(255, 255, 150);
			Color hoverColor = Color.Yellow;
			Color selectShadow = smallScreenFormat ? Color.DarkOrange : new Color(221, 148, 84);
			Color hoverShadow = Color.DarkGoldenrod;
			var joinTab = __instance.joinTab;
			var onlineTab = CoopMenuHolder.OnlineTab;
			bool colorSelect = false;
			bool colorHover = onlineTab.containsPoint(Game1.getMouseX(), Game1.getMouseY());
			IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), onlineTab.bounds.X, onlineTab.bounds.Y, onlineTab.bounds.Width, onlineTab.bounds.Height, colorSelect ? selectColor : (colorHover ? hoverColor : Color.White), 1f, false);
			Utility.drawTextWithColoredShadow(b, onlineTab.label, Game1.dialogueFont, new Vector2((float)onlineTab.bounds.Center.X, (float)(onlineTab.bounds.Y + 40)) - Game1.dialogueFont.MeasureString(onlineTab.label) / 2f, Game1.textColor, colorHover ? hoverShadow : (colorSelect ? selectShadow : new Color(221, 148, 84)), 1.01f, -1f, -1, -1, 3);

			//Public server toggle button
			if (CoopMenuHolder.IsPublicCheckBoxVisible(__instance))
			{
				IClickableMenu.drawTextureBox(b, __instance.xPositionOnScreen + __instance.width - 200, __instance.yPositionOnScreen, 200, 75, Color.White);
				CoopMenuHolder.PublicCheckBox.x = __instance.xPositionOnScreen + __instance.width - 200 + 20;
				CoopMenuHolder.PublicCheckBox.y = __instance.yPositionOnScreen + 20;
				CoopMenuHolder.PublicCheckBox.Draw(b);
			}
		}
	}

	class CoopMenuTabClick : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "receiveLeftClick");
		
		public static bool Prefix(int x, int y, CoopMenu __instance)
		{
			if (CoopMenuHolder.OnlineTab.containsPoint(x,y))
			{
				Console.WriteLine("Clicked online tab");
				ModEntry.OpenServerBrowser();

				return false;
			}

			if (CoopMenuHolder.IsPublicCheckBoxVisible(__instance) && CoopMenuHolder.PublicCheckBox.BoundsPlusText().Contains(x,y))
			{
				CoopMenuHolder.PublicCheckBox.Clicked(x, y);
				return false;
			}

			return true;
		}
	}

	class CoopMenuConnectionFinished : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "connectionFinished");
		
		public static void Postfix(CoopMenu __instance)
		{
			CoopMenuHolder.CreateTab(__instance);
		}
	}

	class CoopMenuGameWindowResized : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "gameWindowSizeChanged");

		public static void Postfix(CoopMenu __instance)
		{
			CoopMenuHolder.CreateTab(__instance);
		}
	}
}
