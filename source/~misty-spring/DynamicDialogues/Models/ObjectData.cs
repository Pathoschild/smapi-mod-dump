/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

// ReSharper disable ClassNeverInstantiated.Global
namespace DynamicDialogues.Models;

/// <summary>
/// A class used for object hunts.
/// </summary>
internal class ObjectData
{
    public string ItemId = "(O)0";
    public int X;
    public int Y;

    public ObjectData()
    {
    }

    public ObjectData(ObjectData od)
    {
        ItemId = od.ItemId;
        X = od.X;
        Y = od.Y;
    }
}