/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace SonoCore;

/// <summary>Represents the base of a mod that depends on SonoCore.</summary>
public abstract class ModBase : Mod
{
    /*********
    ** Public Methods
    *********/
    /// <inheritdoc/>
    public sealed override void Entry(IModHelper helper)
    {
        Entry();
        LoadHarmonyPatches();
    }

    /// <summary>Loads the content packs.</summary>
    /// <remarks>This needs to be called manually, this is because not all mods can have content packs loaded in the same place (FAVR for example.)</remarks>
    public void LoadContentPacks()
    {
        this.Monitor.Log("Loading content packs", LogLevel.Info);

        InitialiseContentPackLoading();

        foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            try
            {
                this.Monitor.Log($"Loading {contentPack.Manifest.Name}", LogLevel.Info);
                LoadContentPack(contentPack);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Unhandled exception occurred when loading content pack: {contentPack.Manifest.Name}\n{ex}", LogLevel.Error);
            }

        FinaliseContentPackLoading();
    }


    /*********
    ** Protected Methods
    *********/
    /// <summary>The mod entry point.</summary>
    protected abstract void Entry();

    /// <summary>Initialises content pack loading.</summary>
    /// <remarks>This is called before any content packs have been loaded through <see cref="LoadContentPack(IContentPack)"/>, this should be used to reset lists used for updating repositories etc.</remarks>
    protected virtual void InitialiseContentPackLoading() { }

    /// <summary>Loads a content pack.</summary>
    /// <param name="contentPack">The content pack to load.</param>
    protected virtual void LoadContentPack(IContentPack contentPack) { }

    /// <summary>Finalises content pack loading.</summary>
    /// <remarks>This is called after all content packs have been loaded through <see cref="LoadContentPack(IContentPack)"/>, this should be used to update repositories etc.</remarks>
    protected virtual void FinaliseContentPackLoading() { }


    /*********
    ** Private Methods
    *********/
    /// <summary>Loads the harmony patches.</summary>
    private void LoadHarmonyPatches()
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);

        foreach (var method in this.GetType().Assembly.GetTypes().SelectMany(type => type.GetTypeInfo().DeclaredMethods))
        {
            var patchAttributes = method.GetCustomAttributes<PatchAttribute>();
            if (!patchAttributes.Any())
                continue;

            if (!method.IsStatic)
            {
                this.Monitor.Log($"Patch '{method.GetFullName()}' isn't static", LogLevel.Error);
                continue;
            }

            foreach (var patchAttribute in patchAttributes)
                switch (patchAttribute.PatchType)
                {
                    case PatchType.Prefix: harmony.Patch(patchAttribute.OriginalMethod, prefix: new HarmonyMethod(method)); break;
                    case PatchType.Transpiler: harmony.Patch(patchAttribute.OriginalMethod, transpiler: new HarmonyMethod(method)); break;
                    case PatchType.Postfix: harmony.Patch(patchAttribute.OriginalMethod, postfix: new HarmonyMethod(method)); break;
                    default: this.Monitor.Log($"Patch '{method.GetFullName()}' has an invalid patch type ({patchAttribute.PatchType})", LogLevel.Error); break;
                }
        }
    }
}
