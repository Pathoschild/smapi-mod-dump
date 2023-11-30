/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

#nullable enable
using System;
using System.Reflection;
using BirbCore.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace BirbCore.Attributes;

/// <summary>
/// A collection of assets that go through the content pipeline.
/// </summary>
public class SAsset : ClassHandler
{
    private MemberInfo? ModAssets;

    public SAsset() : base(0)
    {
    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        this.ModAssets = mod.GetType().GetMemberOfType(type);
        if (this.ModAssets == null)
        {
            Log.Error("Mod must define an asset property");
            return;
        }

        instance = Activator.CreateInstance(type);
        this.ModAssets.GetSetter()(mod, instance);
        base.Handle(type, instance, mod, args);
        return;
    }

    /// <summary>
    /// A single asset. This property is synced with what is in the content pipeline, and can be used directly.
    /// This asset can be overriden by other mods, and those changes will be reflected in this property.
    /// The path of the asset will be "Mods/<ModUniqueID>/<Property>", for instance the following property
    /// <code>
    ///    [Asset(Path="assets/my_texture.png")]
    ///    public static Texture2D MyTexture;
    ///    public static string MyTextureAssetName;
    /// </code>
    /// could be located at "Mods/drbirbdev.BirbCore/MyTexture" in the content pipeline. Other mods could then
    /// load this texture to use it, and this mod can just use the MyTexture property directly.
    /// An optional string property sharing the same name, but ending with "AssetName" can also be included.
    /// This property will be set to the "Mods/<ModUniqueID>/<Property>" value, which is required for some methods.
    /// </summary>
    public class Asset : FieldHandler
    {
        public string Path;
        public AssetLoadPriority Priority;

        public Asset(string path, AssetLoadPriority priority = AssetLoadPriority.Medium)
        {
            this.Path = PathUtilities.NormalizePath(path);
            this.Priority = priority;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {

            IAssetName assetName = mod.Helper.ModContent.GetInternalAssetName(this.Path);

            Action<object, object>? assetNameSetter = instance?.GetType().GetMemberOfName(name + "AssetName")?.GetSetter();
            if (assetNameSetter is not null && instance is not null)
            {
                assetNameSetter(instance, assetName);
            }

            mod.Helper.Events.Content.AssetRequested += (object? sender, AssetRequestedEventArgs e) =>
            {
                if (!e.Name.IsEquivalentTo(assetName))
                {
                    return;
                }

                object? value = e?.GetType().GetMethod("LoadFromModFile")
                    ?.MakeGenericMethod(fieldType)
                    .Invoke(e, new object[] { this.Path, this.Priority });
                setter(instance, value);
            };

            mod.Helper.Events.Content.AssetReady += (object? sender, AssetReadyEventArgs e) =>
            {
                if (!e.Name.IsEquivalentTo(assetName))
                {
                    return;
                }

                setter(instance, LoadValue(fieldType, this.Path, mod));
            };

            mod.Helper.Events.Content.AssetsInvalidated += (object? sender, AssetsInvalidatedEventArgs e) =>
            {
                foreach (IAssetName asset in e.Names)
                {
                    if (asset.IsEquivalentTo(assetName))
                    {
                        setter(instance, LoadValue(fieldType, this.Path, mod));
                    }
                }
            };

            setter(instance, LoadValue(fieldType, this.Path, mod));
        }

        private static object? LoadValue(Type fieldType, string assetPath, IMod mod)
        {
            return mod.Helper.ModContent.GetType().GetMethod("Load", new[] { typeof(string) })
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.ModContent, new string[] { assetPath });
        }

    }
}
