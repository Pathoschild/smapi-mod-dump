/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace CopyInviteCode
{
    /// <summary>A dummy item used to draw a <see cref="HUDMessage"/>.</summary>
    internal class ClipboardItem : Item
    {
        private readonly Texture2D clipboardTexture;

        public ClipboardItem(Texture2D texture)
        {
            this.clipboardTexture = texture;
        }

        public override string DisplayName
        {
            get => "Copied invite code to clipboard!";
            set { }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency,
            float layerDepth,
            StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(this.clipboardTexture, new Rectangle((int)(location.X + 8 * scaleSize), (int)(location.Y + 8 * scaleSize), 64, 64), new Rectangle(0, 0, 64, 64), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, SpriteEffects.None,
                layerDepth);
        }

        public override int Stack
        {
            get => -1;
            set { }
        }

        public override int maximumStackSize()
        {
            return -1;
        }

        public override int addToStack(Item stack)
        {
            return stack.Stack;
        }

        public override string getDescription()
        {
            return "";
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override Item getOne()
        {
            return new ClipboardItem(this.clipboardTexture);
        }
    }
}
