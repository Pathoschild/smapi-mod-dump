**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/drbirbdev/StardewValley**

----

# BirbCore

My core mod.  Mostly contains an attribute library I use for cutting boilerplate out of my mods.

# BirbCore.Attributes

To use Attributes, first make sure your Mod is parsing them in ModEntry by calling `Parser.ParseAll` and passing the Mod instance (usually `this`)

```csharp
global using BirbCore.Attributes; // all examples include BirbCore.Attributes

namespace MyMod;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);
    }
}
```

Now you can annotate classes, and their fields, properties, methods to different affect.

This also parses Harmony attributes, so that doesn't have to be done separately in ModEntry.

This also initializes the BirbCore Log class, which has logging convenience methods like `Log.Info("some text")`.
This can make logging a bit more terse.

## SAsset

```csharp
using StardewModdingApi;

namespace MyMod;

[SAsset]
public class MyAssets
{
    [SAsset.Asset("path/to/my/file.png")]
    public static IRawTextureData MyTexture;
    public static IAssetName MyTextureAssetName;
}
```

SAsset loads your asset file into a field or property, and makes sure that property is synced with the content pipeline.

Your asset can be patched by other mods using content patcher, and the property or field will always be up-to-date.

Asset can be static, or not.  If non-static, an instance will be created of your class, and a field or property in your Mod class will be expected to hold that instance.

## SCommand

```csharp
namespace MyMod;

[SCommand("my_command")]
class MyCommands
{
    [SCommand.Command("My first command")]
    public static void First(string arg1, int arg2, int arg3 = 0)
    {
        Log.Info($"{arg1}, {arg2}, {arg3}");
    }

    [SCommand.Command("My second command")]
    public static void SecondCommand(string arg1, params float[] arg2)
    {
        Log.Info($"{arg1}");
        foreach (float arg in arg2) {
            Log.Info($"{arg}");
        }
    }
}
```

SCommand creates SMAPI commands from your various class methods.  It creates help documentation both from the given text, e.g. `"My first command"`, and from the method signature.

For example, the above two commands could be called like so:
```
my_command first hello 1
my_command first hello 1 2
my_command second_command hello
my_command second_command hello 1.0 2.0 3.0 4.0
```

Notice that the sub-command is just the method name converted to snake_case, to match standard command syntax.

The help documentation might look something like this:
```
my_command first <arg1> <arg2> [arg3]
    My first command

my_command second_command <arg1> <arg2...>
    My second command
```
Method signature can include strings, ints, floats, doubles, or bools.  Arguments can be optional, or parametric.

## SConfig

```csharp
namespace MyMod;

[SConfig]
class MyConfigs
{
    [SConfig.SectionTitle("Section")]
    [SConfig.PageLink("Page1")]
    [SConfig.Paragraph("Paragraph")]
    // provide an asset path to display an image
    [SConfig.Image("Mods/mytexture/Image")]

    [SConfig.StartTitleOnlyBlock]
    // stuff here only shows up on the title screen
    [SConfig.EndTitleOnlyBlock]

    [SConfig.PageBlock("Page1")]
    // stuff below here shows up on Page1, until a new PageBlock begins

    [SConfig.Option]
    public string OptionWithTextBox = "hello";

    [SConfig.Option(0, 100)]
    public int OptionWithSlider = 50;

    [SConfig.Option(new string[] { "string1", "string2", "string3" })]
    public string OptionWithDropdown = "string1";
}
```
SConfig creates integrations with Generic Mod Config Menu.

If GMCM isn't installed for a client, then it still sets an instance of a Config class on the Mod class, however it's full strength is for supporting GMCM.

You can include Section Titles, Paragraphs, Pagination, Title Screen only options, checkboxes, text forms, sliders, and drop-downs without interacting directly with the GMCM API.

Make sure you use a default.json i18n file to include human-readable names and tooltips.
```json
{
    "config.Section": "Section Name",
    "config.Section.tooltip": "The name of the first section"
    ...
}
```
Prefix the Attribute string argument, or the field or property name, with "config." for the base translation, and suffix it with ".tooltip" for the tooltip.

Currently there is no default values, so i18n for names and tooltips is required.

## SContent

```csharp
namespace MyMod;

[SContent("content.json")]
public class MyContent
{
    public string Field1;
    public string Field2;
}
```
SContent handles getting owned content packs for you.  In your Mod class, include an instance like so:
```
    public Dictionary<string, MyContent> Content;
```
SContent will populate that dictionary with keys containing ModIDs, and values containing content files.

You also have the option of making values be lists or dictionaries.

```
    public Dictionary<string, List<MyContent>> Content;
    // OR
    public Dictionary<string, Dictionary<string, MyContent>> Content;
```
You'll need to use the SContent parameters IsList or IsDictionary though.

```
[SContent("content.json", IsList = true)]
public class Content

[SContent("content.json", IsDictionary = true)]
public class Content
```

SContent can include some non-serialized helper fields which get set automatically.
Make sure they aren't serialized with `[JsonIgnore]`

```csharp
    // A unique id for this content. Built from ModId and ContentId.
    [JsonIgnore]
    [SContent.UniqueId]
    public string UniqueID;

    // Mod Id of the provider of this content.
    [JsonIgnore]
    [SContent.ModId]
    public string ModID;

    // A unique id for this content within the providing mod.
    // If content is a dictionary, this will be the dictionary key.
    // If content is a list, this will be the list index.
    // Otherwise, this will be blank.
    [JsonIgnore]
    [SContent.ContentId]
    public string ContentId;

    // A reference to the IContentPack of this content.
    // Can be useful for getting additional files, i18n, or manifest data.
    [JsonIgnore]
    [SContent.ContentPack]
    public IContentPack ContentPack;
```

## SData

TODO: I haven't used this much personally, so I'm not documenting it yet.

## SDelegate

```csharp
namespace MyMod;

[SDelegate]
public class MyDelegates
{
    [SDelegate.TileAction]
    public static bool MyTileAction(GameLocation location, string[] args, Farmer farmer, Point point)
    {
        Log.Info($"{location} {args}");
    }
}
```
SDelegate just wraps different extendable game delegates.

The expectation when adding to these delegates is to use your mods Unique ID as a prefix. SDelegate does this for you.
It uses method name as a suffix, and handles converting both static and instance methods.

## SEvent
```csharp
namespace MyMod;

[SEvent]
public class MyEvents
{
    [SEvent.UpdateTicking]
    public static void MyUpdateTickingEvent(object sender, UpdateTickingEventArgs e) {
        Log.Info($"{e}");
    }
}
```
SEvent wraps different SMAPI events, similarly to SDelegate.

It includes the special attribute `SEvent.GameLaunchedLate` which is the same as GameLaunched, but happens after all attributes have been parsed by default.
This can be useful for any mod logic that should run on game launch, but requires assets, configs, content, or other data loaded by attributes.

Note: If you are customizing attribute priority, GameLaunchedLate is not guaranteed to run after all other annotations are parsed.

## SMod

```csharp
namespace MyMod;

[SMod]
public class ModEntry : Mod
{
    [SMod.Instance]
    internal static ModEntry MyInstance;

    [SMod.Api("spacechase0.SpaceCore")]
    internal static ISpaceCoreApi SpaceCore;

    [SMod.Api("spacechase0.GenericModConfigMenu", IsRequired = false)]
    internal static IGenericModConfigMenuApi GenericModConfigMenu;

    internal MyAssets MyAssets;
    internal MyConfig MyConfig;
    internal Dictionary<string, MyContent> MyContent;

    public overrid void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);
    }
}
```
SMod handles setting a ModEntry instance field, as well as setting any API interface fields.
APIs can be marked as optional (IsRequired = false) and will be set to null if the client does not have the API installed.

Fields that are set by other attributes, such as SAsset and SConfig don't require an attribute.

## SToken
```csharp
namespace MyMod;

[SToken]
public class MyTokens
{
    [SToken.Token]
    public static IEnumerable<string> MyToken()
    {
        yield return "string";
    }
}
```
SToken handles creating content patcher tokens.  Token methods should return `IEnumerable<string>`, and will be registered with Content Patcher automatically using the mod unique id and method name.

You can also create advanced tokens:

```csharp
[SToken.AdvancedToken]
public class MyAdvancedToken
{
    public bool AllowsInput() { return false; }

    public bool CanHaveMultipleValues(string input = null) { return false; }

    public bool UpdateContext() { return false; }

    public bool IsReady() { return false; }

    public IEnumerable<string> GetValues(string input) { return null; }
}
```
Advanced tokens must match the expected shape for content patcher, and should respect the restrictions of that API.

Since advanced tokens are their own class, then can exist in a stand-alone file.

# BirbCore.APIs

This includes some API interfaces I use often, so I don't have to copy them to each mod.  Nothing more, nothing less.

# BirbCore.Extensions

This includes some extension methods. An incomplete list

* String case conversion (ToSnakeCase, ToPascalCase)
* Reflection convenience methods to handle fields or properties or create delegates
* Method to get section of IRawTextureData as a Texture2D, or to get a color by texture coordinate


