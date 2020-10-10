/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System;
using System.Reflection;
using Igorious.StardewValley.DynamicApi2.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Log = Igorious.StardewValley.DynamicApi2.Utils.Log;

namespace Igorious.StardewValley.DynamicApi2.Compatibility
{
    public sealed class EntoaroxFrameworkСompatibilityLayout
    {
        private const string EntoaroxFrameworkID = "Entoarox.EntoaroxFramework";

        private static readonly Lazy<EntoaroxFrameworkСompatibilityLayout> Lazy = new Lazy<EntoaroxFrameworkСompatibilityLayout>(() => new EntoaroxFrameworkСompatibilityLayout());
        public static EntoaroxFrameworkСompatibilityLayout Instance => Lazy.Value;

        private bool UseCompatibility { get; }

        public event Action ContentIsReadyToOverride;

        private EntoaroxFrameworkСompatibilityLayout()
        {
            UseCompatibility = Smapi.GetModRegistry().IsLoaded(EntoaroxFrameworkID);
            if (UseCompatibility) Log.Trace("Found Entoarox Framework. Used compatibility mode.");
            GameEvents.LoadContent += GameEventsOnLoadContent;
        }

        private void GameEventsOnLoadContent(object sender, EventArgs eventArgs)
        {
            GameEvents.LoadContent -= GameEventsOnLoadContent;
            if (UseCompatibility)
            {                
                GameEvents.UpdateTick += GameEventsOnUpdateTick;
            }
            else
            {
                ContentIsReadyToOverride?.Invoke();
            }
        }

        private void GameEventsOnUpdateTick(object sender, EventArgs eventArgs)
        {
            if (Game1.content.GetType() == typeof(LocalizedContentManager)) return;
            GameEvents.UpdateTick -= GameEventsOnUpdateTick;
            ContentIsReadyToOverride?.Invoke();
        }
    }
}