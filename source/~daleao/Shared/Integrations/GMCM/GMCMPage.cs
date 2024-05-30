/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using System;

#endregion using directives

public record GMCMPage(string PageId, string PageTitleKey, Type ParentConfigType, bool LinkToParentPage = false);
