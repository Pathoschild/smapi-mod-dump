/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly ModConfig Config;
    protected readonly IModHelper Helper;

    protected BaseHandler(ModConfig config, IModHelper helper)
    {
        this.Config = config;
        this.Helper = helper;
    }

    public abstract void Init();
    public abstract void Clear();
}