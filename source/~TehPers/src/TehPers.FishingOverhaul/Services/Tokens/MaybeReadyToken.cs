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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.FishingOverhaul.Services.Tokens
{
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Used by Content Patcher."
    )]
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Used by Content Patcher."
    )]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by Content Patcher.")]
    internal class MaybeReadyToken
    {
        private readonly Func<IEnumerable<string>?> getValues;
        private List<string>? lastValues;

        public MaybeReadyToken(Func<IEnumerable<string>?> getValues)
        {
            this.getValues = getValues ?? throw new ArgumentNullException(nameof(getValues));
            this.lastValues = null;
        }

        public bool UpdateContext()
        {
            var prevValues = this.lastValues;
            this.lastValues = this.getValues()?.ToList();
            return (prevValues, this.lastValues) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (var a, var b) => !a.SequenceEqual(b),
            };
        }

        public bool IsReady()
        {
            return this.lastValues is not null;
        }

        public IEnumerable<string> GetValues(string? input)
        {
            return this.lastValues ?? Enumerable.Empty<string>();
        }
    }
}
