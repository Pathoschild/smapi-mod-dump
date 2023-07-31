/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace AchtuurCore.Framework;
public class DecayingText
{
    public string Text { get; set; }
    private int tickTimer;
    public int TickLifeSpan { get; set; }

    public bool LifeSpanOver { get; set; }

    public Color TextColor { get; set; }

    public DecayingText(string text, int tickLifeSpan, Color? color = null)
    {
        this.Text = text;
        this.TickLifeSpan = tickLifeSpan;
        this.tickTimer = 0;
        TextColor = color ?? Color.White;

        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        this.Tick();

        // destroy object when it should die
        if (this.tickTimer > this.TickLifeSpan)
        {
            ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
            LifeSpanOver = true;
        }
    }

    private void Tick()
    {
        this.tickTimer++;
        int a = Math.Max(0, AlphaDecayFunction(tickTimer, TickLifeSpan));
        TextColor = new Color(TextColor, a);
    }

    public void Destroy()
    {
        this.tickTimer = TickLifeSpan;
        TextColor = Color.Transparent;
    }

    public void DrawToScreen(SpriteBatch spriteBatch, Vector2 position, Color? color = null)
    {
        // Multipliy entire color by alpha to reduce opacity
        Color clr = (color ?? this.TextColor) * ((float)TextColor.A / 255f);
        spriteBatch.DrawString(Game1.dialogueFont, this.Text, position, clr);
    }

    /// <summary>
    /// Mathematical function that defines behaviour of decay duration. Currently an exponential function.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="maxT"></param>
    /// <returns></returns>
    private int AlphaDecayFunction(float t, float maxT)
    {
        // Offset, decay only starts after t = o
        float o = (float)maxT / 2f;
        // Some constant to make decay a bit faster
        float c = 10;
        // Exponent
        float exp = -(c * (t - o)) / maxT;
        // e^exp
        return (int)(255 * Math.Pow(Math.E, exp));
    }
}
