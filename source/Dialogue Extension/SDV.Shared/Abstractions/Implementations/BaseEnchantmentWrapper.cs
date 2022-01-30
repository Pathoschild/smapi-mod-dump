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
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class BaseEnchantmentWrapper : IBaseEnchantmentWrapper
  {
    public BaseEnchantmentWrapper(BaseEnchantment item) => GetBaseType = item;
    public BaseEnchantment GetBaseType { get; }
    public int Level { get; set; }
    public NetFields NetFields { get; }
    public bool IsForge() => false;

    public bool IsSecondaryEnchantment() => false;

    public void InitializeNetFields()
    {
    }

    public void OnEquip(IFarmerWrapper farmer)
    {
    }

    public void OnUnequip(IFarmerWrapper farmer)
    {
    }

    public void OnCalculateDamage(IMonsterWrapper monster, IGameLocationWrapper location, IFarmerWrapper who, ref int amount)
    {
    }

    public void OnDealDamage(IMonsterWrapper monster, IGameLocationWrapper location, IFarmerWrapper who, ref int amount)
    {
    }

    public void OnMonsterSlay(IMonsterWrapper m, IGameLocationWrapper location, IFarmerWrapper who)
    {
    }

    public void OnCutWeed(Vector2 tile_location, IGameLocationWrapper location, IFarmerWrapper who)
    {
    }

    public IBaseEnchantmentWrapper GetOne() => null;

    public int GetLevel() => 0;

    public void SetLevel(IItemWrapper item, int new_level)
    {
    }

    public int GetMaximumLevel() => 0;

    public void ApplyTo(IItemWrapper item, IFarmerWrapper farmer = null)
    {
    }

    public bool IsItemCurrentlyEquipped(IItemWrapper item, IFarmerWrapper farmer) => false;

    public void UnapplyTo(IItemWrapper item, IFarmerWrapper farmer = null)
    {
    }

    public bool CanApplyTo(IItemWrapper item) => false;

    public string GetDisplayName() => null;

    public string GetName() => null;

    public bool ShouldBeDisplayed() => false;
  }
}
