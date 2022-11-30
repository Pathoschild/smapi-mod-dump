/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Slothsoft.Informant.Api;

namespace Slothsoft.Informant.Implementation; 

internal class BaseTooltipGeneratorManager<TInput> : ITooltipGeneratorManager<TInput> {

    private readonly List<ITooltipGenerator<TInput>> _generators = new();

    public IEnumerable<IDisplayable> Generators => _generators.ToImmutableArray();

    internal IEnumerable<Tooltip> Generate(params TInput[] inputs) {
        var config = InformantMod.Instance?.Config ?? new InformantConfig();
        return _generators
            .Where(g => config.DisplayIds.GetValueOrDefault(g.Id, true))
            .SelectMany(g => inputs
                .Where(g.HasTooltip)
                .Select(g.Generate)
            );
    }
    
    public void Add(ITooltipGenerator<TInput> generator) {
        _generators.Add(generator);
    }

    public void Remove(string generatorId) {
        _generators.RemoveAll(g => g.Id == generatorId);
    }
}