/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public class ModCraftable : ModObject {
        public virtual bool CanSetOutdoors => true;
        public virtual bool CanSetIndoors => true;
        public virtual int Fragility { get; }

        public ModCraftable(ICoreTranslationHelper translationHelper, string rawName, int cost) : this(translationHelper, rawName, cost, 0) { }
        public ModCraftable(ICoreTranslationHelper translationHelper, string rawName, int cost, int fragility) : base(translationHelper, null, rawName, cost, Category.BigCraftable, -300) {
            this.Fragility = fragility;
        }

        /// <inheritdoc />
        public override string GetRawObjectInformation() {
            string displayName = this.TranslationHelper.Get($"item.{this.RawName}").WithDefault($"item.{this.RawName}").ToString();
            string description = this.TranslationHelper.Get($"item.{this.RawName}.description").WithDefault("No description available.").ToString();
            return $"{displayName}/{this.Cost}/{this.Edibility}/{this.Category}/{description}/{(this.CanSetOutdoors ? "true" : "false")}/{(this.CanSetIndoors ? "true" : "false")}/{this.Fragility}/{displayName}";
        }
    }
}