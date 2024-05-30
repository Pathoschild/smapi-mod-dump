/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Quests;

namespace HelpWanted.Framework.Data;

internal class QuestData
{
    public Texture2D Pad { get; }
    public Rectangle PadSource { get; }
    public Color PadColor { get; }
    public Texture2D Pin { get; }
    public Rectangle PinSource { get; }
    public Color PinColor { get; }
    public Texture2D Icon { get; }
    public Rectangle IconSource { get; }
    public Color IconColor { get; }
    public float IconScale { get; }
    public Point IconOffset { get; }
    public Quest Quest { get; }

    public QuestData(Texture2D pad, Rectangle padSource, Color padColor, Texture2D pin, Rectangle pinSource, Color pinColor,
        Texture2D icon, Rectangle iconSource, Color iconColor, float iconScale, Point iconOffset, Quest quest)
    {
        Pad = pad;
        PadSource = padSource;
        PadColor = padColor;
        Pin = pin;
        PinSource = pinSource;
        PinColor = pinColor;
        Icon = icon;
        IconSource = iconSource;
        IconColor = iconColor;
        IconScale = iconScale;
        IconOffset = iconOffset;
        Quest = quest;
    }
}