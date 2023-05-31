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
using Netcode;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;

namespace Omegasis.Revitalize.Framework.Crafting
{
    /// <summary>
    /// Used in <see cref="Blueprints"/> to determine which crafting recipes should be unlocked by which blueprints.
    /// </summary>
    public class CraftingBookIdToRecipeId : StardustCore.Networking.NetObject
    {

        [JsonIgnore]
        public readonly NetString craftingBookId = new NetString();

        [JsonIgnore]
        public readonly NetString recipeId=new NetString();

        public string CraftingBookId
        {
            get
            {
                return this.craftingBookId.Value;
            }
            set
            {
                this.craftingBookId.Value = value;
            }

        }

        public string RecipeId
        {
            get
            {
                return this.recipeId.Value;
            }
            set
            {
                this.recipeId.Value = value;
            }
        }

        public CraftingBookIdToRecipeId()
        {
            this.initializeNetFields();
        }

        public CraftingBookIdToRecipeId(string craftingStationId, string recipeId)
        {
            this.CraftingBookId = craftingStationId;
            this.RecipeId = recipeId;

            this.initializeNetFields();
        }

        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.craftingBookId, this.recipeId);
        }

        public CraftingBookIdToRecipeId Copy()
        {
            return new CraftingBookIdToRecipeId(this.CraftingBookId, this.RecipeId);
        }
    }
}
