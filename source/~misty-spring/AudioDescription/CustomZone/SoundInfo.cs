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

namespace AudioDescription.CustomZone;

public class SoundInfo
{

    public string Message;

    public string Type;

    public Color Color;

    public int TimeLeft;

    public float Transparency = 1f;

    public int Position = 0;

    public SoundInfo()
    {
    }

    public SoundInfo(SoundInfo si)
    {
        Message = si.Message;
        TimeLeft = si.TimeLeft;
        Color = si.Color;
        Transparency = si.Transparency;
        Position = si.Position;
    }

    public SoundInfo(string sound)
    {
        Message = sound;
        TimeLeft = 350; //3500f
        Color = Color.Black;
        Transparency = 1f;
        Position = ModEntry.SoundMessages.Count + 1;
    }
    public SoundInfo(string sound, Color whichColor)
    {
        Message = sound;
        TimeLeft = 350;
        Color = whichColor;
        Transparency = 1f;
        Position = ModEntry.SoundMessages.Count + 1;
    }

    public SoundInfo(string sound, Color whichColor, int time)
    {
        Message = sound;
        TimeLeft = time;
        Color = whichColor;
        Transparency = 1f;
        Position = ModEntry.SoundMessages.Count + 1;
    }

    public SoundInfo(string sound, Color whichColor, int time, float transp)
    {
        Message = sound;
        TimeLeft = time;
        Color = whichColor;
        Transparency = transp;
        Position = ModEntry.SoundMessages.Count + 1;
    }
}

/*
public static void addHUDMessage(HUDMessage message)
    {
        if (message.type != null || message.whatType != 0)
        {
            for (int j = 0; j < Game1.hudMessages.Count; j++)
            {
                if (message.type != null && Game1.hudMessages[j].type != null && Game1.hudMessages[j].type.Equals(message.type) && Game1.hudMessages[j].add == message.add)
                {
                    Game1.hudMessages[j].number = (message.add ? (Game1.hudMessages[j].number + message.number) : (Game1.hudMessages[j].number - message.number));
                    Game1.hudMessages[j].timeLeft = 3500f;
                    Game1.hudMessages[j].transparency = 1f;
                    return;
                }
                if (message.whatType == Game1.hudMessages[j].whatType && message.whatType != 1 && message.message != null && message.message.Equals(Game1.hudMessages[j].message))
                {
                    Game1.hudMessages[j].timeLeft = message.timeLeft;
                    Game1.hudMessages[j].transparency = 1f;
                    return;
                }
            }
        }
        Game1.hudMessages.Add(message);
        for (int i = Game1.hudMessages.Count - 1; i >= 0; i--)
        {
            if (Game1.hudMessages[i].noIcon)
            {
                HUDMessage tmp = Game1.hudMessages[i];
                Game1.hudMessages.RemoveAt(i);
                Game1.hudMessages.Add(tmp);
            }
        }
    }
*/

/*        else if (this.fadeIn)
        {
            this.transparency = Math.Min(this.transparency + 0.02f, 1f);
        }
        return false;
    }*/

/*public virtual void draw(SpriteBatch b, int i)
{
    Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
    if (this.noIcon)
    {
        int overrideX = tsarea.Left + 16;
        int overrideY = ((Game1.uiViewport.Width < 1400) ? (-64) : 0) + tsarea.Bottom - (i + 1) * 64 * 7 / 4 - 21 - (int)Game1.dialogueFont.MeasureString(this.message).Y;
        IClickableMenu.drawHoverText(b, this.message, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, overrideX, overrideY, this.transparency);
        return;
    }
    Vector2 itemBoxPosition = new Vector2(tsarea.Left + 16, tsarea.Bottom - (i + 1) * 64 * 7 / 4 - 64);
    if (Game1.isOutdoorMapSmallerThanViewport())
    {
        itemBoxPosition.X = Math.Max(tsarea.Left + 16, -Game1.uiViewport.X + 16);
    }
    if (Game1.uiViewport.Width < 1400)
    {
        itemBoxPosition.Y -= 48f;
    }
    b.Draw(Game1.mouseCursors, itemBoxPosition, (this.messageSubject != null && this.messageSubject is Object && (this.messageSubject as Object).sellToStorePrice(-1L) > 500) ? new Rectangle(163, 399, 26, 24) : new Rectangle(293, 360, 26, 24), Color.White * this.transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    float messageWidth = Game1.smallFont.MeasureString((this.messageSubject != null && this.messageSubject.DisplayName != null) ? this.messageSubject.DisplayName : ((this.message == null) ? "" : this.message)).X;
    b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 104f, itemBoxPosition.Y), new Rectangle(319, 360, 1, 24), Color.White * this.transparency, 0f, Vector2.Zero, new Vector2(messageWidth, 4f), SpriteEffects.None, 1f);
    b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 104f + messageWidth, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * this.transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    itemBoxPosition.X += 16f;
    itemBoxPosition.Y += 16f;
    if (this.messageSubject == null)
    {
        switch (this.whatType)
        {
            case 1:
                b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(294, 392, 16, 16), Color.White * this.transparency, 0f, new Vector2(8f, 8f), 4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
                break;
            case 2:
                b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(403, 496, 5, 14), Color.White * this.transparency, 0f, new Vector2(3f, 7f), 4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
                break;
            case 3:
                b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(268, 470, 16, 16), Color.White * this.transparency, 0f, new Vector2(8f, 8f), 4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
                break;
            case 4:
                b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(0, 411, 16, 16), Color.White * this.transparency, 0f, new Vector2(8f, 8f), 4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
                break;
            case 5:
                b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(16, 411, 16, 16), Color.White * this.transparency, 0f, new Vector2(8f, 8f), 4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
                break;
            case 6:
                b.Draw(Game1.mouseCursors2, itemBoxPosition + new Vector2(8f, 8f) * 4f, new Rectangle(96, 32, 16, 16), Color.White * this.transparency, 0f, new Vector2(8f, 8f), 4f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), SpriteEffects.None, 1f);
                break;
        }
    }
    else
    {
        this.messageSubject.drawInMenu(b, itemBoxPosition, 1f + Math.Max(0f, (this.timeLeft - 3000f) / 900f), this.transparency, 1f, StackDrawType.Hide);
    }
    itemBoxPosition.X += 51f;
    itemBoxPosition.Y += 51f;
    if (this.number > 1)
    {
        Utility.drawTinyDigits(this.number, b, itemBoxPosition, 3f, 1f, Color.White * this.transparency);
    }
    itemBoxPosition.X += 32f;
    itemBoxPosition.Y -= 33f;
    Utility.drawTextWithShadow(b, (this.messageSubject == null) ? this.message : this.messageSubject.DisplayName, Game1.smallFont, itemBoxPosition, Game1.textColor * this.transparency, 1f, 1f, -1, -1, this.transparency);
}*/