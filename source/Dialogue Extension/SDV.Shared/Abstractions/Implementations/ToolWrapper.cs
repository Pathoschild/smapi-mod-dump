/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class ToolWrapper : ItemWrapper, IToolWrapper
  {
    public ToolWrapper(Item item) : base(item)
    {
    }

    public int InitialParentTileIndex { get; set; }
    public int CurrentParentTileIndex { get; set; }
    public int IndexOfMenuItemView { get; set; }
    public bool Stackable { get; set; }
    public bool InstantUse { get; set; }
    public bool IsEfficient { get; set; }
    public float AnimationSpeedModifier { get; set; }
    public int UpgradeLevel { get; set; }
    public string description { get; set; }
    public string Description { get; }
    public string BaseName { get; set; }
    public void draw(SpriteBatch b)
    {
    }

    public void tickUpdate(GameTime time, IFarmerWrapper who)
    {
    }

    public bool isHeavyHitter() => false;

    public void Update(int direction, int farmerMotionFrame, IFarmerWrapper who)
    {
    }

    public IFarmerWrapper getLastFarmerToUse() => null;

    public void leftClick(IFarmerWrapper who)
    {
    }

    public void DoFunction(IGameLocationWrapper location, int x, int y, int power, IFarmerWrapper who)
    {
    }

    public void endUsing(IGameLocationWrapper location, IFarmerWrapper who)
    {
    }

    public bool beginUsing(IGameLocationWrapper location, int x, int y, IFarmerWrapper who) => false;

    public bool onRelease(IGameLocationWrapper location, int x, int y, IFarmerWrapper who) => false;

    public bool canThisBeAttached(IObjectWrapper o) => false;

    public IObjectWrapper attach(IObjectWrapper o) => null;

    public void colorTool(int level)
    {
    }

    public void actionWhenClaimed()
    {
    }
    
    public bool doesShowTileLocationMarker() => false;

    public void setNewTileIndexForUpgradeLevel()
    {
    }

    public void ClearEnchantments()
    {
    }

    public int GetMaxForges() => 0;

    public int GetSecondaryEnchantmentCount() => 0;

    public bool CanAddEnchantment(IBaseEnchantmentWrapper enchantment) => false;

    public void CopyEnchantments(IToolWrapper source, IToolWrapper destination)
    {
    }

    public int GetTotalForgeLevels(bool for_unforge = false) => 0;

    public bool AddEnchantment(IBaseEnchantmentWrapper enchantment) => false;

    public bool hasEnchantmentOfType<T>() => false;

    public void RemoveEnchantment(IBaseEnchantmentWrapper enchantment)
    {
    }

    public bool CanUseOnStandingTile() => false;

    public bool CanForge(IItemWrapper item) => false;

    public T GetEnchantmentOfType<T>() where T : IBaseEnchantmentWrapper => default;

    public int GetEnchantmentLevel<T>() where T : IBaseEnchantmentWrapper => 0;

    public bool Forge(IItemWrapper item, bool count_towards_stats = false) => false;
  }
}
