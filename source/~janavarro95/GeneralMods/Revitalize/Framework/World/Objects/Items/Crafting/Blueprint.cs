/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.StardustCore.Animations;
using StardewValley;
using StardewValley.Network;

namespace Omegasis.Revitalize.Framework.World.Objects.Crafting
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Crafting.Blueprint")]
    public class Blueprint : CustomItem
    {
        /// <summary>
        /// A mapping from the name of the crafting book to the name of the crafting recipe to unlock.
        /// </summary>
        public readonly NetStringDictionary<string, NetString> craftingRecipesToUnlock = new();
        public NetRef<Drawable> itemToDraw = new NetRef<Drawable>();


        public Blueprint()
        {

        }

        public Blueprint(BasicItemInformation Info, string CraftingRecipeBookName, string CraftingRecipe, Drawable itemToDraw =null ) : base(Info)
        {
            this.addCraftingRecipe(CraftingRecipeBookName, CraftingRecipe);
            this.itemToDraw.Value = itemToDraw;
        }

        public Blueprint(BasicItemInformation Info, Dictionary<string,string> CraftingRecipesToUnlock, Drawable itemToDraw = null) : base(Info)
        {
            this.addCraftingRecipe(CraftingRecipesToUnlock);
            this.itemToDraw.Value = itemToDraw;
        }

        public Blueprint(BasicItemInformation Info, NetStringDictionary<string, NetString> CraftingRecipesToUnlock, Drawable itemToDraw = null) : base(Info)
        {
            foreach (var craftingBookNameToCraftingRecipeName in CraftingRecipesToUnlock)
            {
                this.addCraftingRecipe(craftingBookNameToCraftingRecipeName);
            }
            this.itemToDraw.Value = itemToDraw;
        }

        public Blueprint(JsonCraftingBlueprint jsonBlueprint) : this(jsonBlueprint.toBasicItemInformation(),jsonBlueprint.recipesToUnlock,new Drawable(jsonBlueprint.itemToDraw.getItem()))
        {

        }

        protected virtual void addCraftingRecipe(Dictionary<string,string> CraftingRecipes)
        {
            foreach (KeyValuePair<string, string> craftingBookNameToCraftingRecipeName in CraftingRecipes)
            {
                this.addCraftingRecipe(craftingBookNameToCraftingRecipeName.Key, craftingBookNameToCraftingRecipeName.Value);
            }
        }

        /// <summary>
        /// Adds a single crafting recipe to this blueprint when used.
        /// </summary>
        /// <param name="CraftingBookName"></param>
        /// <param name="CraftingRecipeName"></param>
        protected virtual void addCraftingRecipe(string CraftingBookName, string CraftingRecipeName)
        {
            this.craftingRecipesToUnlock.Add(CraftingBookName, CraftingRecipeName);
        }

        protected override void initNetFieldsPostConstructor()
        {
            base.initNetFieldsPostConstructor();
            this.NetFields.AddFields(this.craftingRecipesToUnlock,this.itemToDraw);
        }

        public override bool performUseAction(GameLocation location)
        {
            return this.learnRecipes();
        }


        protected virtual bool learnRecipes()
        {
            bool anyUnlocked = false;
            Dictionary<KeyValuePair<string, string>, bool> recipesLearned = RevitalizeModCore.ModContentManager.craftingManager.learnCraftingRecipes(this.craftingRecipesToUnlock);

            foreach(var bookRecipePairToLearnedValues in recipesLearned) {

                string itemToCraftOutputName = RevitalizeModCore.ModContentManager.craftingManager.getCraftingRecipeBook(bookRecipePairToLearnedValues.Key.Key).getCraftingRecipe(bookRecipePairToLearnedValues.Key.Value).recipe.outputName;
                string craftingStationName = Constants.ItemIds.Objects.CraftingStations.GetCraftingStationNameFromRecipeBookId(bookRecipePairToLearnedValues.Key.Key);

                bool isPlural = itemToCraftOutputName.ToLowerInvariant().StartsWith("a") || itemToCraftOutputName.ToLowerInvariant().StartsWith("e") || itemToCraftOutputName.ToLowerInvariant().StartsWith("i") || itemToCraftOutputName.ToLowerInvariant().StartsWith("o") || itemToCraftOutputName.ToLowerInvariant().StartsWith("u");

                if (bookRecipePairToLearnedValues.Value == true)
                {
                    anyUnlocked = true;

                    Game1.drawObjectDialogue(string.Format("You learned how to make {2} {0}! You can make it on {2} {1}. ",itemToCraftOutputName, craftingStationName, isPlural? "an":"a"));

                }
                else
                {
                    Game1.drawObjectDialogue(string.Format("You already know how to make {2} {0} on {2} {1}. ", itemToCraftOutputName, craftingStationName, isPlural ? "an" : "a"));
                }
            }

            return anyUnlocked;
        }



        public override Item getOne()
        {
            Blueprint component = new Blueprint(this.basicItemInformation.Copy(), this.craftingRecipesToUnlock, this.itemToDraw.Value!=null? this.itemToDraw.Value.Copy(): new Drawable());
            return component;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (this.itemToDraw.Value != null)
            {
                this.itemToDraw.Value.drawInMenu(spriteBatch, location, scaleSize, .5f, layerDepth, drawStackNumber, color, drawShadow);
                base.drawInMenu(spriteBatch, location + new Vector2(16, 8), scaleSize * .5f, 1f, layerDepth, drawStackNumber, color, drawShadow);
                return;
            }
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);

        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        protected virtual bool blueprintOutputIsCustomObject()
        {
            if(this.itemToDraw.Value!=null && this.itemToDraw.Value is ICustomModObject)
            {
                return true;
            }
            return false;
        }

        public override string getDescription()
        {
            if (RevitalizeModCore.ModContentManager.craftingManager.knowsCraftingRecipes(this.craftingRecipesToUnlock))
            {
                return "(Learned) \n" + this.basicItemInformation.description.Value;
            }
            return base.getDescription();
        }
    }
}
