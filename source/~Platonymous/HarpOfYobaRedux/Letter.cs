/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using PyTK.Types;
using StardewValley;

namespace HarpOfYobaRedux
{
    internal class Letter : Mail
    {
        public Item item;

        public Letter()
        {

        }

        public Letter(string id, string text, Item item = null)
            :base()
        {
            this.text = text + " %item object 388 1 %%";
            this.attachments = null;
            if (item == null)
                item = new SheetMusic(id);

            this.id = "hoy_" + id;
            this.item = item;
            injectIntoMail();
        }

    }
}
