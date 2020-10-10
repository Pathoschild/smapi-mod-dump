# Contributing

You're welcome to do pull requests. :)

But bear in mind that these are my first ever C# projects, and as such I only have a basic understanding of its various features and quirks.
So I would appreciate if you could add some details how your code works and what it does, so I can better understand what you did. Thanks!

# Building

## Using .NET Core

Open the terminal in the project folder and type:
- `cd ModName` - Select the mod to build
- `dotnet restore` - Install dependencies
- `msbuild` - Build the mod

## MonoDevelop

Apparently this setup is not compatible with MonoDevelop out-of-the-box. You could do the following:
- Create a new project in MonoDevelop
- Copy-paste the `manifest.json` and `src` folder into the project
- Add NuGet packages: `Microsoft.Net.Compilers v3.3.1`, `Pathoschild.Stardew.ModBuildConfig v3.2.2`

# VSCode

Optional. Just in case you use it.

- Get the [ms-dotnettools.csharp](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension
- When using [Mono](https://www.mono-project.com/), add `"omnisharp.useGlobalMono": "always"` to your `settings.json`

# Troubleshooting

Some things I encountered when setting up the project.

## Error: "Can't import project"

Roslyn is in another castle. Just create a symlink:
```
/usr/lib/mono/msbuild/15.0/bin $ sudo ln -s /usr/lib/mono/msbuild/Current/bin/Roslyn/ Roslyn
```

Source: https://forum.manjaro.org/t/cannot-compile-with-msbuild-because-files-are-installed-in-wrong-location/107390

## Error: "Can't find game path"

Create file `stardewvalley.targets` in your home directory:
```
<Project>
   <PropertyGroup>
     <GamePath>/full/path/to/Stardew Valley/game/</GamePath>
   </PropertyGroup>
</Project>
```

Sources:
- https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#configure
- https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#custom-game-path

## Error: "The reference assemblies for .NETFramework,Version=v4.5 were not found."

Do you use VSCode with Mono? See the VSCode section above.

Source: https://github.com/OmniSharp/omnisharp-vscode/issues/4007#issuecomment-677405332

# Solved errors

Fixes for these are stored in the repo, so no need to worry about them. I hope.
But for reference, here's what happened.

## Error: "Method 'System.String.GetPathsOfAllDirectoriesAbove' not found. (MSB4184) (ModName)"

NuGet -> Microsoft.Net.Compilers -> 3.3.1

```
dotnet add package Microsoft.Net.Compilers -v 3.3.1
```

Source: https://stackoverflow.com/a/61585773

## Error: 'There was a mismatch between the processor architecture of the project being built "MSIL" and the processor architecture of the reference "StardewValley", "x86".'

Add the following to the `ModName.csproj` file:

```
<PropertyGroup>
	<Platform>x86</Platform>
	...
</PropertyGroup>
```

# CLI usage

Mainly as notes to myself.

- New solution: `dotnet new sln`
- New project: `dotnet new classLib --name ModName`
- Add project to solution: `dotnet sln add ModName/ModName.csproj`
- Add package via NuGet: `dotnet add package Microsoft.Net.Compilers -v 3.3.1`
- Install packages in an existing repo: `dotnet restore`
- Build a specific target: `msbuild /property:Configuration=Release`

Sources:
- https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new
- https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build#examples
- https://stackoverflow.com/a/42730946