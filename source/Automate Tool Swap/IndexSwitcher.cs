/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/

using StardewValley;

public class IndexSwitcher
{
    public int currentIndex;
    public int lastIndex;
    public int auxIndex;
    public bool canSwitch;
    public IndexSwitcher(int initialIndex)
    {
        currentIndex = initialIndex;
        lastIndex = initialIndex;
        auxIndex = initialIndex;
    }

    public async Task SwitchIndex(int newIndex)
    {
        lastIndex = Game1.player.CurrentToolIndex;
        Game1.player.CurrentToolIndex = newIndex;
        currentIndex = newIndex;

        if (canSwitch)
        {
            await Waiter();
        }
    }
    public async Task Waiter()
    {
        await Task.Delay(500);
        if (!Game1.player.canMove)
        {
            while (!Game1.player.canMove)
            {
                await Task.Delay(20);
            }
        }
        GoToLastIndex();
    }
    public void GoToLastIndex()
    {
        auxIndex = Game1.player.CurrentToolIndex;
        Game1.player.CurrentToolIndex = lastIndex;
        currentIndex = lastIndex;
        lastIndex = auxIndex;

    }
}