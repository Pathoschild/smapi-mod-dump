/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api.DI;

namespace TehPers.Core.DI
{
    internal sealed class ModKernelFactory : IModKernelFactory
    {
        private readonly GlobalKernel globalKernel = new();
        private readonly Dictionary<IMod, IModKernel> modKernels = new();
        private readonly HashSet<Action<IModKernel>> kernelProcessors = new();

        public IResolutionRoot GlobalServices => this.globalKernel;

        public ModKernelFactory()
        {
            this.RegisterGlobalServices();
        }

        private void RegisterGlobalServices()
        {
            this.globalKernel.Bind(typeof(IOptional<>))
                .To(typeof(InjectedOptional<>))
                .InTransientScope();
            this.globalKernel.Bind(typeof(ISimpleFactory<>))
                .To(typeof(SimpleFactory<>))
                .InSingletonScope();
            this.globalKernel.Bind<IModKernelFactory>()
                .ToConstant(this)
                .InSingletonScope();
        }

        public void AddKernelProcessor(Action<IModKernel> processor)
        {
            this.kernelProcessors.Add(processor);
            foreach (var kernel in this.modKernels.Values)
            {
                processor(kernel);
            }
        }

        public IModKernel GetKernel(IMod owner)
        {
            if (this.modKernels.TryGetValue(owner, out var modKernel))
            {
                return modKernel;
            }

            modKernel = new ModKernel(
                owner,
                this.globalKernel,
                this,
                new NinjectSettings
                {
                    LoadExtensions = false,
                }
            );

            foreach (var processor in this.kernelProcessors)
            {
                processor(modKernel);
            }

            this.modKernels.Add(owner, modKernel);
            return modKernel;
        }
    }
}