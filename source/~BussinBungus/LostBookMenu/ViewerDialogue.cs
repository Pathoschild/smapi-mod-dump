/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BussinBungus/BungusSDVMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace LostBookMenu
{
    internal class ViewerDialogue : DialogueBox
    {
        public ViewerDialogue(string dialogue) : base(dialogue)
        {
            Game1.afterDialogues = delegate
            {
                Game1.activeClickableMenu = new BookMenu();
            };
        }
    }
}