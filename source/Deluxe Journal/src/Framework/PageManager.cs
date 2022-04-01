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

namespace DeluxeJournal.Framework
{
    internal class PageManager
    {
        private readonly IDictionary<string, KeyValuePair<Func<Rectangle, IPage>, int>> Registry;

        public PageManager()
        {
            Registry = new Dictionary<string, KeyValuePair<Func<Rectangle, IPage>, int>>();
        }

        public void RegisterPage(string id, Func<Rectangle, IPage> supplier, int order = 0)
        {
            if (Registry.ContainsKey(id))
            {
                throw new InvalidOperationException("Attempted to overwrite page registry entry with key \"" + id + "\".");
            }

            Registry[id] = new KeyValuePair<Func<Rectangle, IPage>, int>(supplier, order);
        }

        /// <summary>Get a list of IPage instances.</summary>
        /// <param name="pageBounds">Size and position of the IPages.</param>
        public List<IPage> GetPages(Rectangle pageBounds)
        {
            List<IPage> pages = new List<IPage>();
            IPage page;

            int id = 0;

            foreach (Func<Rectangle, IPage> supplier in Registry.Values.OrderByDescending(pair => pair.Value).Select(pair => pair.Key))
            {
                page = supplier(pageBounds);
                page.TabID = id++;
                pages.Add(page);
            }

            return pages;
        }
    }
}
