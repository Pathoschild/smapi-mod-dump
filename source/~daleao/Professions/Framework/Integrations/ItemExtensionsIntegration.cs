/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ItemExtensionsIntegration"/> class.</summary>
[ModRequirement("mistyspring.ItemExtensions")]
internal sealed class ItemExtensionsIntegration()
    : ModIntegration<ItemExtensionsIntegration, IItemExtensionsApi>(ModHelper.ModRegistry);
