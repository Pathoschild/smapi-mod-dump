/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using KitchenLib.src.ContentPack.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KitchenLib.src.ContentPack
{
    public class ContentPackUtils
    {
        internal static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Converters = new JsonConverter[]
            {
                new StringEnumConverter(),
                new SemVersionConverter(),
                new AssetBundleConverter()
            }
        };
        internal static void Log(string message)
        {
            BaseMod.instance.Log(message);
        }

        internal static void Error(string message)
        {
            BaseMod.instance.Error(message);
        }
    }

    public enum SerializationContext
    {
        ModManifest,
        ModContent,
        Appliance,
        CompositeUnlockPack,
        CrateSet,
        Decor,
        Dish,
        Effect,
        EffectRepresentation,
        GameDifficultySettings,
        GardenProfile,
        Item,
        ItemGroup,
        LevelUpgradeSet,
        ModularUnlockPack
    }
}
