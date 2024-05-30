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

namespace ActiveMenuAnywhere.Framework.Options;

internal abstract class BaseOption
{
    protected BaseOption(string name, Rectangle sourceRect)
    {
        Name = name;
        SourceRect = sourceRect;
    }

    public Rectangle SourceRect { get; }
    public string Name { get; }

    public virtual void ReceiveLeftClick()
    {
    }
}