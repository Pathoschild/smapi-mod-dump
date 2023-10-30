/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Just-Chaldea/GiftTasteHelper
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework
{
    // TODO: document
    internal interface IGiftHelper
    {
        /*********
        ** Accessors
        *********/
        bool IsInitialized { get; }
        bool IsOpen { get; }


        /*********
        ** Methods
        *********/
        void Init(IClickableMenu menu);
        void Reset();
        bool OnOpen(IClickableMenu menu);
        void OnResize(IClickableMenu menu);
        void OnClose();

        /// <summary>Should this helper recieve OnDraw events.</summary>
        bool CanDraw();
        void OnDraw();

        /// <summary>Should this helper receive input and tick events. Checked every tick.</summary>
        bool CanTick();
        void OnCursorMoved(CursorMovedEventArgs e);

        /// <summary>Indicates if this helper wants to receive OnPostUpdate events.</summary>
        bool WantsUpdateEvent();
        void OnPostUpdate(UpdateTickedEventArgs e);
    }
}
