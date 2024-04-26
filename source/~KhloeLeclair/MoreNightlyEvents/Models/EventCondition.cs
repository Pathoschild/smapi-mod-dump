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

using Leclair.Stardew.Common;

using Newtonsoft.Json;

namespace Leclair.Stardew.MoreNightlyEvents.Models;

public class EventCondition {

	public string? Condition { get; set; }

	public float Weight { get; set; } = 1f;

	public float Chance { get; set; } = 1f;

	public bool IsExclusive { get; set; } = false;

}
