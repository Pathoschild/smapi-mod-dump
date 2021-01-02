/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportNpc
**
*************************************************/

using EnaiumToolKit.Framework.Screen.Elements;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace TeleportNPC.Framework.Screen.Elements
{
    public class NpcButton : Element
    {
        private NPC Npc;

        public NpcButton(string title, string description, NPC npc) : base(title, description)
        {
            Npc = npc;
        }

        public override void Render(SpriteBatch b, int x, int y)
        {
            Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);

            Render2DUtils.DrawButton(b, x, y, Width, Height, Hovered ? Color.Wheat : Color.White);
            FontUtils.DrawHvCentered(b, Title, x + Width / 2, y + Height / 2);
            if (!(Npc is Pet || Npc is Horse))
            {
                new ClickableTextureComponent("Mugshot", new Rectangle(x, y, Height, Height), "", "", Npc.Sprite.Texture,
                    Npc.getMugShotSourceRect(), 0.7f * Game1.pixelZoom).draw(b);
            }
        }
    }
}