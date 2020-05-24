using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using PyTK.Types;
using PyTK.CustomElementHandler;

namespace GekosBows {
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod {

		public static Mod INSTANCE;
		public static IModHelper modhelper;
		public static ITranslationHelper i18n;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			INSTANCE = this;
			modhelper = helper;
			i18n = helper.Translation;

			modhelper.Events.Input.ButtonPressed += OnButtonPressed;

			// Load textures
			ToolBow.texture = helper.Content.Load<Texture2D>("assets/item.png");

		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

			if (!Context.IsWorldReady) {
				return;
			}

			if (e.Button == SButton.O) {
				ToolBow bow = new ToolBow();
				Game1.player.addItemToInventory(bow);

			}
		}

		private void OnGameLaunched(object sender,  GameLaunchedEventArgs e) {
			CustomObjectData.newObject("gekox.bows.toolBow", ToolBow.texture, Color.White, i18n.Get("bow_name"), i18n.Get("desc_bow"), customType: typeof(ToolBow));
		}
	}
}