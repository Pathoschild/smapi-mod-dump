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

namespace DeluxeJournal.Api
{
    public interface IDeluxeJournalApi
    {
        /// <summary>Register a custom IPage.</summary>
        /// <param name="id">Unique page id.</param>
        /// <param name="supplier">A function that takes the page bounds as a parameter and returns an IPage instance.</param>
        /// <param name="order">An optional ordering value for the tab position. Higher values tend toward the top of the journal.</param>
        public void RegisterPage(string id, Func<Rectangle, IPage> supplier, int order = 0);
    }
}
