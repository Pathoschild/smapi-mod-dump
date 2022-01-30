/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class OrderObjectiveWrapper : IOrderObjectiveWrapper
  {
    public OrderObjectiveWrapper(OrderObjective item) => GetBaseType = item;
    public OrderObjective GetBaseType { get; }
    public NetFields NetFields { get; }

    public void OnFail()
    {
    }

    public void InitializeNetFields()
    {
    }

    public void Register(ISpecialOrderWrapper new_order)
    {
    }

    public void Unregister()
    {
    }

    public bool ShouldShowProgress() => false;

    public int GetCount() => 0;

    public void IncrementCount(int amount)
    {
    }

    public void SetCount(int new_count)
    {
    }

    public int GetMaxCount() => 0;

    public void OnCompletion()
    {
    }

    public void CheckCompletion(bool play_sound = true)
    {
    }

    public bool IsComplete() => false;

    public bool CanUncomplete() => false;

    public bool CanComplete() => false;

    public string GetDescription() => null;

    public void Load(ISpecialOrderWrapper order, IDictionary<string, string> data)
    {
    }
  }
}