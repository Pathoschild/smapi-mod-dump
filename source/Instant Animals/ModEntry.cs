/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/joisse1101/InstantAnimals
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace InstantAnimals
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>        
        private ModConfig config;
		public static bool adults;
		public static InstantPurchaseAnimalsMenu purchaseMenu;
		public static bool seen;
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			helper.Events.Display.MenuChanged += this.MenuChange;
			adults = config.BuyAdultLivestock;
		}

		/*********
        ** Private methods
        *********/
		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
                return;

            // Open animal buy menu
            if (Game1.activeClickableMenu == null && e.Button == config.ToggleInstantBuyMenuButton && Game1.currentLocation is Farm)
            {
                this.Monitor.Log($"Animal buy menu toggled", LogLevel.Debug);
				seen = Game1.player.eventsSeen.Contains(3900074);
				if (!seen)
                {
					Game1.player.eventsSeen.Add(3900074);
				}
				purchaseMenu = new InstantPurchaseAnimalsMenu(getPurchaseAnimalStock(), adults);
				Game1.activeClickableMenu = (IClickableMenu)purchaseMenu;
			}
			else if (Game1.activeClickableMenu is InstantPurchaseAnimalsMenu && e.Button == config.ToggleInstantBuyMenuButton)
            {
				adults = !adults;
				purchaseMenu.setAdult(adults);
				purchaseMenu.reloadStock(getPurchaseAnimalStock());
			}
        }

		private void MenuChange(object sender, MenuChangedEventArgs e)
        {
			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;
			if (e.OldMenu is InstantPurchaseAnimalsMenu && !(e.NewMenu is InstantPurchaseAnimalsMenu))
            {
				if (!seen)
				{
					Game1.player.eventsSeen.Remove(3900074);
				}
			}
		}

		public (List<StardewValley.Object>, IDictionary<string, string>, IDictionary<string, Texture2D>, IDictionary<string, string>) getPurchaseAnimalStock()
		{
			List<StardewValley.Object> list = new List<StardewValley.Object>();
			IDictionary<string, string> strings = this.Helper.Content.Load<IDictionary<string, string>>("Strings/StringsFromCSFiles", ContentSource.GameContent);
			StardewValley.Object o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 400 : 0)
			{
				Name = "White Chicken",
				Type = null,
				displayName = strings["Utility.cs.5922"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 400 : 0)
			{
				Name = "Brown Chicken",
				Type = null,
				displayName = strings["Utility.cs.5922"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 400 : 0)
			{
				Name = "Blue Chicken",
				Type = null,
				displayName = strings["Utility.cs.5922"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 400 : 0)
			{
				Name = "Void Chicken",
				Type = null,
				displayName = strings["Utility.cs.5922"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 400 : 0)
			{
				Name = "Golden Chicken",
				Type = null,
				displayName = strings["Utility.cs.5922"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 750 : 0)
			{
				Name = "White Cow",
				Type = null,
				displayName = strings["Utility.cs.5927"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 750 : 0)
			{
				Name = "Brown Cow",
				Type = null,
				displayName = strings["Utility.cs.5927"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 2000 : 0)
			{
				Name = "Goat",
				Type = null,
				displayName = strings["Utility.cs.5933"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 600 : 0)
			{
				Name = "Duck",
				Type = null,
				displayName = strings["Utility.cs.5937"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 4000 : 0)
			{
				Name = "Sheep",
				Type = null,
				displayName = strings["Utility.cs.5942"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 4000 : 0)
			{
				Name = "Rabbit",
				Type = null,
				displayName = strings["Utility.cs.5945"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 8000 : 0)
			{
				Name = "Pig",
				Type = null,
				displayName = strings["Utility.cs.5948"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 500 : 0)
			{
				Name = "Ostrich",
				Type = null,
				displayName = strings["Ostrich_Name"]
			};
			list.Add(o);
			o = new StardewValley.Object(100, 1, isRecipe: false, config.BuyUsesResources ? 500 : 0)
			{
				Name = "Dinosaur",
				Type = null,
				displayName = "Dinosaur"
			};
			list.Add(o);

			IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
			foreach (StardewValley.Object obj in list)
            {
				string textureName = obj.Name;
				if (!obj.Name.Equals("Dinosaur") && !adults)
				{
					textureName = "baby" + obj.Name;
				}
				textures.Add(obj.Name, Helper.Content.Load<Texture2D>("Animals/" + (textureName.Equals("babyDuck") ? "babyWhite Chicken" : textureName), ContentSource.GameContent));
			}

			IDictionary<string, string> data = this.Helper.Content.Load<IDictionary<string, string>>("Data/FarmAnimals", ContentSource.GameContent);

			return (list, strings, textures, data);
		}
	}
}