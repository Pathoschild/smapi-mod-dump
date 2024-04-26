/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Models;

public interface IEffect {

	ulong Id { get; }

	uint Rate { get; }

	void ReloadAssets() { }

	void Update(GameTime time);

	void Remove() { }

}
