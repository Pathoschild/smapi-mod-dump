/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul
{
    [SuppressMessage(
        "ReSharper",
        "UnusedMember.Global",
        Justification = "Class initialized and used by SMAPI."
    )]
    internal class ModFishing : Mod
    {
        private IModKernel? kernel;

        public override void Entry(IModHelper helper)
        {
            if (ModServices.Factory is not { } kernelFactory)
            {
                this.Monitor.Log(
                    "Core mod seems to not be loaded. Aborting setup - this mod is effectively disabled.",
                    LogLevel.Error
                );

                return;
            }

            this.kernel = kernelFactory.GetKernel(this);
            this.kernel.Load<FishingModule>();
        }

        public override object? GetApi()
        {
            return this.kernel?.Get<ISimplifiedFishingApi>();
        }
    }
}
