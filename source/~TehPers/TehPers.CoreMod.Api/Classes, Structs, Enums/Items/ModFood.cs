using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public class ModFood : ModObject {
        protected virtual BuffDescription Buffs { get; }
        protected bool IsDrink { get; }

        public ModFood(ICoreTranslationHelper translationHelper, ISprite sprite, string rawName, int cost, int edibility, Category category, bool isDrink) : this(translationHelper, sprite, rawName, cost, edibility, category, isDrink, new BuffDescription(0)) { }
        public ModFood(ICoreTranslationHelper translationHelper, ISprite sprite, string rawName, int cost, int edibility, Category category, bool isDrink, in BuffDescription buffs) : base(translationHelper, sprite, rawName, cost, category, edibility) {
            this.IsDrink = isDrink;
            this.Buffs = buffs;
        }

        public override string GetRawObjectInformation() {
            return $"{base.GetRawObjectInformation()}/{(this.IsDrink ? "drink" : "food")}/{this.Buffs.GetRawBuffInformation()}/{this.Buffs.Duration}";
        }
    }
}