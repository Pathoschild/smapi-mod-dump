/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes {
    public class ModRecipe : SimpleRecipe {
        private readonly ICoreTranslationHelper _translationHelper;
        private readonly string _name;

        public override IEnumerable<IRecipePart> Ingredients { get; }
        public override IEnumerable<IRecipePart> Results { get; }
        public override ISprite Sprite { get; }

        public ModRecipe(ICoreTranslationHelper translationHelper, ISprite sprite, IRecipePart result, params IRecipePart[] ingredients) : this(translationHelper, sprite, result.Yield(), ingredients?.AsEnumerable()) { }
        public ModRecipe(ICoreTranslationHelper translationHelper, ISprite sprite, IRecipePart result, IEnumerable<IRecipePart> ingredients, string name = null, bool isCooking = false) : this(translationHelper, sprite, result.Yield(), ingredients?.AsEnumerable(), name, isCooking) { }
        public ModRecipe(ICoreTranslationHelper translationHelper, ISprite sprite, IEnumerable<IRecipePart> results, params IRecipePart[] ingredients) : this(translationHelper, sprite, results, ingredients?.AsEnumerable()) { }
        public ModRecipe(ICoreTranslationHelper translationHelper, ISprite sprite, IEnumerable<IRecipePart> results, IEnumerable<IRecipePart> ingredients, string name = null, bool isCooking = false) : base(isCooking) {
            this.Sprite = sprite;
            this.Results = results;
            this.Ingredients = ingredients;
            this._translationHelper = translationHelper;
            this._name = name;
        }

        public override string GetDisplayName() {
            return this._name == null ? base.GetDisplayName() : this._translationHelper.Get($"recipe.{this._name}").WithDefault(this._name).ToString();
        }

        public override string GetDescription() {
            return this._name == null ? base.GetDisplayName() : this._translationHelper.Get($"recipe.{this._name}.description").WithDefault(base.GetDescription()).ToString();
        }
    }
}