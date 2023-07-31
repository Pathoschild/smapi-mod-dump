**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Achtuur/StardewMods**

----

# Author guide

this mod supports adding custom labels for just about everything you can hover your cursor over.
In this document, a tutorial will be given on how to add your own custom labels.


## Creating your own label

* Step 1: Copy the `IHoverLabel` interface from [IHoverLabel.cs](../IHoverLabel.cs) into your own code.
* Step 2: Copy the `IHoverLabelApi` interface from [IHoverLabelApi.cs](../IHoverLabelApi.cs) into your own code
* Step 3: create your own label that implements the interface, make sure to check out the [IHoverLabel section](#IHoverLabel-tutorial) for more details on how to do this.
* Step 4: create an instance of the api using the SMAPI mod registry helper: `IHoverLabelApi labelApi = this.Helper.ModRegistry.GetApi<IHoverLabelApi>("Achtuur.HoverLabels")`.
* Step 5: register your label (called `yourLabel` in the example): `labelApi.Register(this.ModManifest, "yourLabelName", new yourLabel())`.









<h2 id="IHoverLabel-tutorial"> IHoverLabel </h2>

You can make your own label class by copying over `IHoverLabel` and creating a new class that implements the interface. The following example will focus on creating a custom label that appears when you hover over a tile that is tilled.

### Simple label

To start off with, the condition to generate a label has to be specified. This is done using the `ShouldGenerateLabel(Vector2 cursorTile)` method. This method takes the tile the cursor is currently on and returns whether a label should be generated. In our case, a hoe'd tile means that `Game1.currentLocation.terrainFeatures` should contain a `HoeDirt` tile. The method would look something like:

```cs
public bool ShouldGenerateLabel(Vector2 cursorTile) 
{
    return Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile)
        && Game1.currentLocation.terrainFeatures[cursorTile] is HoeDirt;
}
```

Next, the label should generate a name when hovering over tilled dirt. Let's go for `"Tilled dirt"` as the name. To do this, the `GetName()` method is used.

```cs
public string GetName()
{
    return "Tilled dirt";
}
```

Finally, we want the description to say "This tile has been hoe'd", with "isn't that cool?" on the next line. To do this, the `GetDescription()` method is used. Every entry in the returned `IEnumerable<string>` is separated by a new line, which is why we use `yield return` here twice. You could also use `\n` for newlines, however it is recommended to use separate entries.

```cs
public IEnumerable<string> GetDescription()
{
    yield return "This tile has been hoe'd";
    yield return "Isn't that cool?";
}
```

Your class should then look something like this:

```cs
internal class ExampleLabel : IHoverLabel
{
    public int Priority { get; set; } = 0;

    public void DrawOnOverlay(SpriteBatch spriteBatch)
    {
    }

    public void UpdateCursorTile(Vector2 cursorTile)
    {
    }

    public bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile)
            && Game1.currentLocation.terrainFeatures[cursorTile] is HoeDirt;
    }

    public string GetName()
    {
        return "Tilled dirt";
    }

    public IEnumerable<string> GetDescription()
    {
        yield return "This tile has been hoe'd";
        yield return "Isn't that cool?";
    }
}
```

The final step is to use the `IHoverlabelApi` to register your label. Note that this should be done in the `GameLaunched` event or after, NOT in `Mod.Entry`. Registering is done in the following way after copying the `IHoverLabelApi` into your own code:

```cs
IHoverLabelApi labelApi = this.Helper.ModRegistry.GetApi<IHoverLabelApi>("Achtuur.HoverLabels");
labelApi.Register(this.ModManifest, "Example", new ExampleLabel());
```

Note that the name when registering should be unique with respect to all the labels in your mod. If you were to register two labels with the same name, the first label will be overwritten by the second.

The final result should look like this (notice the cursor is on top of tilled dirt):

![Example label showing tilled dirt](./images//examplelabel.png)


### Advanced

In order to have more advanced logic inside your class, the `UpdateCursorPosition(Vector2 cursorTile)` is provided. This method is called when your label class should update its name and description based on the current cursor tile. The `cursorTile` is exact same cursor tile as is input into `ShouldGenerateLabel(Vector2 cursorTile)`.

There is also a priority field. When you have multiple labels that have similar conditions, this can be useful. For example, if you have a label for any tree, and a label specifically for maple trees, you want the maple tree label to have a higher priority. The labels provided by this mod have a priority of 0, so if you don't see your label when you think you should, consider increasing the priority.

Finally, `DrawOnOverlay` is called right before the label gets rendered to the screen. The point of this function is to use the `spriteBatch` to draw to the screen, in order to convey extra information. Example use cases for this in the mod are the sprinkler, scarecrow and Junimo hut ranges being shown when hovering over them.



