/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using Shockah.Kokoro;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Shockah.EarlyGingerIsland;

public class ModConfig : IVersioned.Modifiable
{
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }
	[JsonProperty] public bool SkipBoatCutscene { get; internal set; } = false;
	[JsonProperty] public int BoatTicketPrice { get; internal set; } = 1000;
	[JsonProperty] public bool AllowIslandFarmBeforeCC { get; internal set; } = false;
	[JsonProperty] public PlantingOnIslandFarmBeforeCC PlantingOnIslandFarmBeforeCC { get; internal set; } = PlantingOnIslandFarmBeforeCC.OnlyOneCrop;
	[JsonProperty] public int BoatFixHardwoodRequired { get; internal set; } = 200;
	[JsonProperty] public int BoatFixIridiumBarsRequired { get; internal set; } = 5;
	[JsonProperty] public int BoatFixBatteryPacksRequired { get; internal set; } = 5;
	[JsonProperty] public int GoldenWalnutsRequiredForQiRoom { get; internal set; } = 100;

	[JsonProperty] public bool IgnoreFreeUnlockRequirements { get; internal set; } = false;
	[JsonProperty] public int FirstUnlockCost { get; internal set; } = 1;
	[JsonProperty] public int WestUnlockCost { get; internal set; } = 10;
	[JsonProperty] public int FarmhouseUnlockCost { get; internal set; } = 20;
	[JsonProperty] public int MailboxUnlockCost { get; internal set; } = 5;
	[JsonProperty] public int ObeliskUnlockCost { get; internal set; } = 20;
	[JsonProperty] public int DigsiteUnlockCost { get; internal set; } = 10;
	[JsonProperty] public int TraderUnlockCost { get; internal set; } = 10;
	[JsonProperty] public int VolcanoBridgeUnlockCost { get; internal set; } = 5;
	[JsonProperty] public int VolcanoExitShortcutUnlockCost { get; internal set; } = 5;
	[JsonProperty] public int ResortUnlockCost { get; internal set; } = 20;
	[JsonProperty] public int ParrotExpressUnlockCost { get; internal set; } = 10;

	[JsonProperty] public List<UnlockCondition> UnlockConditions { get; internal set; } = [new(new(1, "spring", 1), 8), new(new(1, "winter", 1), 4), new(new(2, "winter", 1), 0)];
}