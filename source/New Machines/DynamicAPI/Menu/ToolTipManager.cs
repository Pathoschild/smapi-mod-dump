/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewModdingAPI.Events;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public sealed class ToolTipManager
    {
        private static ToolTipManager _instance;

        public static ToolTipManager Instance => _instance ?? (_instance = new ToolTipManager());

        private ToolTipManager()
        {
            PlayerEvents.LoadedGame += (s, e) =>
            {
                GraphicsEvents.OnPostRenderEvent += OnRender;
            };
        }

        private readonly List<GridToolTip> _currentToolTips = new List<GridToolTip>();

        public IReadOnlyList<GridToolTip> CurrentToolTips => _currentToolTips;

        private void OnRender(object sender, EventArgs e)
        {
            try
            {
                var previewMenus = _registeredMenus.Where(m => m.NeedDraw()).ToList();
                _currentToolTips.Clear();
                _currentToolTips.AddRange(previewMenus);
                var exclusive = _currentToolTips.Where(m => m.IsExclusive).ToList();
                var drawableMenus = exclusive.Any() ? exclusive : _currentToolTips;
                foreach (var drawableMenu in drawableMenus)
                {
                    drawableMenu.Draw();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                GraphicsEvents.OnPostRenderEvent -= OnRender;
            }
        }

        private readonly List<GridToolTip> _registeredMenus = new List<GridToolTip>();

        public void Register<TMenu>() where TMenu : GridToolTip, new()
        {
            _registeredMenus.Add(new TMenu());
        }
    }
}