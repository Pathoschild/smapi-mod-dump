/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.ContentPatcher;
#else
namespace StardewMods.Common.Services.Integrations.ContentPatcher;
#endif

/// <summary>Raised when the Content Patcher Conditions Api is ready.</summary>
internal sealed class ConditionsApiReadyEventArgs : EventArgs { }