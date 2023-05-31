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
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.HUD;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.StardustCore.Animations;
using StardewValley;
using StardewValley.Network;

namespace Omegasis.Revitalize.Framework.World.Objects.Crafting
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Crafting.Blueprint")]
    public class Blueprint : CustomItem
    {
        /// <summary>
        /// A mapping from the name of the crafting book to the name of the crafting recipe to unlock.
        /// </summary>
        public readonly NetObjectList<CraftingBookIdToRecipeId> craftingRecipesToUnlock = new();
        public NetRef<Drawable> itemToDraw = new NetRef<Drawable>();


        public Blueprint()
        {

        }

        public Blueprint(BasicItemInformation Info, CraftingBookIdToRecipeId recipeToUnlock, Drawable itemToDraw =null ) : base(Info)
        {
            this.addCraftingRecipe(recipeToUnlock);
            this.itemToDraw.Value = itemToDraw;
        }

        public Blueprint(BasicItemInformation Info, List<CraftingBookIdToRecipeId> recipesToUnlock, Drawable itemToDraw = null) : base(Info)
        {
            this.addCraftingRecipe(recipesToUnlock.ToArray());
            this.itemToDraw.Value = itemToDraw;
        }

        public Blueprint(BasicItemInformation Info, NetObjectList<CraftingBookIdToRecipeId> CraftingRecipesToUnlock, Drawable itemToDraw = null) : base(Info)
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

        protected virtual void addCraftingRecipe(params CraftingBookIdToRecipeId[] recipesToUnlock)
        {
            foreach (CraftingBookIdToRecipeId craftingBookNameToCraftingRecipeName in recipesToUnlock)
            {
                this.craftingRecipesToUnlock.Add(craftingBookNameToCraftingRecipeName);
            }
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.craftingRecipesToUnlock,this.itemToDraw);
        }

        /// <summary>
        /// Used to use the item while still in the farmer's hands.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performUseAction(GameLocation location)
        {
            return this.learnRecipes();
        }


        protected virtual bool learnRecipes()
        {
            return RevitalizeModCore.ModContentManager.craftingManager.learnCraftingRecipes(this.craftingRecipesToUnlock, true).Count() > 0;
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
                return "(Learned) \n" + base.getDescription();
            }
            return base.getDescription();
        }

        public override bool canBeTrashed()
        {
            return false;
        }

        public override bool canBeDropped()
        {
            return false;
        }
    }
}
