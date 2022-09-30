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

namespace Leclair.Stardew.BCBuildings;

public class ModConfig {

	public string? CostAdditional { get; set; } = null;

	public int CostMaterial { get; set; } = 100;

	public int CostCurrency { get; set; } = 100;

	public int RefundMaterial { get; set; } = 0;

	public int RefundCurrency { get; set; } = 0;

	public bool AllowMovingUnfinishedGreenhouse { get; set; } = false;

}
