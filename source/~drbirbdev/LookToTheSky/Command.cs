/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;

namespace LookToTheSky;

[SCommand("looktothesky")]
class Command
{

    [SCommand.Command("Add a bird to the sky")]
    public static void AddBird(int yPos = 100, bool moveRight = true)
    {
        ModEntry.Instance.SkyObjects.Add(new Bird(yPos, moveRight));
    }
}
