/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using DeluxeJournal.Menus;
using DeluxeJournal.Framework;

namespace DeluxeJournal.Api
{
    public class DeluxeJournalApi : IDeluxeJournalApi
    {
        private readonly PageManager _pageManager;

        internal DeluxeJournalApi(DeluxeJournalMod mod)
        {
            if (mod.PageManager == null)
            {
                throw new InvalidOperationException("Deluxe Journal API instantiated before mod entry.");
            }

            _pageManager = mod.PageManager;
        }

        public void RegisterPage(string id, Func<Rectangle, IPage> supplier, int order = 0)
        {
            _pageManager.RegisterPage(id, supplier, order);
        }
    }
}
