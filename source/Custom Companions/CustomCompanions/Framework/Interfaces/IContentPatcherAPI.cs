/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace CustomCompanions.Framework.Interfaces
{
    public interface IContentPatcherAPI
    {
        /// <summary>Register a complex token. This is an advanced API; only use this method if you've read the documentation and are aware of the consequences.</summary>
        /// <param name="mod">The manifest of the mod defining the token (see <see cref="Mod.ModManifest"/> in your entry class).</param>
        /// <param name="name">The token name. This only needs to be unique for your mod; Content Patcher will prefix it with your mod ID automatically, like <c>YourName.ExampleMod/SomeTokenName</c>.</param>
        /// <param name="token">An arbitrary class with one or more methods from <see cref="ConventionDelegates"/>.</param>
        void RegisterToken(IManifest mod, string name, object token);
    }
}
