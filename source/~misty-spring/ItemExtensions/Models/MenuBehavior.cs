/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text.RegularExpressions;
using ItemExtensions.Models.Enums;
using ItemExtensions.Models.Internal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Triggers;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace ItemExtensions.Models;

/// <summary>
/// Behavior on item used.
/// </summary>
/// <see cref="CachedTriggerAction"/>
/// <see cref="StardewValley.Menus.GameMenu"/>
public class MenuBehavior : IWorldChangeData
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    
    public string TargetId { get; set; } //qualified item ID
    public List<string> RandomItemId { get; set; } = new();
    public int RemoveAmount { get; set; }
    
    public string ReplaceBy { get; set; } //qualified item ID
    public bool RetainQuality { get; set; } = true;
    public bool RetainAmount { get; set; } = true;
    
    public int TextureIndex { get; set; } = -1;
    
    public Dictionary<string,string> AddModData { get; set; } = new();
    
    public string QualityChange { get; set; }
    internal int ActualQuality { get; set; }
    internal Modifier QualityModifier { get; set; }
    
    public string PriceChange { get; set; }
    internal double ActualPrice { get; set; }
    internal Modifier PriceModifier { get; set; }
    
    //from iworldchange
    public string Health { get; set; }
    public string Stamina { get; set; }
    
    public Dictionary<string, int> AddItems { get; set; } = new();
    public Dictionary<string, int> RemoveItems { get; set; } = new();
    
    public string PlayMusic { get; set; }
    public string PlaySound { get; set; }
    
    public string TriggerAction { get; set; }
    public List<string> RemoveFlags { get; set; } = new();
    public string Conditions { get; set; }

    public List<string> AddContextTags { get; set; } = new();
    public List<string> RemoveContextTags { get; set; } = new();
    
    public string AddQuest { get; set; }
    public string AddSpecialOrder { get; set; }
    
    public string RemoveQuest { get; set; }
    public string RemoveSpecialOrder { get; set; }
    public List<string> AddFlags { get; set; } = new();

    public MenuBehavior()
    {}
    
    public bool Parse(out MenuBehavior o)
    {
        try
        {
            Price();
        }
        catch (Exception e)
        {
            Log("Error when parsing price: "+ e, LogLevel.Error);
            o = null;
            return false;
        }
        
        try
        {
            Quality();
        }
        catch (Exception e)
        {
            Log("Error when parsing quality: "+ e, LogLevel.Error);
            o = null;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(PlaySound) && !Game1.soundBank.Exists(PlaySound))
        {
            Log($"Error: Sound doesn't exist. ({PlaySound})", LogLevel.Error);
            o = null;
            return false;
        }

        var target = ItemRegistry.GetDataOrErrorItem(TargetId);
        if (target.DisplayName == ItemRegistry.GetErrorItemName())
        {
            Log("Error finding item. Behavior won't be added.", LogLevel.Error);
            o = null;
            return false;
        }
        
        o = this;
        
        return true;
    }
    
    /// <summary>
    /// Check which price change we're doing, and set modifier
    /// </summary>
    private void Price()
    {
        if (string.IsNullOrWhiteSpace(PriceChange))
            return;
        
        var raw = PriceChange.Replace(" ", "").Replace(',','.');
        //var first = raw.AsSpan(0,1);
        if (raw.Contains('+'))
        {
            PriceModifier = Modifier.Sum;
        }
        else if (raw.Contains('-'))
        {
            PriceModifier = Modifier.Substract;
        }
        else if (raw.Contains(':') || raw.Contains('/') || raw.Contains('\\'))
        {
            PriceModifier = Modifier.Divide;
        }
        else if (raw.Contains('*') || raw.Contains('x'))
        {
            PriceModifier = Modifier.Multiply;
        }
        else if (raw.Contains('%'))
        {
            PriceModifier = Modifier.Percentage;
        }
        else
        {
            PriceModifier = Modifier.Set;
        }
        
        var stripped = Regex.Replace(raw, "[^0-9.]", "");
        ModEntry.Mon.Log("Stripped string: " + stripped);
        ActualPrice = int.Parse(stripped);
    }

    private void Quality()
    {
        if (string.IsNullOrWhiteSpace(QualityChange))
            return;
        
        var raw = QualityChange.Replace(" ", "");
        //var first = raw.AsSpan(0,1);
        if (raw.Contains('+'))
        {
            QualityModifier = Modifier.Sum;
        }
        else if (raw.Contains('-'))
        {
            QualityModifier = Modifier.Substract;
        }
        else
        {
            QualityModifier = Modifier.Set;
        }
        
        var stripped = Regex.Replace(raw, "[^0-9]", "");
        ActualQuality = int.Parse(stripped);
    }
}