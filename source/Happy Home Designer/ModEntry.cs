/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HappyHomeDesigner.Integration;
using HappyHomeDesigner.Menus;
using HappyHomeDesigner.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace HappyHomeDesigner
{
	public class ModEntry : Mod
	{
		public const string MOD_ID = "tlitookilakin.HappyHomeDesigner";

		internal static IMonitor monitor;
		internal static IManifest manifest;
		internal static IModHelper helper;
		internal static Config config;
		internal static ITranslationHelper i18n;

		public override void Entry(IModHelper helper)
		{
			monitor = Monitor;
			ModEntry.helper = helper;
			i18n = Helper.Translation;
			config = Helper.ReadConfig<Config>();
			manifest = ModManifest;

			helper.Events.GameLoop.GameLaunched += Launched;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.Input.MouseWheelScrolled += OnMouseScroll;
			helper.Events.Player.Warped += OnWarp;

			AssetManager.Init(Helper);
			InventoryWatcher.Init(Helper);
		}

		private void OnWarp(object sender, WarpedEventArgs e)
		{
			if (Catalog.ActiveMenu.Value is Catalog catalog)
				catalog.exitThisMenuNoSound();
		}

		private void OnMouseScroll(object sender, MouseWheelScrolledEventArgs e)
		{
			if (e.Delta is not 0 && Catalog.ActiveMenu.Value is Catalog catalog)
			{
				var mouse = Game1.getMousePosition(true);
				if (catalog.isWithinBounds(mouse.X, mouse.Y))
				{
					catalog.receiveScrollWheelAction(-Math.Sign(e.Delta));
					e.Suppress();
				}
			}
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!e.IsSuppressed() && config.CloseWithKey && Game1.activeClickableMenu is null)
			{
				if (Catalog.ActiveMenu.Value is Catalog cat) {

					if (config.ToggleShortcut.JustPressed())
					{
						cat.Toggle(true);
						helper.Input.Suppress(e.Button);
						return;
					}

					var binds = Game1.options.menuButton;
					for (int i = 0; i < binds.Length; i++)
					{
						if ((int)binds[i].key == (int)e.Button)
						{
							cat.exitThisMenu();
							helper.Input.Suppress(e.Button);
						}
					}
				}
			}
		}

		private void Launched(object sender, GameLaunchedEventArgs e)
		{
			if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
			{
				IGMCM.API = Helper.ModRegistry.GetApi<IGMCM>("spacechase0.GenericModConfigMenu");
				IGMCM.Installed = true;
				config.Register(IGMCM.API, ModManifest);
			}

			Patch(new(ModManifest.UniqueID));

			AlternativeTextures.Init(Helper);
			CustomFurniture.Init(Helper);
		}

		private static void Patch(Harmony harmony)
		{
			ReplaceShop.Apply(harmony);
			ItemCloneFix.Apply(harmony);
			FurnitureAction.Apply(harmony);
			InventoryCombine.Apply(harmony);
			SearchFocusFix.Apply(harmony);
			ItemReceive.Apply(harmony);

			AltTex.Apply(harmony);
			// TODO rewrite patches when dga comes back
			//DGA.Apply(harmony);
		}
	}
}
