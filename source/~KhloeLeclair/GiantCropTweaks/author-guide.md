**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

← [README](README.md)

## Contents

* [Getting Started](#getting-started)
* [Nightly Events](#nightly-events)
  * [Conditions](#conditions)
  * [Side Effects](#side-effects)
* [Event Types](#event-types)
  * [`Placement`](#placement)
	* [Output](#output)
  * [`Script`](#script)
* [Commands](#commands)
* [Mod Data](#mod-data)
* * [Triggers](#triggers)


## Getting Started

### Create a Content Pack

Giant Crop Tweaks does not support content packs of its own. Instead, I
expect you to use Content Patcher to edit its data. Specifically, you'll
be using the `EditData` action with the target:
```
Mods/leclair.giantcroptweaks/Data
```

For details on its format, see the following:

## Data Format

The Giant Crop Tweaks data format expects each entry to correspond
with an entry in `Data/GiantCrops`. So, say you add your own giant
crop with a patch like this:
```json
{
	"Action": "EditData",
	"Target": "Data/GiantCrops",
	"Entries": {
		"{{ModId}}_MyCoolCrop": {
			// stuff here
		}
	}
}
```

You'd want to then add another patch, hitting GCT's asset:
```json
{
	"Action": "EditData",
	"Target": "Mods/leclair.giantcroptweaks/Data",
	"Entries": {
		"{{ModId}}_MyCoolCrop": {
			// stuff goes here!
		}
	}
}
```

See?

Two data entries in two separate assets, but both dealing with
the same specific giant crop.

Our asset supports the following properties:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr><th colspan=2>Identity</th></tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The giant crop's unique Id. This should match the Id
of an entry in `Data/GiantCrops`.

</td>
</tr>
<tr><th colspan=2>Behaviors</th></tr>
<tr>
<td><code>CanGrowWhenNotFullyRegrown</code></td>
<td>

*Optional.* If you set this to true, then the giant crop will only
be able to grow when the crop it's growing from is fully grown. This
is particularly important if you have a re-growable crop that gets
re-planted, but the giant crop has a high potential chance to spawn.

In that situation, without this, the giant crop would be able to
spawn more frequently than you might want.

Default: `false`

</td>
</tr>
<tr>
<td><code>ShouldReplant</code></td>
<td>

*Optional.* Whether or not this giant crop should re-plant its original
crop when it's harvested. Possible values:

* `Never`: This giant crop should never re-plant on harvest.
* `Always`: This giant crop should *always* re-plant on harvest.
* `WhenRegrowing`: This giant crop should re-plant on harvest if its
  original crop is a re-growing crop.

Default: `WhenRegrowing`

</td>
</tr>
<tr><th colspan=2>Colors</th></tr>
<tr>
<td><code>Colors</code></td>
<td>

*Optional.* A list of color values that this giant crop can be. The color
is used for rendering the overlay, if an overlay texture is set. It may
also be used for coloring the harvested items, if that is enabled with
`HarvestItemsToColor`.

</td>
</tr>
<tr>
<td><code>UseBaseCropTintColors</code></td>
<td>

*Optional.* If you set this to true, `Colors` will be ignored and,
instead, the giant crop will try to read its color list from its
original crop.

Default: `false`

</td>
</tr>
<tr>
<td><code>HarvestItemsToColor</code></td>
<td>

*Optional.* A list of item Ids of harvest items that should be converted
into colored items when this giant crop is harvested. This can be used
to cause some or all of the harvest items to take on a color from this
giant crop, giving you more specific control over what exact items
get colored.

</td>
</tr>
<tr>
<td><code>RandomizeHarvestItemColors</code></td>
<td>

*Optional.* If this is set, each item from each `HarvestItems` block for
this giant crop will get its own color, assuming the item is in
`HarvestItemsToColor`.

If this is *not* set, then the giant crop's color is applied to all
relevant items.

This only works to drop multiple colors if you have more than one entry
in your giant crop's `HarvestItems` block. I recommend making copies
of your harvest item entry and reducing the stack counts appropriately
to allow multiple colors to show up.

Default: `false`

</td>
</tr>
<tr><th colspan=2>Overlay Texture</th></tr>
<tr>
<td><code>OverlayTexture</code></td>
<td>

*Optional.* A texture to draw over this giant crop. Overlay textures
are used for drawing a color, without needing to apply that color to
your entire giant crop.

</td>
</tr>
<tr>
<td><code>OverlayPrismatic</code></td>
<td>

*Optional.* If this is true, the crop will render with prismatic
colors rather than one of the set colors from `Colors`.

</td>
</tr>
<tr>
<td><code>OverlayPosition</code></td>
<td>

*Optional.* The position in `OverlayTexture` to draw this giant
crop with. If this is not set, this will default to the
`TexturePosition` of the relevant `Data/GiantCrops` entry. You
may need to set this to `{"X": 0, "Y": 0}` deliberately.

</td>
</tr>
<tr>
<td><code>OverlaySize</code></td>
<td>

*Optional.* The size of the overlay to render, in tiles. If this
is not set, the size will be taken from the `TileSize` of the
relevant `Data/GiantCrops` entry.

</td>
</tr>
<tr>
<td><code>OverlayOffset</code></td>
<td>

*Optional.* The offset to render the overlay at, in tiles.

Default: `{"X": 0, "Y": 0}`

</td>
</tr>
</table>
