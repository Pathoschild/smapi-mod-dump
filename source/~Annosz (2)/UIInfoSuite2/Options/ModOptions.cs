/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System.Collections.Generic;

namespace UIInfoSuite2.Options;

internal record ModOptions
{
  public bool AllowExperienceBarToFadeOut { get; set; } = true;
  public bool ShowExperienceBar { get; set; } = true;
  public bool ShowExperienceGain { get; set; } = true;
  public bool ShowLevelUpAnimation { get; set; } = true;
  public bool ShowHeartFills { get; set; } = true;
  public bool ShowExtraItemInformation { get; set; } = true;
  public bool ShowLocationOfTownsPeople { get; set; } = true;
  public bool ShowLuckIcon { get; set; } = true;
  public bool ShowTravelingMerchant { get; set; } = true;
  public bool ShowRainyDay { get; set; } = true;
  public bool ShowCropAndBarrelTooltip { get; set; } = true;
  public bool ShowBirthdayIcon { get; set; } = true;
  public bool ShowAnimalsNeedPets { get; set; } = true;
  public bool HideAnimalPetOnMaxFriendship { get; set; } = true;
  public bool ShowItemEffectRanges { get; set; } = true;
  public bool ShowItemsRequiredForBundles { get; set; } = true;
  public bool ShowHarvestPricesInShop { get; set; } = true;
  public bool DisplayCalendarAndBillboard { get; set; } = true;
  public bool ShowWhenNewRecipesAreAvailable { get; set; } = true;
  public bool ShowToolUpgradeStatus { get; set; } = true;
  public bool HideMerchantWhenVisited { get; set; } = false;
  public bool ShowExactValue { get; set; } = false;
  public bool ShowRobinBuildingStatusIcon { get; set; } = true;
  public bool ShowSeasonalBerry { get; set; } = true;
  public bool ShowSeasonalBerryHazelnut { get; set; } = false;
  public bool ShowTodaysGifts { get; set; } = true;
  public bool HideBirthdayIfFullFriendShip { get; set; } = true;
  public Dictionary<string, bool> ShowLocationOfFriends { get; set; } = new();
}
