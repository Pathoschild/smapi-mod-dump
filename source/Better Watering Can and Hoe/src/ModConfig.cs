/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Aflojack/BetterWateringCanAndHoe
**
*************************************************/

using StardewModdingAPI;

class ModConfig{
   public bool BetterWateringCanModEnabled { get; set; } = true;
   public bool BetterHoeModEnabled { get; set; } = true;
   public SButton SelectionOpenKey { get; set; } = SButton.R;
   public bool WateringCanAlwaysHighestOption { get; set; } = false;
   public bool HoeAlwaysHighestOption { get; set; } = false;
   public bool WateringCanSelectTemporary { get; set; } = false;
   public bool HoeSelectTemporary { get; set; } = false;
   public int WateringCanTimerStart { get; set; } = 3600;
   public int HoeTimerStart { get; set; } = 3600;
}