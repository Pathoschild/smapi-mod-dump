/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace GingerIslandStart.Additions;

public class ItemSaveData
{

    public List<BaseEnchantment> Enchantments { get; set; }
    public List<Object> Attachments { get; set; }
    public int Count { get; set; }
    public bool Recipe { get; set; }
    public int SpecialVariable { get; set; }
    public string QualifiedItemId { get; set; }
    public int Quality { get; set; }
    
    public ItemSaveData()
    {}
    
    public ItemSaveData(Item item)
    {
        Attachments = GetAttachments(item);
        Enchantments = GetEnchantments(item);
        Recipe = item.IsRecipe;
        SpecialVariable = item.SpecialVariable;
        Count = item.Stack;
        QualifiedItemId = item.QualifiedItemId;
        Quality = item.Quality;
    }
    
    private static List<Object> GetAttachments(Item item)
    {
        if (item is not FishingRod f)
            return null;
        
        var result = new List<Object>();
        foreach (var slot in f.attachments)
        {
            result.Add(slot);
        }

        return result;
    }

    private static List<BaseEnchantment> GetEnchantments(Item item)
    {
        if (item is not Tool t)
            return null;
        
        var result = new List<BaseEnchantment>();
        foreach (var baseEnchantment in t.enchantments)
        {
            result.Add(baseEnchantment);
        }

        return result;
    }

    public Item GetItem()
    {
        var result = ItemRegistry.Create(QualifiedItemId, Count, Quality);

        result.SpecialVariable = SpecialVariable;
        result.IsRecipe = Recipe;
        
        if (result is FishingRod f)
        {
            f.attachments.Set(Attachments);
            return f;
        }

        if (result is Tool t)
        {
            t.enchantments.Set(Enchantments);
            return t;
        }

        return result;
    }
}