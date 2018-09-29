using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace StackToNearbyChests.Patches
{
	static class TypeGetter
	{
		public static Type InventoryPage() => new StardewValley.Menus.InventoryPage(0, 0, 0, 0).GetType();
		public static Type IClickableMenu() => InventoryPage().BaseType;
		public static Type ClickableTextureComponent() => new StardewValley.Menus.ClickableTextureComponent(
			new Microsoft.Xna.Framework.Rectangle(),
			null,
			new Microsoft.Xna.Framework.Rectangle(),
			0f).GetType();
		public static Type SpriteBatch() => new Microsoft.Xna.Framework.Graphics.SpriteBatch(StardewValley.Game1.graphics.GraphicsDevice).GetType();
	}
	
	class InventoryPage_Patcher_Constructor : Patch
	{
		public override Type GetTargetType() => TypeGetter.InventoryPage();
		public override string GetTargetMethodName() => null;
		public override Type[] GetTargetMethodArguments() => new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };

		public static void Postfix(InventoryPage __instance, int x, int y, int width, int height)
		{
			ButtonHolder.Constructor(__instance, x, y, width, height);
		}
	}
	
	class InventoryPage_Patcher_receiveLeftClick : Patch
	{
		public override Type GetTargetType() => TypeGetter.InventoryPage();
		public override string GetTargetMethodName() => "receiveLeftClick";
		public override Type[] GetTargetMethodArguments() => new Type[] { typeof(int), typeof(int), typeof(bool) };

		public static void Postfix(int x, int y)
		{
			ButtonHolder.ReceiveLeftClick(x, y);
		}
	}
	
	class InventoryPage_Patcher_draw : Patch
	{
		public override Type GetTargetType() => TypeGetter.InventoryPage();
		public override string GetTargetMethodName() => "draw";
		public override Type[] GetTargetMethodArguments() => new Type[] { TypeGetter.SpriteBatch() };

		public static void Postfix(SpriteBatch b)
		{
			ButtonHolder.PostDraw(b);
		}
	}
	
	class InventoryPage_Patcher_performHoverAction : Patch
	{
		public override Type GetTargetType() => TypeGetter.InventoryPage();
		public override string GetTargetMethodName() => "performHoverAction";
		public override Type[] GetTargetMethodArguments() => new Type[] { typeof(int), typeof(int) };

		public static void Postfix(int x, int y)
		{
			ButtonHolder.PerformHoverAction(x, y);
		}
	}
	
	class IClickableMenu_Patcher_populateClickableComponentList : Patch
	{
		public override Type GetTargetType() => TypeGetter.IClickableMenu();
		public override string GetTargetMethodName() => "populateClickableComponentList";
		public override Type[] GetTargetMethodArguments() => new Type[] { };

		public static void Postfix(IClickableMenu __instance)
		{
			if (__instance is InventoryPage inventoryPage)
				ButtonHolder.PopulateClickableComponentsList(inventoryPage);
		}
	}
	
	class ClickableTextureComponent_Patcher_Draw : Patch
	{
		public override Type GetTargetType() => TypeGetter.ClickableTextureComponent();
		public override string GetTargetMethodName() => "draw";
		public override Type[] GetTargetMethodArguments() => new Type[] { TypeGetter.SpriteBatch() };

		public static void Postfix(ClickableTextureComponent __instance, SpriteBatch b)
		{
			ButtonHolder.TrashCanDrawn(__instance, b);
		}
	}
}
