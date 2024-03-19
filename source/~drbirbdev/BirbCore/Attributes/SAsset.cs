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
    private MemberInfo? _modAssets;

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (!mod.GetType().TryGetMemberOfType(type, out MemberInfo memberInfo))
        {
            Log.Error("Mod must define an asset property");
            return;
        }

        this._modAssets = memberInfo;

        Action<object?, object?> setter = this._modAssets.GetSetter();

        setter(mod, instance);
        base.Handle(type, instance, mod, args);
    }

    /// <summary>
    /// A single asset. This property is synced with what is in the content pipeline, and can be used directly.
    /// This asset can be overriden by other mods, and those changes will be reflected in this property.
    /// The path of the asset will be "Mods/&lt;ModUniqueID&gt;/&lt;Property&gt;", for instance the following property
    /// <code>
    ///    [Asset(Path="assets/my_texture.png")]
    ///    public static Texture2D MyTexture;
    ///    public static string MyTextureAssetName;
    /// </code>
    /// could be located at "Mods/drbirbdev.BirbCore/MyTexture" in the content pipeline. Other mods could then
    /// load this texture to use it, and this mod can just use the MyTexture property directly.
    /// An optional string property sharing the same name, but ending with "AssetName" can also be included.
    /// This property will be set to the "Mods/&lt;ModUniqueID&gt;/&lt;Property&gt;" value, which is required for some methods.
    /// </summary>
    public class Asset(string path, AssetLoadPriority priority = AssetLoadPriority.Medium) : FieldHandler
    {
        private readonly string _path = PathUtilities.NormalizePath(path);

        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter,
            Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            IAssetName assetName = mod.Helper.ModContent.GetInternalAssetName(this._path);

            if (instance is null)
            {
                if (fieldType.DeclaringType != null &&
                    fieldType.DeclaringType.TryGetSetterOfName(name + "AssetName", out Action<object?, object?> assetNameSetter))
                {
                    assetNameSetter(instance, assetName);
                }
            }
            else
            {
                if (instance.GetType().TryGetSetterOfName(name + "AssetName", out Action<object?, object?> assetNameSetter))
                {
                    assetNameSetter(instance, assetName);
                }
            }


            mod.Helper.Events.Content.AssetRequested += (sender, e) =>
            {
                if (!e.Name.IsEquivalentTo(assetName))
                {
                    return;
                }

                //TODO: see if more specific generics are needed
                //TODO: set onBehalfOf if SMAPI would allow it
                // e.LoadFrom(() => mod.Helper.ModContent.Load<object>(this._path), priority);

                e.GetType().GetMethod("LoadFromModFile")
                    ?.MakeGenericMethod(fieldType)
                    .Invoke(e, [this._path, priority]);

                setter(instance, LoadValue(fieldType, this._path, mod));
            };

            mod.Helper.Events.Content.AssetReady += (sender, e) =>
            {
                if (!e.Name.IsEquivalentTo(assetName))
                {
                    return;
                }

                setter(instance, LoadValue(fieldType, this._path, mod));
            };

            mod.Helper.Events.Content.AssetsInvalidated += (sender, e) =>
            {
                foreach (IAssetName asset in e.Names)
                {
                    if (asset.IsEquivalentTo(assetName))
                    {
                        setter(instance, LoadValue(fieldType, this._path, mod));
                    }
                }
            };

            setter(instance, LoadValue(fieldType, this._path, mod));
        }

        private static object? LoadValue(Type fieldType, string assetPath, IMod mod)
        {
            return mod.Helper.ModContent.GetType().GetMethod("Load", [typeof(string)])
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.ModContent, [assetPath]);
        }
    }
}
