/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace GingerIslandStart.Events;

public static class Render
{
    public static void DrawOverWorld(object sender, RenderedWorldEventArgs e)
    {
        if (Game1.player is null || Game1.player.currentLocation is null || Game1.player.currentLocation.Name != "IslandSouth")
            return;
        
        var boatTex = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\WillysBoat");
        
        //box
        var boxPosition = Game1.GlobalToLocal(new Vector2(20, 44) * 64);
        e.SpriteBatch.Draw(boatTex, boxPosition,new Rectangle(50, 352, 11, 11),Game1.ambientLight,0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
        
        //broken glass
        var glassPosition = Game1.GlobalToLocal(new Vector2(17, 45) * 64 - new Vector2(0, 12));
        e.SpriteBatch.Draw(boatTex,glassPosition,new Rectangle(110, 311, 12, 12),Game1.ambientLight,0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
        
        //anchor
        var anchorPosition = Game1.GlobalToLocal(new Vector2(18, 47) * 64);
        e.SpriteBatch.Draw(boatTex,anchorPosition,new Rectangle(80, 351, 10, 17),Game1.ambientLight,0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
    }
}