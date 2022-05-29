/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class FishingEffectManagerFactory
    {
        private readonly IModKernel kernel;
        private readonly CalculatorFactory calculatorFactory;

        public FishingEffectManagerFactory(IModKernel kernel, CalculatorFactory calculatorFactory)
        {
            this.kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            this.calculatorFactory = calculatorFactory
                ?? throw new ArgumentNullException(nameof(calculatorFactory));
        }

        public FishingEffectManager Create(IManifest owner, FishingEffectEntry entry)
        {
            return new(
                this.kernel.GlobalKernel,
                this.calculatorFactory.Conditions(owner, entry.Conditions),
                entry
            );
        }
    }
}
