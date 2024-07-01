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

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal abstract class BaseHandler : IHandler
{
    protected readonly IModHelper Helper;
    protected readonly ModConfig Config;

    protected BaseHandler(IModHelper helper, ModConfig config)
    {
        this.Helper = helper;
        this.Config = config;
    }
    public abstract void Init();
}