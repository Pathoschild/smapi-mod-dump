/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Extensions;
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
    const int JadeIndex = 64;

    static Dictionary<int, int> NodeDroppedItemIndex = new()
    {
        { 751, SObject.copper },
        { 290, SObject.iron },
        { 764, SObject.gold },
        { 765, SObject.iridium },
        { 844, 848 }, // cinder shard
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
        { 819, 749}, // omni geode
        { 25, 719 }, // mussel node
        { 818, 330 }, // clay node

    };
    public NodeLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = GetCursorObject(cursorTile);

        return sobj is not null
            && GetNodeType(sobj.ParentSheetIndex) is not null;
    }

    public override void GenerateLabel()
    {
        NodeType? nodeType = GetNodeType(hoverObject.ParentSheetIndex);

        if (nodeType is null)
            return;

        //nodeType = nodeType.Value;

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
            case null:
                break;
        }

    }
    private void GenerateNodeLabel()
    {
        SObject nodeItem = GetNodeItem(hoverObject.ParentSheetIndex);
        Name = I18n.LabelNodeName(nodeItem.DisplayName);
    }

    private void GenerateMysticNodeLabel()
    {
        Name = I18n.LabelNodeMysticName();
        Description.Add("Drops the following items:");

        SObject iridiumOre = ModEntry.GetObjectWithId(SObject.iridium);
        SObject goldOre = ModEntry.GetObjectWithId(SObject.gold);
        SObject prismaticShard = ModEntry.GetObjectWithId(SObject.prismaticShardIndex);

        Description.Add($"> {iridiumOre.DisplayName} (1-3)");
        Description.Add($"> {goldOre.DisplayName} (1-4");
        Description.Add($"> {prismaticShard.DisplayName} (25% chance)");

    }

    private void GenerateGemNodeLabel()
    {
        Name = I18n.LabelNodeGemName();
        Description.Add("Drops one of the following gems:");

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
            Description.Add($"> {gem.DisplayName}");
        }
    }

    private void GenerateBoneNodeLabel()
    {
        Name = I18n.LabelNodeBoneName();
    }

    private static NodeType? GetNodeType(int stoneIndex)
    {
        switch (stoneIndex)
        {
            case 46:
                return NodeType.MysticNode;
            case 44:
                return NodeType.GemNode;
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
