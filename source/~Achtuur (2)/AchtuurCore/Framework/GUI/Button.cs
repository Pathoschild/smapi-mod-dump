/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Events;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.GUI;

public struct ButtonPressedArgs { }


public class Button
{
    public Vector2 Size => new(Width, Height);
    public float Width => Texture.Width * Scale;
    public float Height => Texture.Height * Scale;

    private static Texture2D ButtonTexture;
    public Texture2D Texture;
    public float Scale;

    public event EventHandler<ButtonPressedArgs> OnClick;

    public Button()
    {
        Texture = ButtonTexture;
        Scale = 1f;
    }

    public void Click()
    {
        OnClick?.Invoke(this, new ButtonPressedArgs());
    }

    public virtual void Draw(SpriteBatch sb, Vector2 position, Vector2? scale=null, Color? color=null)
    {
        sb.DrawTexture(Texture, position, scale, color);
    }
}
