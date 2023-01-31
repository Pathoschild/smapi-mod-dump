/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

/******************************************
 * The following file was copied from https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/IContentPatcherAPI.cs.
 *
 * The original license is as below:
 *
 * The MIT License (MIT)
 *
 * Copyright 2016–2021 Jesse Plamondon-Willard (Pathoschild)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 *
 * ****************************************/
namespace AtraShared.Integrations.Interfaces.ContentPatcher;

/// <summary>The Content Patcher API which other mods can access.</summary>
/// <remarks>Copied from https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/IContentPatcherAPI.cs. </remarks>
public interface IContentPatcherAPI
{
    /// <summary>Register a simple token.</summary>
    /// <param name="mod">The manifest of the mod defining the token (see <see cref="Mod.ModManifest"/> in your entry class).</param>
    /// <param name="name">The token name. This only needs to be unique for your mod; Content Patcher will prefix it with your mod ID automatically, like <c>YourName.ExampleMod/SomeTokenName</c>.</param>
    /// <param name="getValue">A function which returns the current token value. If this returns a null or empty list, the token is considered unavailable in the current context and any patches or dynamic tokens using it are disabled.</param>
    void RegisterToken(IManifest mod, string name, Func<IEnumerable<string?>?> getValue);

    /// <summary>Register a complex token. This is an advanced API; only use this method if you've read the documentation and are aware of the consequences.</summary>
    /// <param name="mod">The manifest of the mod defining the token (see <see cref="Mod.ModManifest"/> in your entry class).</param>
    /// <param name="name">The token name. This only needs to be unique for your mod; Content Patcher will prefix it with your mod ID automatically, like <c>YourName.ExampleMod/SomeTokenName</c>.</param>
    /// <param name="token">An arbitrary class with one or more methods from <see cref="ConventionDelegates"/>.</param>
    void RegisterToken(IManifest mod, string name, object token);
}

public interface IFullContentPatcherApi : IContentPatcherAPI
{
    /*********
    ** Accessors
    *********/

    /// <summary>Whether the conditions API is initialized and ready for use.</summary>
    /// <remarks>Due to the Content Patcher lifecycle, the conditions API becomes available roughly two ticks after the <see cref="IGameLoopEvents.GameLaunched"/> event.</remarks>
    bool IsConditionsApiReady { get; }

    /*********
    ** Methods
    *********/

    /// <summary>Get a set of managed conditions which are matched against Content Patcher's internal context.</summary>
    /// <param name="manifest">The manifest of the mod parsing the conditions (see <see cref="Mod.ModManifest"/> in your enter class).</param>
    /// <param name="rawConditions">The conditions to parse, in the same format as <c>When</c> blocks in Content Patcher content packs.</param>
    /// <param name="formatVersion">The format version for which to parse conditions, used to ensure forward compatibility with future Content Patcher versions. See <c>Format</c> in the Content Patcher token documentation.</param>
    /// <param name="assumeModIds">
    /// <para>The unique IDs of mods whose custom tokens to allow in the <paramref name="rawConditions"/>. You don't need to list the mod identified by <paramref name="manifest"/>, mods listed as a required dependency in the <paramref name="manifest"/>, or mods identified by a <c>HasMod</c> condition in the <paramref name="rawConditions"/>.</para>
    /// <para>NOTE: this is meant to prevent mods from breaking if a player doesn't have a required mod installed. You shouldn't simply list all installed mods, and parsing conditions will still fail if a mod isn't installed regardless of the listed mod IDs.</para>
    /// </param>
    IManagedConditions ParseConditions(IManifest manifest, IDictionary<string, string?>? rawConditions, ISemanticVersion formatVersion, string[]? assumeModIds = null);
}