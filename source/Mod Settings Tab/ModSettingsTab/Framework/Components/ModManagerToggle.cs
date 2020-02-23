using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Menu;
using StardewValley;

namespace ModSettingsTab.Framework.Components
{
    public class ModManagerToggle : OptionsElement
    {
        private static readonly Rectangle SourceRectUnchecked = new Rectangle(0, 288, 32, 17);
        private static readonly Rectangle SourceRectChecked = new Rectangle(32, 288, 32, 17);

        public bool Disable
        {
            get => _disable;
            set
            {
                _disable = value;
                ModData.ModList[ModId].Disabled = _disable;
                ModData.NeedReload = true;
            }
        }

        public readonly List<string> ModPack;
        private bool _disable;

        public ModManagerToggle(string uniqueId,List<string> modPack)
            : base("", uniqueId, "", null, 
                32, 
                BaseOptionsModPage.SlotSize.Y / 2, 
                64, 
                34)
        {
            ModPack = modPack ?? new List<string>();
            Name = ModData.ModList[ModId].Name;
            var version = ModData.ModList[ModId].Version;
            var length = Name.Length;
            Label = length * 2 <= 64 || length <= 57
                ? $"{Name} v.{version}"
                : $"{Name.Substring(0, 57)}... v{version}";
            InfoIconBounds = new Rectangle(0, -8, 0, 0);
            _disable = ModData.ModList[ModId].Disabled;
        }


        public override void ReceiveLeftClick(int x, int y)
        {
            Game1.playSound("drumkit6");
            base.ReceiveLeftClick(x, y);
            Disable = !Disable;
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            b.Draw(ModData.Texture, new Vector2(slotX + Bounds.X, slotY + Bounds.Y),
                Disable ? SourceRectChecked : SourceRectUnchecked, Color.White * (GreyedOut ? 0.33f : 1f), 0.0f,
                Vector2.Zero, 2f, SpriteEffects.None, 0.4f);
            base.Draw(b, slotX, slotY);
        }
    }
}