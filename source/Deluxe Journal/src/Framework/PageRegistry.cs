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
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Menus;

namespace DeluxeJournal.Framework
{
    internal static class PageRegistry
    {
        private readonly struct RegistryEntry(PageSupplier pageSupplier, OverlaySupplier? overlaySupplier, int orderPriority)
        {
            public PageSupplier PageSupplier => pageSupplier;

            public OverlaySupplier? OverlaySupplier => overlaySupplier;

            public int OrderPriority => orderPriority;
        }

        /// <summary>Supply an <see cref="IPage"/> instance.</summary>
        /// <param name="bounds">Page bounds.</param>
        public delegate IPage PageSupplier(Rectangle bounds);

        /// <summary>Supply an <see cref="IOverlay"/> instance.</summary>
        /// <param name="bounds">Overlay bounds.</param>
        public delegate IOverlay OverlaySupplier(Rectangle bounds);

        /// <summary>Page registry map.</summary>
        private static readonly IDictionary<string, RegistryEntry> Registry = new Dictionary<string, RegistryEntry>();

        /// <summary>All registered page IDs.</summary>
        public static IEnumerable<string> Keys => Registry.Keys;

        /// <summary>All registered page IDs in order of priority.</summary>
        public static IEnumerable<string> PriorityOrderedKeys => Registry.OrderByDescending(pair => pair.Value.OrderPriority).Select(pair => pair.Key);

        /// <summary>Register a new page.</summary>
        /// <param name="id">Unique page ID.</param>
        /// <param name="pageSupplier">Supplier that creates an <see cref="IPage"/>.</param>
        /// <param name="overlaySupplier">Supplier that creates an <see cref="IOverlay"/>.</param>
        /// <param name="orderPriority">An order priority value for the tab position. Higher values tend toward the top of the journal.</param>
        public static void Register(string id, PageSupplier pageSupplier, OverlaySupplier? overlaySupplier = null, int orderPriority = 0)
        {
            if (Registry.ContainsKey(id))
            {
                throw new InvalidOperationException($"Attempted to overwrite page registry entry with key '{id}'.");
            }

            Registry.Add(id, new(pageSupplier, overlaySupplier, orderPriority));
        }

        /// <summary>Create an ordered list of <see cref="IPage"/> instances by invoking all registered suppliers.</summary>
        /// <param name="bounds">Bounds that each page will be given.</param>
        /// <returns>A list of <see cref="IPage"/> instances ordered by priority value.</returns>
        public static List<IPage> CreatePages(Rectangle bounds)
        {
            List<IPage> pages = new(Registry.Count);
            int tab = 0;

            foreach ((string pageId, RegistryEntry entry) in Registry.OrderByDescending(pair => pair.Value.OrderPriority))
            {
                IPage page = entry.PageSupplier(bounds);
                page.PageId = pageId;
                page.TabId = tab++;
                pages.Add(page);
            }

            return pages;
        }

        /// <summary>Create an <see cref="IOverlay"/> instance.</summary>
        /// <param name="id">Registered page ID.</param>
        /// <param name="settings">Overlay settings to be applied.</param>
        /// <returns>An <see cref="IOverlay"/> instance or <c>null</c> if no supplier exists for the given page ID.</returns>
        public static IOverlay? CreateOverlay(string id, OverlaySettings settings)
        {
            if (Registry.TryGetValue(id, out var entry) && entry.OverlaySupplier is OverlaySupplier supplier)
            {
                IOverlay overlay = supplier(settings.Bounds);
                overlay.PageId = id;
                settings.Apply(overlay);
                return overlay;
            }

            return null;
        }
    }
}
