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

namespace HelpWanted.Framework;

public class QuestData
{
    public Texture2D PadTexture { get; set; }
    public Rectangle PadTextureSource { get; set; }
    public Color PadColor { get; set; }
    public Texture2D PinTexture { get; set; }
    public Rectangle PinTextureSource { get; set; }
    public Color PinColor { get; set; }
    public Texture2D Icon { get; set; }
    public Rectangle IconSource { get; set; }
    public Color IconColor { get; set; }
    public float IconScale { get; set; }
    public Point IconOffset { get; set; }
    public Quest Quest { get; set; }
    
    public QuestData(Texture2D padTexture, Rectangle padTextureSource, Color padColor, Texture2D pinTexture, Rectangle pinTextureSource, Color pinColor, Texture2D icon, Rectangle iconSource, Color iconColor, float iconScale, Point iconOffset, Quest quest)
    {
        PadTexture = padTexture;
        PadTextureSource = padTextureSource;
        PadColor = padColor;
        PinTexture = pinTexture;
        PinTextureSource = pinTextureSource;
        PinColor = pinColor;
        Icon = icon;
        IconSource = iconSource;
        IconColor = iconColor;
        IconScale = iconScale;
        IconOffset = iconOffset;
        Quest = quest;
    }
}