/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData;
using StardewValley.GameData.Machines;
using StardewValley.Menus;

namespace SprinklerAttachments
{
    public class ModEntry : Mod
    {
        public static readonly HashSet<string> MachineExclusions = new()
        {
            "(BC)BaitMaker",
        };

        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data/Machines"))
            {
                e.Edit(EditDataMachines, AssetEditPriority.Late);
            }
        }

        private void EditDataMachines(IAssetData asset)
        {
            // QuantityModifier decreaseQuality = new()
            // {
            //     Condition = "ITEM_QUALITY Input 1 4",
            //     Modification = QuantityModifier.ModificationType.Subtract,
            //     Amount = 1
            // };

            IDictionary<string, MachineData> data = asset.AsDictionary<string, MachineData>().Data;
            foreach (KeyValuePair<string, MachineData> kv in data)
            {
                string qItemId = kv.Key;
                MachineData machine = kv.Value;
                if (machine.IsIncubator || machine.OutputRules == null || !machine.AllowFairyDust)
                    continue;

                foreach (MachineOutputRule rule in machine.OutputRules)
                {
                    if (rule.OutputItem == null)
                        continue;
                    if (rule.Triggers.Any((trig) => trig.Trigger != MachineOutputTrigger.ItemPlacedInMachine))
                        continue;
                    rule.OutputItem.ForEach(item =>
                    {
                        switch (qItemId)
                        {
                            case "(BC)BaitMaker":
                                if (item is null || item.OutputMethod != null)
                                    return;
                                item.StackModifiers ??= new List<QuantityModifier>();
                                item.StackModifiers.Add(new()
                                {
                                    Condition = "ITEM_QUALITY Input 1 1",
                                    Modification = QuantityModifier.ModificationType.Add,
                                    Amount = 1
                                });
                                item.StackModifiers.Add(new()
                                {
                                    Condition = "ITEM_QUALITY Input 2 2",
                                    Modification = QuantityModifier.ModificationType.Add,
                                    Amount = 2
                                });
                                item.StackModifiers.Add(new()
                                {
                                    Condition = "ITEM_QUALITY Input 4 4",
                                    Modification = QuantityModifier.ModificationType.Add,
                                    Amount = 3
                                });
                                break;
                            case "(BC)25":
                                item.StackModifiers ??= new List<QuantityModifier>();
                                item.StackModifiers.Add(new()
                                {
                                    Condition = "ITEM_QUALITY Input 2 2",
                                    Modification = QuantityModifier.ModificationType.Add,
                                    Amount = 1
                                });
                                item.StackModifiers.Add(new()
                                {
                                    Condition = "ITEM_QUALITY Input 4 4",
                                    Modification = QuantityModifier.ModificationType.Add,
                                    Amount = 2
                                });
                                break;
                            default:
                                if (item is null || item.OutputMethod != null)
                                    return;
                                if (item.Quality == 2)
                                { // special case large milk/egg, copy quality, but produce 2
                                    item.StackModifiers ??= new List<QuantityModifier>();
                                    item.StackModifiers.Add(new()
                                    {
                                        Modification = QuantityModifier.ModificationType.Add,
                                        Amount = 1
                                    });
                                }
                                else
                                    item.CopyQuality = true;
                                break;
                        }
                    });
                }
            }
        }
    }
}
