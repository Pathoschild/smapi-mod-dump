**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TehPers/StardewValleyMods**

----

# Teh's Fishing Overhaul - API

There are two separate APIs exposed by Teh's Fishing Overhaul:

- **Simplified API:** Any mod can access this API through SMAPI's mod registry.
- **Full API:** This API is accessible through TehCore (even as an optional dependency).

## Simplified API

The simplified API is exposed through SMAPI's mod registry. This is the standard way of accessing APIs through SMAPI. To access it, first copy the [`ISimplifiedFishingApi`][simplified interface] interface to your C# project. Afterwards, add a dependency (or optional dependency) to `TehPers.FishingOverhaul` to your mod's `manifest.json`. Finally, you can access the API through the mod registry:

```cs
var fishingApi = helper.ModRegistry.GetApi<ISimplifiedFishingApi>("TehPers.FishingOverhaul");
```

Each of the methods on the simplified API should be documented, so make sure to read the docs to see how to use them. There are several methods which either accept or return stringified representations of `NamespacedKey`. Those values follow the format `"<namespace>:<key>"`. For example, `"StardewValley:Object/832"` is the stringified key for pineapples. The [content pack docs] have more details on how these strings are formatted.

## Full API

> **Note:** This section is still a work-in-progress.

Unfortunately, due to the restrictive nature of SMAPI's built in mod API support, it isn't possible to expose the full API through the mod registry. Instead, the full API is accessible through TehCore. Accessing it is fairly straightforward, you just need to request the type from something called your mod kernel either directly or through dependency injection. For more details on what "kernels" and "dependency injection" are, visit the [Ninject docs][ninject docs]. However, it isn't necessary to know either of those things to access the fishing API.

To reference the Fishing API from your mod, add the following to your project's `.csproj` file:

```xml
<PropertyGroup>
    <!-- ... -->

    <!-- Add this to the property group below everything else -->
    <BundleExtraAssemblies>ThirdParty</BundleExtraAssemblies>
    <!-- Remove unnecessary output files -->
    <IgnoreModFilePatterns>^TehPers\.Core\.Api\.(dll|pdb|xml)$, ^TehPers\.FishingOverhaul\.Api\.(dll|pdb|xml)$, ^Ninject\.dll$</IgnoreModFilePatterns>
</PropertyGroup>

<ItemGroup>
    <!-- These should point to the different APIs in your Mods folder - change these if needed -->
    <Reference Include="$(GameModsPath)/TehPers.Core/TehPers.Core.Api.dll" />
    <Reference Include="$(GameModsPath)/TehPers.FishingOverhaul/TehPers.FishingOverhaul.Api.dll" />
</ItemGroup>
```

If you compile your mod, you should not see any new files added to your mod's output directory. However, you should be able to access the API now.

**Make sure to add `TehPers.FishingOverhaul` and `TehPers.Core` as dependencies to your mod's manifest!** If your mod does not need them to function, then add them as optional dependencies so that they are loaded first.

### Requesting the API instance

The different API types can be pulled through a mod kernel. For more information, check out the relevant documentation on the TehCore API. In short, create your mod kernel and request the types that you need:

```cs
// YourMod.cs (class that extends StardewValleyAPI.Mod)

// Request the mod kernel
var kernel = ModServices.Factory.GetKernel(this);

// Inject any content sources you want. Make sure to inject any dependencies they have as well.
// Several types are automatically injected for you, including IModHelper and IManifest.
// Note that you must use 'GlobalProxyRoot' to expose your service to Teh's Fishing Overhaul.
kernel.GlobalProxyRoot
    .Bind<IFishingContentSource>()
    .To<YourContentSource>() // or .ToMethod, or .ToConstant, whatever works for you
    .InSingletonScope(); // any scope works fine

// Request the fishing API
var fishingApi = kernel.Get<IFishingApi>();
```

### `IFishingApi`

This interface exposes the primary fishing API. This interface primarily acts as a source of truth for how fishing should behave. New content cannot be added through this interface, however it can be used to see how fishing behaves with the content that is already loaded. For example, a separate fishing HUD mod could use this interface to see what fish can be caught by a farmer, what the chances of catching those fish are, and which of those fish are legendary. Similarly, treasure and trash can be retrieved from this interface.

Make sure to read the documentation for how to use the interface. To actually create instances of the items from their `NamespacedKey`s, look at `INamespaceRegistry` from the TehCore API.

TODO: ^ these docs don't actually exist yet...

TODO: maybe link to the core mod docs?

### `IFishingContentSource`

This interface allows custom fishing content to be added to the game. Fishing content includes:

- Adding new fish traits to allow items to be treated as fish
- Adding new fish/trash/treasure entries to make items catchable as fish/trash/treasure

The interface is fairly straightforward, so make sure to read the documentation for it. Whenever fishing content should be reloaded, make sure to invoke `IFishingApi.RequestReload()`.

[simplified interface]: /TehPers.FishingOverhaul.Api/ISimplifiedFishingApi.cs
[content pack docs]: /docs/TehPers.FishingOverhaul/Content%20Packs.md
[ninject docs]: https://github.com/ninject/ninject/wiki
