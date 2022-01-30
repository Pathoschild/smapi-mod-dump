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

namespace SDV.Shared.Abstractions
{
  public interface IToolWrapper : IItemWrapper
  {
    int InitialParentTileIndex { get; set; }
    int CurrentParentTileIndex { get; set; }
    int IndexOfMenuItemView { get; set; }
    bool Stackable { get; set; }
    bool InstantUse { get; set; }
    bool IsEfficient { get; set; }
    float AnimationSpeedModifier { get; set; }
    int UpgradeLevel { get; set; }
    string description { get; set; }
    string Description { get; }
    string BaseName { get; set; }
    void draw(SpriteBatch b);

    void tickUpdate(GameTime time, IFarmerWrapper who);
    bool isHeavyHitter();
    void Update(int direction, int farmerMotionFrame, IFarmerWrapper who);
    IFarmerWrapper getLastFarmerToUse();
    void leftClick(IFarmerWrapper who);
    void DoFunction(IGameLocationWrapper location, int x, int y, int power, IFarmerWrapper who);
    void endUsing(IGameLocationWrapper location, IFarmerWrapper who);
    bool beginUsing(IGameLocationWrapper location, int x, int y, IFarmerWrapper who);
    bool onRelease(IGameLocationWrapper location, int x, int y, IFarmerWrapper who);
    bool canThisBeAttached(IObjectWrapper o);
    IObjectWrapper attach(IObjectWrapper o);
    void colorTool(int level);
    void actionWhenClaimed();
    bool doesShowTileLocationMarker();
    void setNewTileIndexForUpgradeLevel();
    void ClearEnchantments();
    int GetMaxForges();
    int GetSecondaryEnchantmentCount();
    bool CanAddEnchantment(IBaseEnchantmentWrapper enchantment);
    void CopyEnchantments(IToolWrapper source, IToolWrapper destination);
    int GetTotalForgeLevels(bool for_unforge = false);
    bool AddEnchantment(IBaseEnchantmentWrapper enchantment);
    bool hasEnchantmentOfType<T>();
    void RemoveEnchantment(IBaseEnchantmentWrapper enchantment);
    bool CanUseOnStandingTile();
    bool CanForge(IItemWrapper item);
    T GetEnchantmentOfType<T>() where T : IBaseEnchantmentWrapper;
    int GetEnchantmentLevel<T>() where T : IBaseEnchantmentWrapper;
    bool Forge(IItemWrapper item, bool count_towards_stats = false);
  }
}