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
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.DI;

namespace TehPers.Core.DI
{
    internal class InjectedOptional<T> : IOptional<T>
    {
        private readonly T? service;

        public T Value => this.HasValue ? this.service! : throw new InvalidOperationException("There is no value");
        public bool HasValue { get; }

        public InjectedOptional(IReadOnlyList<T> services)
        {
            if (services.Count == 1)
            {
                this.service = services[0];
                this.HasValue = true;
            }
            else
            {
                this.HasValue = false;
            }
        }

        public bool TryGetValue([NotNullWhen(true)] out T? value)
        {
            if (this.HasValue)
            {
                value = this.service!;
                return true;
            }

            value = default;
            return false;
        }
    }
}