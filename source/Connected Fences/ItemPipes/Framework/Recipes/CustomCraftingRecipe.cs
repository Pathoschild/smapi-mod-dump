/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using SObject = StardewValley.Object;


namespace ItemPipes.Framework.Recipes
{
    public class CustomCraftingRecipe : CraftingRecipe
    {
        public string IDName { get; set; }
        public Texture2D ItemTexture { get; set; }
        public List<string> Ingredients { get; set; }
        public string Product { get; set; }
        public CustomCraftingRecipe(string name) : base(name)
        {

        }

        public CustomCraftingRecipe(string name, bool isCookingRecipe) : base(name, isCookingRecipe)
        {
			DataAccess DataAccess = DataAccess.GetDataAccess();
			IDName = Utilities.GetIDName(name);
			if(IDName.Equals("pipo"))
            {
				this.bigCraftable = true;
            }
			this.DisplayName = DataAccess.ItemNames[IDName];
			ItemTexture = DataAccess.Sprites[IDName+"_item"];
			description = DataAccess.ItemDescriptions[IDName];
			this.isCookingRecipe = isCookingRecipe;
			this.name = name;
			string info = DataAccess.Recipes[IDName];
			string[] infoSplit = info.Split('/');
			string[] ingredientsSplit = infoSplit[0].Split(' ');
			this.recipeList.Clear();
			this.itemToProduce.Clear();
			for (int j = 0; j < ingredientsSplit.Length; j += 2)
			{
				this.recipeList.Add(Convert.ToInt32(ingredientsSplit[j]), Convert.ToInt32(ingredientsSplit[j + 1]));
				
			}
			string[] itemToProduceList = infoSplit[2].Split(' ');
			for (int i = 0; i < itemToProduceList.Length; i += 2)
			{
				this.itemToProduce.Add(Convert.ToInt32(itemToProduceList[i]));
				this.numberProducedPerCraft = ((itemToProduceList.Length <= 1) ? 1 : Convert.ToInt32(itemToProduceList[i + 1]));
			}
			if (!isCookingRecipe)
			{
				if (infoSplit[3] == "true")
				{
					this.itemType = "BO";
					this.bigCraftable = true;
				}
				else if (infoSplit[3] == "false")
				{
					this.itemType = "O";
				}
				else
				{
					this.itemType = infoSplit[3];
				}
			}
			this.timesCrafted = (Game1.player.craftingRecipes.ContainsKey(name) ? Game1.player.craftingRecipes[name] : 0);
			/*
			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				this.DisplayName = infoSplit[infoSplit.Length - 1];
			}
			else
			{
				this.DisplayName = name;
			}
			*/
		}


        public override void drawMenuView(SpriteBatch b, int x, int y, float layerDepth = 0.88f, bool shadow = true)
        {
			if (this.bigCraftable)
			{
				Rectangle srcRect = new Rectangle(0, 0, 16, 32);
				Utility.drawWithShadow(b, ItemTexture, new Vector2(x, y), srcRect, Color.White, 0f, Vector2.Zero, 4f, flipped: false, layerDepth);
			}
			else
			{
				Rectangle srcRect = new Rectangle(0, 0, 16, 16);
				Utility.drawWithShadow(b, ItemTexture, new Vector2(x, y), srcRect, Color.White, 0f, Vector2.Zero, 4f, flipped: false, layerDepth);
			}
		}
		
		public override Item createItem()
		{
			SObject createdItem = ItemFactory.CreateItem(IDName);
			createdItem.stack.Value = this.numberProducedPerCraft;
			return createdItem;
		}

		public override void drawRecipeDescription(SpriteBatch b, Vector2 position, int width, IList<Item> additional_crafting_items)
		{
			DataAccess dataAccess = DataAccess.GetDataAccess();
			int lineExpansion = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? 8 : 0);
			b.Draw(Game1.staminaRect, new Rectangle((int)(position.X + 8f), (int)(position.Y + 32f + Game1.smallFont.MeasureString("Ing!").Y) - 4 - 2 - (int)((float)lineExpansion * 1.5f), width - 32, 2), Game1.textColor * 0.35f);
			Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"), Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f);
			for (int i = 0; i < this.recipeList.Count; i++)
			{
				int required_count = this.recipeList.Values.ElementAt(i);
				int required_item = this.recipeList.Keys.ElementAt(i);
				int bag_count = Game1.player.getItemCount(required_item, 8);
				int containers_count = 0;
				required_count -= bag_count;
				if (additional_crafting_items != null)
				{
					containers_count = Game1.player.getItemCountInList(additional_crafting_items, required_item, 8);
					if (required_count > 0)
					{
						required_count -= containers_count;
					}
				}
				string ingredient_name_text = this.getNameFromIndex(this.recipeList.Keys.ElementAt(i));
				Color drawColor = ((required_count <= 0) ? Game1.textColor : Color.Red);
				Vector2 text_draw_position = new Vector2(position.X + 32f + 8f, position.Y + 64f + (float)(i * 64 / 2) + (float)(i * 4) + 4f);
				if (dataAccess.ItemIDNames.Contains(Utilities.GetIDName(ingredient_name_text)))
                {
					Rectangle srcRect = new Rectangle(0, 0, 16, 16);
					Texture2D IngredientTexture = dataAccess.Sprites[ingredient_name_text + "_item"];
					b.Draw(IngredientTexture, new Vector2(position.X, position.Y + 64f + (float)(i * 64 / 2) + (float)(i * 4)), srcRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
					Utility.drawTinyDigits(this.recipeList.Values.ElementAt(i), b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(string.Concat(this.recipeList.Values.ElementAt(i))).X, position.Y + 64f + (float)(i * 64 / 2) + (float)(i * 4) + 21f), 2f, 0.87f, Color.AntiqueWhite);
					string customName = dataAccess.ItemNames[ingredient_name_text];
					Utility.drawTextWithShadow(b, customName, Game1.smallFont, text_draw_position, drawColor);
				}
				else
                {
					b.Draw(Game1.objectSpriteSheet, new Vector2(position.X, position.Y + 64f + (float)(i * 64 / 2) + (float)(i * 4)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getSpriteIndexFromRawIndex(this.recipeList.Keys.ElementAt(i)), 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
					Utility.drawTinyDigits(this.recipeList.Values.ElementAt(i), b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(string.Concat(this.recipeList.Values.ElementAt(i))).X, position.Y + 64f + (float)(i * 64 / 2) + (float)(i * 4) + 21f), 2f, 0.87f, Color.AntiqueWhite);
					Utility.drawTextWithShadow(b, ingredient_name_text, Game1.smallFont, text_draw_position, drawColor);
				}

				if (Game1.options.showAdvancedCraftingInformation)
				{
					text_draw_position.X = position.X + (float)width - 40f;
					b.Draw(Game1.mouseCursors, new Rectangle((int)text_draw_position.X, (int)text_draw_position.Y + 2, 22, 26), new Rectangle(268, 1436, 11, 13), Color.White);
					Utility.drawTextWithShadow(b, string.Concat(bag_count + containers_count), Game1.smallFont, text_draw_position - new Vector2(Game1.smallFont.MeasureString(bag_count + containers_count + " ").X, 0f), drawColor);
				}
			}
			b.Draw(Game1.staminaRect, new Rectangle((int)position.X + 8, (int)position.Y + lineExpansion + 64 + 4 + this.recipeList.Count * 36, width - 32, 2), Game1.textColor * 0.35f);
			Utility.drawTextWithShadow(b, Game1.parseText(this.description, Game1.smallFont, width - 8), Game1.smallFont, position + new Vector2(0f, 76 + this.recipeList.Count * 36 + lineExpansion), Game1.textColor * 0.75f);
		}

		public new string getNameFromIndex(int index)
		{
			if (index < 0)
			{
				return index switch
				{
					-1 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568"),
					-2 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569"),
					-3 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570"),
					-4 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571"),
					-5 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572"),
					-6 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573"),
					-777 => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574"),
					_ => "???",
				};
			}
			string retString = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.575");
			if (Game1.objectInformation.ContainsKey(index))
			{
				retString = Game1.objectInformation[index].Split('/')[4];
			}
			if(DataAccess.GetDataAccess().ModItemsIDs.Values.Contains(index))
            {
				retString = DataAccess.GetDataAccess().ItemIDs.FirstOrDefault(x => x.Value == index).Key;
			}
			return retString;
		}
	}
}
