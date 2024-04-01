/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace ItemExtensions.Additions;

public static class ModKeys
{
    /// <summary> For extra trade requirements in shops. </summary>
    public const string ExtraTradesKey = "mistyspring.ItemExtensions/ExtraTrades";
    
    //eating/drinking customization
    /// <summary> Sets animation after consuming item. </summary>
    public const string AfterEating = "mistyspring.ItemExtensions/AfterEatingAnimation";
    /// <summary> Sets custom animation for item consumption. </summary>
    public const string CustomEating = "mistyspring.ItemExtensions/EatingAnimation";
    /// <summary> Customizes drink color in animation. </summary>
    public const string DrinkColor = "mistyspring.ItemExtensions/DrinkColor";
    
    //small changes via customfields
    /// <summary> Modifies item's maximum stack. </summary>
    public const string MaxStack = "mistyspring.ItemExtensions/MaximumStack";
    /// <summary> Can disable item showing above head. </summary>
    public const string ShowAboveHead = "mistyspring.ItemExtensions/ShowAboveHead";
    
    //set for all small resources
    public const string Resource = "mistyspring.ItemExtensions/Resource";
    
    //for spawning
    /// <summary> For setting an allowed rectangle in resourceClump spawn. Must have the same parameters as a rectangle would, ie: "x y width height"</summary>
    public const string SpawnRect = "mistyspring.ItemExtensions/ClumpSpawnRect";
    /// <summary> By default, clumps avoid spawning over other items. If for whichever reason the user wants to disable it: add this key with value "false" to location's CustomFields </summary>
    public const string AvoidOverlap = "mistyspring.ItemExtensions/AvoidOverlap";
    
    //for custom clump actions
    /// <summary> Resource Clump's Id, for custom behavior and drops. </summary>
    public const string ClumpId = "mistyspring.ItemExtensions/CustomClumpId";
    //clump light
    /// <summary> Light Id, used when removing from map. </summary>
    public const string LightId = "mistyspring.ItemExtensions/LightId";
    /// <summary> Light size, used when placing on map. </summary>
    public const string LightSize = "mistyspring.ItemExtensions/LightSize";
    /// <summary> Light color, used when placing on map. </summary>
    public const string LightColor = "mistyspring.ItemExtensions/LightColor";
    /// <summary> Light transparency, used when placing on map. </summary>
    public const string LightTransparency = "mistyspring.ItemExtensions/LightTrans";

    //simpler version of SeedData
    /// <summary> Sets the allowed mixed seeds. </summary>
    public const string MixedSeeds = "mistyspring.ItemExtensions/MixedSeeds";
    /// <summary> If the user doesn't want this seed to be included in the roster. </summary>
    public const string AddMainSeed = "mistyspring.ItemExtensions/AddMainSeed";
    
    //for spawning
    public const string RandomClumpForage = "mistyspring.ItemExtensions_RANDOM_CLUMPS";
    public const string AllClumpsForage = "mistyspring.ItemExtensions_ALL_CLUMPS";
}