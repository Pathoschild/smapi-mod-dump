/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Contained;

namespace ItemExtensions.Models;

public class ItemData
{
    //misc
    public int MaximumStack { get; set; } = -1;
    public bool HideItem { get; set; } = false;

    //more capability (?)
    public LightData Light { get; set; } = null;
        
    //when x happens
    public OnBehavior OnEquip { get; set; } = null;
    public OnBehavior OnUnequip { get; set; } = null;
    public OnBehavior OnUse { get; set; } = null;
    public OnBehavior OnDrop { get; set; } = null;
    public OnBehavior OnPurchase { get; set; } = null;
    
    // before/after eating
    public FarmerAnimation Eating { get; set; } = null;
    public FarmerAnimation AfterEating { get; set; } = null;
}