using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Menu;
using StardewValley;

namespace ModSettingsTab.Framework.Components
{
    public class ModManagerCheckBox : OptionsElement
    {
        public bool IsChecked;
        
        public ModManagerCheckBox(string modId) : base("", modId, "", null, 
            32, BaseOptionsModPage.SlotSize.Y / 2)
        {
            Name = ModData.ModList[ModId].Name;
            var version = ModData.ModList[ModId].Version;
            var length = Name.Length;
            Label = length * 2 <= 32 || length <= 25
                ? $"{Name} v.{version}"
                : $"{Name.Substring(0, 25)}... v{version}";
            IsChecked = false;
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            Game1.playSound("drumkit6");
            IsChecked = !IsChecked;
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            b.Draw(Game1.mouseCursors, new Vector2(slotX + Bounds.X, slotY + Bounds.Y),
                IsChecked ? OptionsCheckbox.SourceRectChecked : OptionsCheckbox.SourceRectUnchecked,
                Color.White * (GreyedOut ? 0.33f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
            base.Draw(b, slotX, slotY);
        }
    }
}