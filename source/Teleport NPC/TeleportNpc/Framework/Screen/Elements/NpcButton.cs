/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportNpc
**
*************************************************/

using EnaiumToolKit.Framework.Extensions;
using EnaiumToolKit.Framework.Screen.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace TeleportNpc.Framework.Screen.Elements;

public class NpcButton : BaseButton
{
    private readonly ClickableTextureComponent? _component;

    public NpcButton(string title, string description, NPC npc) : base(title, description)
    {
        if (npc is not (Pet or Horse or TrashBear or Raccoon))
        {
            _component = new ClickableTextureComponent("Mugshot", default, "", "", npc.Sprite.Texture,
                npc.getMugShotSourceRect(), 0.7f * Game1.pixelZoom);
        }
        else
        {
            _component = npc switch
            {
                Pet pet => new ClickableTextureComponent("Mugshot", default, "", "",
                    npc.Sprite.Texture,
                    new Rectangle(0, pet.Sprite.SourceRect.Height * 2 - 26, pet.Sprite.SourceRect.Width, 24),
                    0.7f * Game1.pixelZoom),
                Horse horse => new ClickableTextureComponent("Mugshot", default, "", "",
                    npc.Sprite.Texture,
                    new Rectangle(0, horse.Sprite.SourceRect.Height * 2 - 26, horse.Sprite.SourceRect.Width, 24),
                    0.7f * Game1.pixelZoom),
                TrashBear trashBear => new ClickableTextureComponent("Mugshot", default, "",
                    "",
                    npc.Sprite.Texture,
                    new Rectangle(0, trashBear.Sprite.SourceRect.Height, trashBear.Sprite.SourceRect.Width, 24),
                    0.7f * Game1.pixelZoom),
                Raccoon raccoon => new ClickableTextureComponent("Mugshot", default, "", "",
                    npc.Sprite.Texture,
                    new Rectangle(0, raccoon.Sprite.SourceRect.Height * 2 - 26, raccoon.Sprite.SourceRect.Width, 24),
                    0.7f * Game1.pixelZoom),
                _ => null
            };
        }
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        b.DrawButtonTexture(x, y, Width, Height, Hovered ? Color.Wheat : Color.White);
        b.DrawStringCenter(Title!, x, y, Width, Height);
        if (_component != null)
        {
            _component.bounds = new Rectangle(x, y, Height, Height);
            _component.draw(b);
        }

        base.Render(b, x, y);
    }
}