/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Extensions;
using AchtuurCore.Framework.Borders;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;

enum NodeType
{
    NormalNode,
    GemNode,
    MysticNode,
    BoneNode,
}

internal class NodeLabel : ObjectLabel
{
    /// <summary>
    /// ParentsheetIndex of Jade, only gem that does not have a constant in SObject class
    /// </summary>
    const int JadeIndex = 70;

    static Dictionary<int, int> NodeDroppedItemIndex = new()
    {
        { 751, SObject.copper },
        { 849, SObject.copper }, // volcano copper ore
        { 290, SObject.iron },
        { 764, SObject.gold },
        { 765, SObject.iridium },
        { 843, 848 }, // cinder shard
        { 95, 909 }, // radioactive ore
        { 2, SObject.diamondIndex },
        { 4, SObject.rubyIndex },
        { 6, JadeIndex }, // jade
        { 8, SObject.amethystClusterIndex},
        { 10, SObject.topazIndex },
        { 12, SObject.emeraldIndex },
        { 14, SObject.aquamarineIndex },
        { 75, 535 }, // normal geode
        { 76, 536 }, // frozen geode
        { 77, 537 }, // magma geode
        { 819, 749 }, // omni geode
        { 25, 719 }, // mussel node
        { 818, 330 }, // clay node

    };
    public NodeLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        Debug.DebugOnlyExecute(() =>
        {
            NodeDroppedItemIndex = new()
            {
                { 751, SObject.copper },
                { 290, SObject.iron },
                { 764, SObject.gold },
                { 765, SObject.iridium },
                { 849, SObject.copper }, // volcano copper ore
                { 850, SObject.iron }, //volcano iron ore
                { 843, 848 }, // cinder shard
                { 95, 909 }, // radioactive ore
                { 2, SObject.diamondIndex },
                { 4, SObject.rubyIndex },
                { 6, JadeIndex }, // jade
                { 8, SObject.amethystClusterIndex},
                { 10, SObject.topazIndex },
                { 12, SObject.emeraldIndex },
                { 14, SObject.aquamarineIndex },
                { 75, 535 }, // normal geode
                { 76, 536 }, // frozen geode
                { 77, 537 }, // magma geode
                { 819, 749 }, // omni geode
                { 25, 719 }, // mussel node
                { 818, 330 }, // clay node

            };
        });


        SObject sobj = GetCursorObject(cursorTile);

        return sobj is not null
            && sobj.Name == "Stone"
            && GetNodeType(sobj.ParentSheetIndex) is not null;
    }

    public override void GenerateLabel()
    {
        NodeType? nodeType = GetNodeType(hoverObject.ParentSheetIndex);
        switch (nodeType)
        {
            case NodeType.NormalNode:
                GenerateNodeLabel();
                break;
            case NodeType.MysticNode:
                GenerateMysticNodeLabel();
                break;
            case NodeType.GemNode:
                GenerateGemNodeLabel();
                break;
            case NodeType.BoneNode:
                GenerateBoneNodeLabel();
                break;
            default:
                break;
        }

    }
    private void GenerateNodeLabel()
    {
        SObject nodeItem = GetNodeItem(hoverObject.ParentSheetIndex);
        AddBorder(new TitleLabel(I18n.LabelNodeName(nodeItem.DisplayName)));
    }

    private void GenerateMysticNodeLabel()
    {
        AddBorder(new TitleLabel(I18n.LabelNodeMysticName()));
        AddBorder(I18n.LabelDropItems());

        SObject iridiumOre = ModEntry.GetObjectWithId(SObject.iridium);
        SObject goldOre = ModEntry.GetObjectWithId(SObject.gold);
        SObject prismaticShard = ModEntry.GetObjectWithId(SObject.prismaticShardIndex);

        AppendLabelToBorder($"> {iridiumOre.DisplayName} (1-3)");
        AppendLabelToBorder($"> {goldOre.DisplayName} (1-4)");
        AppendLabelToBorder($"> {prismaticShard.DisplayName} (25% chance)");

    }

    private void GenerateGemNodeLabel()
    {
        AddBorder(new TitleLabel(I18n.LabelNodeGemName()));
        AddBorder("Drops one of the following gems:");

        // Use array of SObjects since then the displayname can be used
        // which is automatically translated
        SObject[] gemObjects = new[]
        {
            ModEntry.GetObjectWithId(SObject.amethystClusterIndex),
            ModEntry.GetObjectWithId(SObject.aquamarineIndex),
            ModEntry.GetObjectWithId(SObject.diamondIndex),
            ModEntry.GetObjectWithId(SObject.emeraldIndex),
            ModEntry.GetObjectWithId(JadeIndex),
            ModEntry.GetObjectWithId(SObject.rubyIndex),
            ModEntry.GetObjectWithId(SObject.topazIndex),
        };

        foreach (SObject gem in gemObjects)
        {
            AppendLabelToBorder($"> {gem.DisplayName}");
        }
    }

    private void GenerateBoneNodeLabel()
    {
        AddBorder(new TitleLabel(I18n.LabelNodeBoneName()));
    }

    private static NodeType? GetNodeType(int stoneIndex)
    {
        switch (stoneIndex)
        {
            case 46:
                return NodeType.MysticNode;
            case 44:
                return NodeType.GemNode;
            case 816:
            case 817:
                return NodeType.BoneNode;
        }

        if (NodeDroppedItemIndex.Keys.Contains(stoneIndex))
            return NodeType.NormalNode;

        return null;
    }

    private static SObject GetNodeItem(int stoneIndex)
    {
        return ModEntry.GetObjectWithId(NodeDroppedItemIndex[stoneIndex]);
    }
}
