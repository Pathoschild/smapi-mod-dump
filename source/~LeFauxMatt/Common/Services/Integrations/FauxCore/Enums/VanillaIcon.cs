/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
#else
namespace StardewMods.Common.Services.Integrations.FauxCore;
#endif

using NetEscapades.EnumGenerators;

/// <summary>Icons added to IconRegistry from vanilla.</summary>
[EnumExtensions]
public enum VanillaIcon
{
    /// <summary>An arrow pointing down.</summary>
    ArrowDown,

    /// <summary>An arrow pointing left.</summary>
    ArrowLeft,

    /// <summary>An arrow pointing right.</summary>
    ArrowRight,

    /// <summary>An arrow pointing up.</summary>
    ArrowUp,

    /// <summary>A backpack.</summary>
    Backpack,

    /// <summary>A cancel button.</summary>
    Cancel,

    /// <summary>A checked checkbox.</summary>
    Checked,

    /// <summary>A chest.</summary>
    Chest,

    /// <summary>A golden coin.</summary>
    Coin,

    /// <summary>The color picker toggle button.</summary>
    ColorPicker,

    /// <summary>A red circle with a slash.</summary>
    DoNot,

    /// <summary>A dropdown arrow.</summary>
    Dropdown,

    /// <summary>An empty heart.</summary>
    EmptyHeart,

    /// <summary>A fish.</summary>
    Fish,

    /// <summary>A fishing reward chest.</summary>
    FishingChest,

    /// <summary>A wrapped gift.</summary>
    Gift,

    /// <summary>A filled heart.</summary>
    Heart,

    /// <summary>An ok button.</summary>
    Ok,

    /// <summary>An organize button.</summary>
    Organize,

    /// <summary>A plus.</summary>
    Plus,

    /// <summary>A gold star.</summary>
    QualityGold,

    /// <summary>An iridium star.</summary>
    QualityIridium,

    /// <summary>A silver star.</summary>
    QualitySilver,

    /// <summary>A shield.</summary>
    Shield,

    /// <summary>A skull.</summary>
    Skull,

    /// <summary>A sword.</summary>
    Sword,

    /// <summary>A pickaxe.</summary>
    Tool,

    /// <summary>A garbage can.</summary>
    Trash,

    /// <summary>An unchecked checkbox.</summary>
    Unchecked,

    /// <summary>A vegetable.</summary>
    Vegetable,
}