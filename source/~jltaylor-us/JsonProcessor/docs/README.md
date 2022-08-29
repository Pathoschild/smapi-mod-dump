**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewJsonProcessor**

----

# Json Processor

This mod is for other mod authors, and there is no reason for end users to install it unless
required by another mod.  You can download it from [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/13183).

**Json Processor** transforms JSON trees using pluggable transformers.  It provides a CLI in the
SMAPI console, as well as advanced APIs for changing the set of tranformers in use and for implementing
new transformers.  You can use Json Processor to reduce boilerplate in your JSON files, making them
more compact and maintainable.

# Use as a development-time tool

The simplest way to use Json Processor is to transform your JSON files during development and
include the expanded output in your mod distribution.  The Json Processor mod provides the
`process-json` command in the SMAPI console for this purpose.

(Why is this implemented as a SMAPI mod instead of a standalone tool?  Hosting the tool inside SDV
itself means no dealing with the complexities of providing a cross-platform tool, setting up library
paths, etc.)

The `process-json` command uses the default set of transformers; see [that
documentation](default-transformers.md) for information about how you can use those to reduce
boilerplate in your JSON files.

Run `process-json help` for help with the syntax of the command.

# Use at run-time

Json Processor also provides an API that you can invoke during the game (just like other mods).  If
all you need is to transform a JSON tree using the default transformers, then use the [simple
API](../IJsonProcessorSimpleAPI.cs).  The [full api](../IJsonProcessorAPI.cs) includes functions to add
and remove transformers from the processor, as well as the interfaces for defining new transformers.

## Use from a Content Patcher mod

The [Content Patcher Json Processor](../../ContentPatcherJsonProcessor/) mod provides integration
between Json Processor and Content Patcher so that you can write your `content.json` files with Json
Processor transformers.  See that mod for more information.

# Quick-start examples

So how do you use this thing to make some more convenient JSON, anyway?

## Example 1:  Removing repeated text

Let's suppose you are making an accessory for [Fashion
Sense](https://github.com/Floogen/FashionSense), similar to [this
example](https://github.com/Floogen/FashionSense/wiki/Accessory-Model-Properties#all-four-accessory-models-example),
and you have noticed that the `MovementAnimation` section of each direction of your accessory model
is identical.  Rather than repeating those definitions, you can use the `define` and `var`
transformers of the Json Processor to say that text only once.  At the beginning of accessory JSON
file, define the animation sequence:

```jsonc
{
  "Name": "My Accessory",
  "$define": {
    "animation seq": [
      {
        "Frame": 0,
        "Duration": 100
      },
      {
        "Frame": 1,
        "Duration": 100,
        "OverrideStartingIndex": true
      },
      {
        "Frame": 2,
        "Duration": 100
      },
      {
        "Frame": 3,
        "Duration": 100
      }
    ]
  },
  "FrontAccessory": {
  // etc...
```

Then in each of the places you need to repeat that text, reference the definition instead:

```jsonc
  // ...
  "FrontAccessory": {
    "DisableGrayscale": true,
    // ...
    "MovementAnimation": { "$var": "animation seq" }
  },
  "RightAccessory": {
    "DisableGrayscale": true,
    // ...
    "MovementAnimation": { "$var": "animation seq" }
  },
  // ...
```

Once you've constructed a JSON file using Json Processor transformers, you can use the SMAPI command
line interface provided by Json Processor to process the file and write the results to a new JSON
file.  For convenience, let's say you've put the file inside the Json Processor mod folder and
called it `input.json`.  (You can use full file paths instead of putting the file in the Json
Processor mod folder, but that doesn't make for a very compact example.)  Run the process command in
the SMAPI console:

```
process-json input.json output.json
```

This will run the processor with the default set of transformers and write the output into
`output.json`.  You can then copy or move `output.json` into the appropriately named file in your
Fashion Sense mod.

## Example 2: List generation

Suppose that you are making a Fashion Sense content pack and need to make a lengthy
[conditions list](https://github.com/Floogen/FashionSense/wiki/Conditions-Properties) to disable
animations while the player is performing various actions.  Writing this out by hand might look
something like

```jsonc
  "Conditions": [
    {
      "Name": "IsDrinking",
      "Value": false
    },
    {
      "Name": "IsCasting",
      "Value": false
    },
    // etc...
  ]
```

Rather than writing out the same clause for a dozen different condition names, you can use the
`for-each` transformer to write the list of names and apply a template for each one.

```jsonc
  "Conditions": {
    "$transform": "for-each",
    "var": "condition name",
    "in": [
      "IsDrinking",
      "IsCasting",
      // etc...
    ],
    "yield": {
      "Name": { "$var": "condition name" }
      "Value": false
    }
  }
```

# See Also

* [Release notes](release-notes.md)

