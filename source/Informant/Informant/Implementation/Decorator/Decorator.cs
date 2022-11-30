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
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;

namespace Slothsoft.Informant.Implementation.Decorator;

internal class Decorator<TInput> : IDecorator<TInput> {

    private readonly Func<string> _displayName;
    private readonly Func<string> _description;
    private readonly Func<TInput, Texture2D?> _decorator;

    public Decorator(string id, Func<string> displayName, Func<string> description, Func<TInput, Texture2D?> decorator) {
        Id = id;
        _displayName = displayName;
        _description = description;
        _decorator = decorator;
    }

    public string Id { get; }
    public string DisplayName => _displayName();
    public string Description => _description();

    public bool HasDecoration(TInput input) {
        return _decorator(input) != null;
    }

    public Decoration Decorate(TInput input) {
        return new Decoration(_decorator(input)!);
    }
}