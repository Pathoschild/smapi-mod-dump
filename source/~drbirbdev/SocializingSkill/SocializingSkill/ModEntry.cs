/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbCore.Attributes;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SocializingSkill;

[SMod]
public class ModEntry : Mod
{
    [SMod.Instance]
    internal static ModEntry Instance;
    internal static Config Config;
    internal static Assets Assets;

    internal ITranslationHelper I18N => this.Helper.Translation;

    internal static readonly PerScreen<List<string>> BelovedCheckedToday = new();

    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);
    }
}
