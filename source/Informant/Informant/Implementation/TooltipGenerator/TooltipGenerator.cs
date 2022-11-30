/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using Slothsoft.Informant.Api;

namespace Slothsoft.Informant.Implementation.TooltipGenerator;

internal class TooltipGenerator<TInput> : ITooltipGenerator<TInput> {
    
    private readonly Func<string> _displayName;
    private readonly Func<string> _description;
    private readonly Func<TInput, string?> _generator;

    public TooltipGenerator(string id, Func<string> displayName, Func<string> description, Func<TInput, string?> generator) {
        Id = id;
        _displayName = displayName;
        _description = description;
        _generator = generator;
    }

    public string Id { get; }
    public string DisplayName => _displayName();
    public string Description => _description();

    public bool HasTooltip(TInput input) {
        return _generator(input) != null;
    }

    public Tooltip Generate(TInput input) {
        return new Tooltip(_generator(input)!);
    }
}